using NHibernate.Engine;
using NHibernate.Linq;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly IUoW _uow;

        public Repository(IUoW uow)
        { _uow = uow; }


        /// <summary>
        /// Get the entity with the given Id
        /// </summary>
        /// <param name="id">Int64</param>
        /// <returns>TEntity</returns>
        public TEntity Get(Int64 id)
        {
            return _uow.Session.Get<TEntity>(id);
        }

        /// <summary>
        /// Get the entity with the given Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>TEntity</returns>
        public TEntity Get(int id)
        {
            return _uow.Session.Get<TEntity>(id);
        }

        /// <summary>
        /// Get entity with the 
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        public TEntity Get(string columnName, string columnValue)
        {
            return _uow.Session.Get<TEntity>(0);
        }


        /// <summary>
        /// Get count
        /// </summary>
        /// <returns>Int64</returns>
        public Int64 Count() => _uow.Session.QueryOver<TEntity>().RowCountInt64();


        /// <summary>
        /// Get count
        /// </summary>
        /// <returns>Int64</returns>
        public Int64 Count(Expression<Func<TEntity, bool>> predicate) => _uow.Session.Query<TEntity>().Where(predicate).LongCount();

        /// <summary>
        /// Get int count
        /// </summary>
        /// <returns>int</returns>
        public int IntCount(Expression<Func<TEntity, bool>> predicate) => _uow.Session.Query<TEntity>().Where(predicate).Count();

        public void Add(TEntity entity)
        {
            _uow.Session.Save(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }


        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Save a bunch of datatables
        /// </summary>
        /// <param name="dataTable">List{DataTable}</param>
        /// <param name="tableName"></param>
        public bool SaveBundle(List<DataTable> listOfDataTables, string tableName)
        {
            try
            {
                using (var connection = (SqlConnection)(((ISessionFactoryImplementor)_uow.Session.SessionFactory).ConnectionProvider.GetConnection()))
                {
                    using (SqlTransaction tranx = connection.BeginTransaction())
                    {
                        using (var copy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, tranx))
                        {
                            try
                            {
                                copy.BulkCopyTimeout = 10000;
                                copy.DestinationTableName = tableName;
                                foreach (DataColumn column in listOfDataTables[0].Columns) { copy.ColumnMappings.Add(column.ColumnName, column.ColumnName); }

                                listOfDataTables.ForEach(dataTable =>
                                {
                                    copy.WriteToServer(dataTable);
                                });
                                tranx.Commit();
                            }
                            catch (Exception exception)
                            {
                                tranx.Rollback();
                                throw exception;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Save a large bunch.
        /// <para>For chunked records you might want to look at this method 
        /// SaveBundle(List<DataTable> listOfDataTables, string tableName), which would take the chunked records, 
        /// save and commit all the records or roll back
        /// </para>
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        public bool SaveBundle(DataTable dataTable, string tableName)
        {
            try
            {
                using (var connection = ((ISessionFactoryImplementor)_uow.Session.SessionFactory).ConnectionProvider.GetConnection())
                {
                    //using (var tranx = connection.BeginTransaction())
                    {
                        try
                        {
                            var serverCon = (SqlConnection)connection;
                            var copy = new SqlBulkCopy(serverCon);
                            copy.BulkCopyTimeout = 10000;
                            copy.DestinationTableName = tableName;
                            foreach (DataColumn column in dataTable.Columns) { copy.ColumnMappings.Add(column.ColumnName, column.ColumnName); }
                            copy.WriteToServer(dataTable);
                            //tranx.Commit();
                        }
                        catch (Exception exception)
                        {
                            //tranx.Rollback();
                            throw exception;
                        }
                    }
                }
                return true;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

    }
}
