using System.Collections.Generic;
using System.Data;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services.Contracts;
using NHibernate.Engine;
using System.Data.SqlClient;
using System;
using Orchard.Logging;
using System.Text;
using Parkway.CBS.ReferenceData;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Core.Services
{
    public class RefDataTempManager : IRefDataTempManager<RefDataTemp>
    {
        private readonly IRepository<RefDataTemp> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public RefDataTempManager(IRepository<RefDataTemp> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }


        public void SaveBundle(DataTable refDataTempDataTable, string tableName)
        {
            try
            {
                using (var connection = ((ISessionFactoryImplementor)_transactionManager.GetSession().SessionFactory).ConnectionProvider.GetConnection())
                {
                    var serverCon = (SqlConnection)connection;
                    var copy = new SqlBulkCopy(serverCon);
                    copy.BulkCopyTimeout = 10000;
                    copy.DestinationTableName = tableName;
                    foreach (DataColumn column in refDataTempDataTable.Columns)
                    {
                        copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                    copy.WriteToServer(refDataTempDataTable);
                }
            }
            catch (Exception exception)
            {
                Logger.Error("ERROROROR", exception);
            }
        }


        /// <summary>
        /// Update the value of the given columns
        /// </summary>
        /// <param name="columnNameAndValue"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        public void UpdateRefDataTemp(Dictionary<string, string> columnNameAndValue, string batchNumber, int revenueHeadId, int billingId)
        {
            using (var session  = _transactionManager.GetSession().SessionFactory.OpenStatelessSession())
            {
                using (var tranx = session.BeginTransaction())
                {
                    try
                    {
                        StringBuilder baseQL = new StringBuilder("Update " + typeof(RefDataTemp).FullName + " r SET ");
                        //loop through set values
                        int hasMoreItems = columnNameAndValue.Count;
                        int paramSize = columnNameAndValue.Count;

                        foreach (var item in columnNameAndValue)
                        {
                            if (hasMoreItems != 1) { baseQL.AppendFormat("r." + item.Key + " = :{0}, ", item.Key); hasMoreItems--; }
                            else { baseQL.AppendFormat("r." + item.Key + " = :{0} ", item.Key); }
                        }
                        //add the where clause
                        baseQL.AppendFormat("WHERE r.BatchNumber = '{0}' AND r.RevenueHeadId = '{1}' AND r.BillingModelId = '{2}'", batchNumber, revenueHeadId.ToString(), billingId.ToString());

                        var query = session.CreateQuery(baseQL.ToString());

                        foreach (var item in columnNameAndValue) { query.SetParameter(item.Key, item.Value); }

                        query.ExecuteUpdate();
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception.Message, exception);
                    }
                }
            }
                
        }
    }
}