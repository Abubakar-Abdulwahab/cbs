using Orchard.Data;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Orchard.Users.Models;
using Orchard;
using Parkway.CBS.Core.Models;
using NHibernate.Engine;
using System.Data.SqlClient;
using System.Data;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Core.Services
{
    public abstract class BaseManager<M> where M : CBSBaseModel
    {
        private readonly IRepository<M> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly ITransactionManager _transactionManager;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }

        public BaseManager(IRepository<M> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices)
        {
            _repository = repository;
            _user = user;
            Logger = NullLogger.Instance;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Start UOW transaction
        /// </summary>
        public void StartUOW()
        {
            _transactionManager.GetSession().BeginTransaction();
        }


        /// <summary>
        /// Start UOW transaction
        /// </summary>
        public void EndUOW()
        {
            _transactionManager.GetSession().Transaction.Commit();
            _transactionManager.GetSession().Transaction.Dispose();
        }


        /// <summary>
        /// Roll back all transactions
        /// </summary>
        public void RollBackAllTransactions()
        {
            try
            {
                _transactionManager.GetSession().Transaction.Rollback();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error cleaning up transactions " + exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get user object model with the given TaxEntityId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserPartRecord</returns>
        public UserPartRecord User(int id)
        {
            try
            {
                return _user.Get(id);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Get user object model with the given TaxEntityId
        /// </summary>
        /// <param name="id"></param>
        /// <returns>UserPartRecord</returns>
        public UserPartRecord User(string email)
        {
            try
            {
                return _user.Get(u => u.Email == email);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Get model with the given id.
        /// Returns Null if exception. Check for null.
        /// </summary>
        /// <param name="id">TaxEntityId of the record</param>
        /// <returns><paramref name="M"/>Model</returns>
        public M Get(int id)
        {
            try { return _repository.Get(id); }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return null;
            }
        }

        /// <summary>
        /// Get model based where column = variable
        /// <para>Must always resolve to a string value. That is it can only query columns of string data types 
        /// e.g Code column stores variaables as string, User_Id doesn't.</para>
        /// </summary>
        /// <param name="column">Name of the column the select operation is done on</param>
        /// <param name="variable">Comparison value.</param>
        /// <returns><typeparamref name="M"/>Model</returns>
        public M Get(string column, string variable)
        {
            try
            {
                ParameterExpression argParam = Expression.Parameter(typeof(M), "r");
                Expression nameProperty = Expression.Property(argParam, column);

                var comparisonValue = Expression.Constant(variable);
                Expression expression = Expression.Equal(nameProperty, comparisonValue);

                var lambda = Expression.Lambda<Func<M, bool>>(expression, argParam);
                return _repository.Get(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Get model
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns><typeparamref name="M"/>Model</returns>
        public virtual M Get(Expression<Func<M, bool>> lambda)
        {
            try
            {
                return _repository.Get(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        public int Count(Expression<Func<M, bool>> lambda)
        {
            try
            {
                return _repository.Count(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Evict object from cache
        /// </summary>
        /// <param name="entity"></param>
        public void Evict(M entity)
        {
            try
            {
                _transactionManager.GetSession().Evict(entity);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Clear session
        /// </summary>
        public void ClearSession()
        {
            try
            {
                _transactionManager.GetSession().Clear();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Save the model object
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        public virtual bool Save(M model)
        {
            try
            {
                _repository.Create(model);
            }
            catch (Exception exception)
            {
                _transactionManager.GetSession().Transaction.Rollback();
                Logger.Error(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Save a large bunch
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        public bool SaveBundle(DataTable dataTable, string tableName)
        {
            try
            {
                IDbConnection connection = _transactionManager.GetSession().Connection;
                var cmd = new SqlCommand();
                cmd.Connection = (SqlConnection)connection;
                _transactionManager.GetSession().Transaction.Enlist(cmd);
                SqlConnection serverCon = (SqlConnection)connection;
                SqlBulkCopy copy = new SqlBulkCopy(serverCon, SqlBulkCopyOptions.Default, cmd.Transaction);
                copy.BulkCopyTimeout = 10000;
                copy.DestinationTableName = tableName;
                foreach (DataColumn column in dataTable.Columns) { copy.ColumnMappings.Add(column.ColumnName, column.ColumnName); }
                copy.WriteToServer(dataTable);
                return true;
            }
            catch (Exception exception)
            {
                _transactionManager.GetSession().Transaction.Rollback();
                Logger.Error(exception, exception.Message + " Error trying to save against " + tableName);
            }
            return false;
        }


        [Obsolete]
        /// <summary>
        /// Save a bunch of Models of the same type
        /// <para>METH IS OK, BUT FOR THE COLLECTION IF CHUNKED,
        /// YOU MIGHT WANT TO COMMIT AFTER ALL THE CHUNKS HAVE BEEN SAVED SUCCESSFULLY</para>
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        public bool SaveBundle(ICollection<M> collection)
        {
            //TODO: meth is ok, but for the collection if chunked,
            // you might want to commit after all the chunks have been saved successfully
            M itemToSave = null;
            int counter = 1;
            using (var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
            {
                session.SetBatchSize(100);
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
                        Logger.Error(exception, string.Format("Could not save object "));
                        tranx.Rollback();
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Save a bunch of Models of the same type
        /// <para>This method doesn't commit immediately. It returns the index of the item that failed to insert else it returns -1</para>
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>int</returns>
        public int SaveBundleUnCommitStatelessWithErrors(ICollection<M> collection)
        {
            int counter = 0;
            try
            {
                var session = _transactionManager.GetSession();
                foreach (var item in collection)
                {
                    counter++;
                    session.Save(item);
                }
                return -1;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Could not save object "));
                _transactionManager.GetSession().Transaction.Rollback();
            }
            return counter;
        }




        /// <summary>
        /// Save a list. This is transactionless and relies on the main app blanket transaction
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        public bool SaveBundleUnCommit(ICollection<M> collection)
        {
            //TODO: meth is ok, but for the collection if chunked,
            // you might want to commit after all the chunks have been saved successfully
            var session = _transactionManager.GetSession();
            try
            {
                foreach (var item in collection)
                {
                    session.Save(item);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Could not save object "));
                _transactionManager.GetSession().Transaction.Rollback();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Save a bunch of Models of the same type
        /// <para>This method doesn't commit immediately until all other requests of the caller is successful</para>
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        public bool SaveBundleUnCommitStateless(ICollection<M> collection)
        {
            //TODO: meth is ok, but for the collection if chunked,
            // you might want to commit after all the chunks have been saved successfully
            //M itemToSave = null;
            //int counter = 1;
            var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession();
            try
            {
                foreach (var item in collection)
                {
                    //itemToSave = item;
                    session.Insert(item); /*counter++;*/
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Could not save object "));
                _transactionManager.GetSession().Transaction.Rollback();
                return false;
            }

            return true;
        }


        /// <summary>
        /// Delete records
        /// </summary>
        /// <param name="collection"></param>
        /// <returns>bool</returns>
        public bool DeleteBundle(ICollection<M> collection)
        {
            var session = _transactionManager.GetSession().SessionFactory.OpenStatelessSession().SetBatchSize(collection.Count);
            using (var tranx = session.BeginTransaction())
            {
                try
                {
                    foreach (var item in collection)
                    {
                        session.Delete(item);
                    }
                    tranx.Commit();
                }
                catch (Exception exception)
                {
                    tranx.Rollback();
                    Logger.Error(exception, string.Format("Could not delete object ", Utilities.Util.SimpleDump(collection)));
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Updaate the model
        /// </summary>
        /// <param name="model"></param>
        /// <returns>bool</returns>
        public bool Update(M model)
        {
            model.UpdatedAtUtc = DateTime.Now.ToLocalTime();
            try { _repository.Update(model); }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Get a collection of model in ascending order by string
        /// </summary>
        /// <param name="lambda">lambda expression</param>
        /// <param name="order">Ordering</param>
        /// <returns>IEnumerable{M}</returns>
        public IEnumerable<M> GetCollection(Expression<Func<M, bool>> lambda, string order)
        {
            try
            {
                ParameterExpression argParam = Expression.Parameter(typeof(M), "keySelector");
                Expression orderProp = Expression.Property(argParam, order);

                var pr = Expression.Lambda<Func<M, string>>(orderProp, argParam);
                return _repository.Fetch(lambda, o => o.Asc(pr));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get collection of M
        /// </summary>
        /// <returns>IEnumerable{M}</returns>
        public IEnumerable<M> GetCollection()
        {
            try
            {
                return _repository.Table.ToList<M>();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get a collection of model
        /// </summary>
        /// <param name="lambda">lambda expression</param>
        /// <returns>IEnumerable{M}</returns>
        public IEnumerable<M> GetCollection(Expression<Func<M, bool>> lambda)
        {
            try
            {
                return _repository.Fetch(lambda);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get a collection of M order by string value, take and skip option.
        /// </summary>
        /// <param name="lambda">Expression</param>
        /// <param name="take">Int value</param>
        /// <param name="skip">Int value</param>
        /// <returns>IEnumerable{M}</returns>
        public IEnumerable<M> GetCollection(Expression<Func<M, bool>> lambda, int take, int skip = 0)
        {
            try
            {
                return _repository.Fetch(lambda, o => o.Asc(i => i.CreatedAtUtc), skip, take);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}