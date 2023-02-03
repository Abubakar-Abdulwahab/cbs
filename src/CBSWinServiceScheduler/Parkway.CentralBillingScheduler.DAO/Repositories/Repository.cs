using NHibernate;
using NHibernate.Linq;
using Parkway.CentralBillingScheduler.DAO.Models;
using Parkway.CentralBillingScheduler.DAO.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using NHibernate.Engine;
using System.Data.SqlClient;

namespace Parkway.CentralBillingScheduler.DAO.Repositories
{
    public class Repository<M> : IRepository<M> where M : CBSBaseModel
    {
        private readonly ISession _session;

        public Repository(ISession session)
        {
            _session = session;
        }

        public bool Add(M entity)
        {
            throw new NotImplementedException();
        }

        public bool Add(IEnumerable<M> items)
        {
            throw new NotImplementedException();
        }

        public bool Delete(M entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(IEnumerable<M> entities)
        {
            throw new NotImplementedException();
        }

        public M Get(int id)
        {
            try
            {
                return _session.Get<M>(id);
            }
            catch (Exception exception)
            {
                return null;
            }
        }


        /// <summary>
        /// Get model
        /// </summary>
        /// <param name="lambda"></param>
        /// <returns><typeparamref name="M"/>Model</returns>
        public IList<M> Get(Expression<Func<M, bool>> lambda)
        {
            try
            {
                return _session.Query<M>().Where(lambda).ToList<M>();
            }
            catch (Exception exception)
            {
                return null;
            }
        }

        public bool SaveBundle(DataTable dataTable, string tableName)
        {
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
                return false;
            }
        }

        public bool Update(M entity)
        {
            throw new NotImplementedException();
        }
    }
}
