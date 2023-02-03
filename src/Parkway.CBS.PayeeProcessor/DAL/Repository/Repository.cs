using log4net;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.PayeeProcessor.DAL.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {

        private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly ISession _session;
        public Repository(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Query a particular table entity and return all record 
        /// </summary>
        public virtual IQueryable<TEntity> Table
        {
            get { return Queryable(); }
        }

        /// <summary>
        /// Count and return total number of Record in a table entity using a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Fetch(predicate).Count();
        }

        /// <summary>
        /// Get a List of all Item using 
        /// </summary>
        /// <param name="queryDef"></param>
        /// <returns></returns>
        public IList<TEntity> GetAllList(IEnumerable<KeyValuePair<String, Object>> queryDef = null)
        {
            var crt = _session.CreateCriteria<TEntity>();

            foreach (var item in queryDef)
            {
                crt.Add(Restrictions.Eq(item.Key, item.Value));
            }

            var result = crt.List<TEntity>();
            return result;

        }

        /// <summary>
        /// Delete an Object from the database using Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveNow"></param>

        public void Delete(object id, bool saveNow = true)
        {
            Delete(_session.Get<TEntity>(id));
            if (saveNow)
                SaveChanges();
        }

        /// <summary>
        /// Delete an entity record from the database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        public void Delete(TEntity entity, bool saveNow = true)
        {
            _session.Delete(entity);
            if (saveNow)
                SaveChanges();
        }


        /// <summary>
        /// Delete a range of entity record from the database
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="save"></param>
        public void DeleteRange(IEnumerable<TEntity> entities, bool save = true)
        {
            foreach (var entity in entities)
            {
                Delete(entity, save);
            }
            if (save)
                SaveChanges();
        }


        /// <summary>
        /// Fetch certain number of record for the database and perform filtering and ordering on the record set.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, 
            IOrderedQueryable<TEntity>> orderBy = null, int? page = default(int?), int? pageSize = default(int?))
        {
            IQueryable<TEntity> query = Table;
            if(orderBy != null)
            {
                query = orderBy(query);
            }
            if(predicate != null)
            {
                query = query.Where(predicate);

            }
            if(page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);

            }
            return query;
        }


        /// <summary>
        /// Perform a find using an expression syntax
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.SingleOrDefault(predicate);
        }

        /// <summary>
        /// Finds an entity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity Find(object id)
        {
            Logger.Info($"Finding record for entity with Id {id}");
            return _session.Get<TEntity>(id);
        }

        /// <summary>
        /// Perform a find using a set of key values
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public TEntity Find(params object[] keyValues)
        {
            Logger.Info($"Find record for entity with KeyValue {keyValues[0]}");
            return _session.Get<TEntity>(keyValues[0]);
        }


        /// <summary>
        /// Insert an entity or item into the database.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        public void Insert(TEntity entity, bool saveNow = true)
        {
            _session.Clear();
            _session.Save(entity);
            if (saveNow)
                SaveChanges();
        }


        /// <summary>
        /// Insert a range of entities into the datbase and commit to the database
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="saveNow"></param>
        public void InsertRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            foreach(var entity in entities)
            {
                Insert(entity, false);
            }
        }


        /// <summary>
        /// Return a queryable dataset
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> Queryable()
        {
            return _session.Query<TEntity>();
        }

        public void SaveBundle(IEnumerable<TEntity> collection)
        {

            Logger.Info($"Saving bundle for entities with total number {collection.Count()}");
            TEntity itemToSave = null;
            int counter = 1;

            var statelessSession = _session.SessionFactory.OpenStatelessSession();

            statelessSession.SetBatchSize(collection.Count());
            using (var tranx = statelessSession.BeginTransaction())
            {
                try
                {
                    foreach(var item in collection)
                    {
                        itemToSave = item;
                        statelessSession.Insert(item);
                        counter++; 
                    }
                }
                catch(Exception ex)
                {
                    Logger.Error("An error occurred while saving bundle.", ex);
                    tranx.Rollback();
                }
            }
             
        }

        public bool SaveBundle(DataTable dataTable, string tableName)
        {
            Logger.InfoFormat($"Saving bundle for table {dataTable} with table Name {tableName}");
            try
            {
                using (var connection = ((ISessionFactoryImplementor)_session.SessionFactory).ConnectionProvider.GetConnection())
                {
                    var serverCon = (SqlConnection)connection;
                    var copy = new SqlBulkCopy(serverCon);
                    copy.BulkCopyTimeout = 10000;
                    copy.DestinationTableName = tableName;
                    foreach (DataColumn column in dataTable.Columns) { copy.ColumnMappings.Add(column.ColumnName, column.ColumnName); }
                    copy.WriteToServer(dataTable);
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error("Error ocurred while saving bundle. See details", exception);
                _session.Transaction.Rollback();

            }
            return false;
        }

        public bool SaveBundle(ICollection<TEntity> collection)
        {
            Logger.Info($"Saving bundle for entities total number {collection.Count()}");

            TEntity itemToSave = null;
            int counter = 1;

            var statelessSession = _session.SessionFactory.OpenStatelessSession();

            statelessSession.SetBatchSize(collection.Count());
            using (var tranx = statelessSession.BeginTransaction())
            {
                try
                {
                    foreach (var item in collection)
                    {
                        itemToSave = item;
                        statelessSession.Insert(item);
                        counter++;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("Error ocurred while saving bundle. See details ", ex);
                    tranx.Rollback(); 
                    throw;
                }
            }
            return true;

        }

        public int SaveChanges()
        {
            _session.Flush();
            return 1;
        }

        public void Update(TEntity entity, bool saveNow = true)
        {
            _session.Merge(entity);
            if (saveNow)
                SaveChanges();
        }

        public void UpdateRange(IEnumerable<TEntity> entities, bool save = true)
        {
            foreach (var entity in entities)
            {
                Update(entity, save);
            }
            if (save)
                SaveChanges();
        }
    }
}
