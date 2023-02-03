using CBSPay.Core.Interfaces;
using log4net;
using NHibernate;
using Parkway.Tools.NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using NHibernate.Linq;
using NHibernate.Engine;
using System.Data.SqlClient;
 
namespace CBSPay.Core.Services
{
    public class Repository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {

        public ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }

        
        protected readonly ISession _session;

        public Repository()
        {
            _session = SessionManager.GetInstance("CBSPaySessionFactory", "Default").GetSession();
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
            _session.Evict(entity);
            //_session.Merge(entity);
            _session.Update(entity);
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

            var _sessionFactory = (ISessionFactoryImplementor)SessionManager.GetInstance("CBSPaySessionFactory", "Default").GetSession().SessionFactory;

            var statelessSession = _sessionFactory.OpenStatelessSession();
           
            
            statelessSession.SetBatchSize(collection.Count());
            using (var tranx = statelessSession.BeginTransaction())
            {
                try
                {
                    foreach (var item in collection)
                    {
                        itemToSave = item;
                        statelessSession.Insert(item); counter++;
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
            var _sessionFactory = (ISessionFactoryImplementor)SessionManager.GetInstance("CBSPaySessionFactory", "Default").GetSession().SessionFactory;
            try
            {
                
                using (var connection = _sessionFactory.ConnectionProvider.GetConnection())
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
                SessionManager.GetInstance("CBSPaySessionFactory", "Default").GetSession().Transaction.Rollback();

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

            var _sessionFactory = (ISessionFactoryImplementor)SessionManager.GetInstance("CBSPaySessionFactory", "Default").GetSession().SessionFactory;

            var statelessSession = _sessionFactory.OpenStatelessSession();

            TEntity itemToSave = null;
            int counter = 1;
            using (var session = statelessSession)
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
