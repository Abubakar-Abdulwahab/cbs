using NHibernate;
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

namespace PayeeProcessor.DAL.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
         
        //private readonly NHibernateSessionManager SessionManager;
        protected readonly ISession _session;

        public Repository()
        {
            _session = new NHibernateSessionManager().OpenSession();
            
        }
         
         
        public virtual IQueryable<TEntity> Table
        {
            get { return Queryable(); }
        }

        /// <summary>
        /// Perform a find using a set of key values
        /// </summary>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public virtual TEntity Find(params object[] keyValues)
        { 
            return _session.Get<TEntity>(keyValues[0]);
        }

        /// <summary>
        /// Perform a Find using an expression syntax 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Table.SingleOrDefault(predicate);
        }


        /// <summary>
        /// Perform an Entity find by Id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Find(object id)
        { 
            return _session.Get<TEntity>(id);
        }

        public virtual void Insert(TEntity entity, bool saveNow = true)
        {
            _session.Clear();
            _session.Save(entity);
            if (saveNow)
                SaveChanges();
        }

        /// <summary>
        /// Perform a database insertion using 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="saveNow"></param>
        public virtual void InsertRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            foreach (var entity in entities)
            {
                Insert(entity, false);
            }
            if (saveNow)
                SaveChanges();
        }

        /// <summary>
        /// returns a queryable dataset
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Queryable()
        { 
            return _session.Query<TEntity>();
        }

        //public virtual IList<TEntity> GetList()
        //{
        //    return _session.QueryOver<TEntity>().List();
        //}

        //public virtual IList<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        //{
        //    return _session.QueryOver<TEntity>().Where(c=> c.Equals(predicate)).List();
        //} 

        /// <summary>
        /// Delete an entire source code 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        public virtual void Delete(TEntity entity, bool saveNow = true)
        {
            _session.Delete(entity);
            if (saveNow)
                SaveChanges();
        }

        /// <summary>
        /// Delete an object from database using id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveNow"></param>
        public virtual void Delete(object id, bool saveNow = true)
        {
            Delete(_session.Get<TEntity>(id));
            if (saveNow)
                SaveChanges();
        }

        /// <summary>
        /// Delete a range of objects 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="save"></param>

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool save = true)
        {
            foreach (var entity in entities)
            {
                Delete(entity, save);
            }
            if (save)
                SaveChanges();
        }


        /// <summary>
        /// Update an Entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveNow"></param>
        public virtual void Update(TEntity entity, bool saveNow = true)
        {
            //_session.Evict(entity);
            _session.Merge(entity);
            if (saveNow)
                SaveChanges();
        }

        /// <summary>
        /// Update a set of collections to the database 
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="save"></param>
        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool save = true)
        {
            foreach (var entity in entities)
            {
                Update(entity, save);
            }
            if (save)
                SaveChanges();
        }

        /// <summary>
        /// Perform Count on a query result using the Expression predicate  
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Fetch(predicate).Count();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
           IOrderedQueryable<TEntity>> orderBy = null, int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = Table;

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        public virtual int SaveChanges()
        {
            _session.Flush();
            return 1;
        }


        /// <summary>
        /// Save a bunch of Models of the same type
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        public void SaveBundle(IEnumerable<TEntity> collection)
        {
            TEntity itemToSave = null;
            int counter = 1;
            var session = new NHibernateSessionManager().OpenStatelessSession();
            session.SetBatchSize(collection.Count());
            using (var tranx = session.BeginTransaction())
            {
                try
                {
                    foreach (var item in collection)
                    {
                        itemToSave = item;
                        session.Insert(item); counter++;
                    }
                    tranx.Commit();
                }
                catch (Exception exception)
                { 
                    tranx.Rollback();
                  
                }
            }
             
        }


        /// <summary>
        /// Save a large bunch/batch record 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        public bool SaveBundle(DataTable dataTable, string tableName)
        {
            try
            {
                using (var connection = ((ISessionFactoryImplementor)new NHibernateSessionManager().OpenSession().SessionFactory).ConnectionProvider.GetConnection())
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
                new NHibernateSessionManager().OpenSession().Transaction.Rollback();
                 
            }
            return false;
        }


        /// <summary>
        /// Save a bunch of Models of the same type
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        public bool SaveBundle(ICollection<TEntity> collection)
        {
            TEntity itemToSave = null;
            int counter = 1;
            using (var session = new NHibernateSessionManager().OpenStatelessSession())
            {
                session.SetBatchSize(collection.Count());
                using (var tranx = session.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in collection)
                        {
                            itemToSave = item;
                            session.Insert(item); counter++;
                        }
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    { 
                        tranx.Rollback();
                        throw;
                         
                    }
                }
            } 
            return true;
        }
    }
}
