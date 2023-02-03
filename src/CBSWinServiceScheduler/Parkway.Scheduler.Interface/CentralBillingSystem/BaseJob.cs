using NHibernate;
using NHibernate.Transform;
using Parkway.CBS.ReferenceData;
using Parkway.CBS.ReferenceData.Configuration;
using Parkway.CBS.ReferenceData.DataSource;
using Parkway.CBS.ReferenceData.DataSource.Contracts;
using Parkway.CentralBillingScheduler.DAO.Models;
using Parkway.CentralBillingScheduler.DAO.Repositories;
using Parkway.CentralBillingScheduler.DAO.Repositories.Contracts;
using Parkway.Scheduler.Interface.CentralBillingSystem.HelperModels;
using Parkway.Scheduler.Interface.Loggers.Contracts;
using Parkway.Scheduler.Interface.Remote;
using Parkway.Scheduler.Interface.Remote.Contracts;
using Serilog.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.Scheduler.Interface.CentralBillingSystem
{
    internal abstract class BaseJob
    {
        /// <summary>
        /// Generate the HTTP signature heder
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maggi">client secret hint salt ;o) </param>
        public string GenerateSignature(string value, string maggi)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(maggi);
            byte[] messageBytes = Encoding.UTF8.GetBytes(value);

            byte[] hashmessage = new HMACSHA256(keyByte).ComputeHash(messageBytes);

            // to lowercase hexits
            String.Concat(Array.ConvertAll(hashmessage, x => x.ToString("x2")));

            // to base64
            return Convert.ToBase64String(hashmessage);
        }

        public RemoteClientResponse CallRemoteClient(string URL, string verb, Dictionary<string, dynamic> headers, dynamic bodyParams = null, string headerContentType = "application/json", string bodyContentType = "application/json")
        {
            IRemoteClient remote = new RemoteClient();
            return remote.SendRequest(URL, verb, headers, bodyParams, headerContentType, bodyContentType);
        }


        /// <summary>
        /// Get unique identifier
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns>string</returns>
        protected string GetBatchNumber(string prefix)
        {
            //https://stackoverflow.com/questions/17632584/how-to-get-the-unix-timestamp-in-c-sharp#35425123
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
            TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.Ticks) - epochTicks;
            //return prefix + unixTicks.TotalSeconds.ToString(); Mock1520106946.26801
            return "Mock1520106946.26801";
        }


        /// <summary>
        /// Get ref data
        /// </summary>
        /// <param name="referenceDataSourceName"></param>
        /// <exception cref="Exception">if not reference data source is found</exception>
        protected List<RefDataTemp> GetRefData(string referenceDataSourceName, int revenueHeadId, string batchNumber, decimal amount, int billingId)
        {
            IEnumerable<IReferenceDataSource> _refDataSources = new List<IReferenceDataSource> { { new Mock() }, { new Adapter() }, { new ParkwayRefData() } };
            dynamic processDetails = new ExpandoObject();
            processDetails.BatchNumber = batchNumber;
            processDetails.BillingId = billingId;
            processDetails.Amount = amount;

            foreach (var refSource in _refDataSources)
            {
                //break;
                if (refSource.ReferenceDataSourceName() == referenceDataSourceName)
                {
                    RefData refData = GetRefDataType(referenceDataSourceName);
                    List<GenericRefDataTemp> result = refSource.GetActiveBillableTaxEntitesPerRevenueHead(revenueHeadId, refData, processDetails);
                    return result.Select(res => new RefDataTemp
                    {
                        AdditionalDetails = res.AdditionalDetails,
                        Address = res.Address,
                        Amount = res.Amount,
                        BatchNumber = res.BatchNumber,
                        BillingModelId = res.BillingModelId,
                        Email = res.Email,
                        Recipient = res.Recipient,
                        RevenueHeadId = res.RevenueHeadId,
                        Status = res.Status,
                        StatusDetail = res.StatusDetail,
                        TaxEntityCategoryId = res.TaxEntityCategoryId,
                        TaxIdentificationNumber = res.TaxIdentificationNumber,
                        UniqueIdentifier = res.UniqueIdentifier,
                    }).ToList();
                }
            }
            throw new Exception("No reference data source found for reference data source name " + referenceDataSourceName);
        }


        /// <summary>
        /// Get ref data type
        /// </summary>
        /// <param name="referenceDataSourceName"></param>
        /// <returns>RefData</returns>
        private RefData GetRefDataType(string referenceDataSourceName)
        {
            IRefDataConfiguration refDataConfig = new RefDataConfiguration();

            if (referenceDataSourceName == "Mock") { return new RefData() { Name = "Mock" }; }
            string stateName = "";
            throw new Exception("please fix");
            try { return refDataConfig.GetCollection(stateName).Where(rf => rf.Name == referenceDataSourceName).Single(); }
            catch (Exception exception) { throw new Exception("No reference data found " + exception.Message); }
        }



        protected bool StoreRefData(string tableName, List<RefDataTemp> entities, ISession session)
        {
            ISchedulerLogger logger = SchedulerInterface.GetLoggerInstance();
            //save entities into temp table
            int chunkSize = 500000;
            var dataSize = entities.Count;

            double pageSize = ((double)dataSize / (double)chunkSize);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }
            int stopper = 0;
            int skip = 0;
            int counter = 0;
            var startTime = Stopwatch.StartNew();

            try
            {
                while (stopper < pages)
                {
                    var dataTable = new DataTable(tableName);
                    dataTable.Columns.Add(new DataColumn("Recipient"));
                    dataTable.Columns.Add(new DataColumn("Address"));
                    dataTable.Columns.Add(new DataColumn("TaxEntityCategoryId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("BillingModelId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("RevenueHeadId", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("TaxIdentificationNumber"));
                    dataTable.Columns.Add(new DataColumn("UniqueIdentifier"));
                    dataTable.Columns.Add(new DataColumn("Email"));
                    dataTable.Columns.Add(new DataColumn("Amount"));
                    dataTable.Columns.Add(new DataColumn("BatchNumber"));
                    dataTable.Columns.Add(new DataColumn("Status", typeof(int)));
                    dataTable.Columns.Add(new DataColumn("AdditionalDetails"));
                    dataTable.Columns.Add(new DataColumn("StatusDetail"));
                    dataTable.Columns.Add(new DataColumn("CreatedAtUtc", typeof(DateTime)));
                    dataTable.Columns.Add(new DataColumn("UpdatedAtUtc", typeof(DateTime)));

                    int index = 0;
                    entities.Skip(skip).Take(chunkSize).ToList().ForEach(x =>
                    {
                        var row = dataTable.NewRow();
                        row["Recipient"] = x.Recipient;
                        row["Address"] = x.Address;
                        row["TaxEntityCategoryId"] = x.TaxEntityCategoryId;
                        row["BillingModelId"] = x.BillingModelId;
                        row["RevenueHeadId"] = x.RevenueHeadId;
                        row["TaxIdentificationNumber"] = x.TaxIdentificationNumber;
                        row["UniqueIdentifier"] = x.TaxIdentificationNumber + "#" + x.BatchNumber + "#" + index++;
                        row["AdditionalDetails"] = x.AdditionalDetails;
                        row["Email"] = x.Email;
                        row["Amount"] = x.Amount;
                        row["BatchNumber"] = x.BatchNumber;
                        row["Status"] = x.Status;
                        row["StatusDetail"] = x.StatusDetail;
                        row["CreatedAtUtc"] = DateTime.Now.ToLocalTime();
                        row["UpdatedAtUtc"] = DateTime.Now.ToLocalTime();
                        dataTable.Rows.Add(row);
                    });

                    logger.Error(string.Format("Ref Data insertion has started Size: {0} Skip: {1} Time: {2}", dataSize, skip, startTime.Elapsed.ToString()));
                    IRepository<RefDataTemp> _refDataRepo = new Repository<RefDataTemp>(session);
                    bool result = _refDataRepo.SaveBundle(dataTable, tableName);
                    skip += chunkSize;
                    stopper++;
                    logger.Error("Counter " + ++counter);
                }
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message);
                //TODO
                return false;
            }
            startTime.Stop();
            logger.Error(string.Format("RUNTIME - {0} SIZE: {1}", startTime.Elapsed, dataSize));
            return true;
        }


        /// <summary>
        /// Get a joiner of the tax entity table and ref data temp table
        /// </summary>
        /// <param name="session"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        /// <returns>ProcessResponseModel</returns>
        protected ProcessResponseModel GetRefDataWithTheirCorrespondingCashflowCredentials(ISession session, string batchNumber, int revenueHeadId, int billingId)
        {
            ISchedulerLogger logger = SchedulerInterface.GetLoggerInstance();
            try
            {
                var queryString = string.Format(@"SELECT t.CashflowCustomerId as CashflowCustomerId, t.PrimaryContactId as CashflowPrimaryContactId, r.TaxIdentificationNumber as TaxIdentificationNumber, r.Recipient as Recipient, r.Address as Address, r.TaxEntityCategoryId as TaxEntityCategoryId, r.Email as Email, r.Id as RefDataTenpId, r.AdditionalDetails as AdditionalDetails, r.Amount as Amount FROM [{0}] as t RIGHT JOIN [{1}] as r ON t.TaxPayerIdentificationNumber = r.TaxIdentificationNumber WHERE r.BatchNumber = '{2}' AND r.RevenueHeadId = '{3}' AND r.BillingModelId = '{4}'", "Parkway_CBS_Core_TaxEntity", "Parkway_CBS_Core_RefDataTemp", batchNumber, revenueHeadId.ToString(), billingId.ToString());

                return new ProcessResponseModel { MethodReturnObject = session.CreateSQLQuery(queryString).SetResultTransformer(Transformers.AliasToBean<RefDataAndCashflowDetails>()).List<RefDataAndCashflowDetails>() };
            }
            catch (Exception exception)
            {
                logger.Error("Error getting joiner of tax entity and ref data temp " + exception.Message + exception.StackTrace);
                return new ProcessResponseModel { HasErrors = true, ErrorMessage = string.Format("Exception: {0} StackTrace: {1}", exception.Message, exception.StackTrace) };
            }
        }


        /// <summary>
        /// Update ref data temp
        /// </summary>
        /// <param name="session"></param>
        /// <param name="columnAndValue"></param>
        /// <param name="batchNumber"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="billingId"></param>
        protected void UpdateRefDataTemp(ISession session, string tableName, Dictionary<string, dynamic> columnAndValue, string batchNumber, int revenueHeadId, int billingId)
        {
            ISchedulerLogger logger = SchedulerInterface.GetLoggerInstance();
            //< id name = "MerchantId" column = "MERCHANT_ID" type = "string" >
            //< generator class="assigned" />
            //</id>
            StringBuilder baseQL = new StringBuilder("Update " + typeof(RefDataTemp) + " r SET ");
            //loop through set values
            int hasMoreItems = columnAndValue.Count;
            int paramSize = columnAndValue.Count;

            foreach (var item in columnAndValue)
            {
                if (hasMoreItems != 1) { baseQL.AppendFormat("r." + item.Key + " = :{0} , ", item.Key); hasMoreItems--; }
                else { baseQL.AppendFormat("r." + item.Key + " = :{0} ", item.Key); }
            }
            //add the where clause
            baseQL.AppendFormat("WHERE r.BatchNumber = '{0}' AND r.RevenueHeadId = '{1}' AND r.BillingModelId = '{2}'", batchNumber, revenueHeadId.ToString(), billingId.ToString());
            using (var sess = session.SessionFactory.OpenStatelessSession())
            {
                using (var tranx = sess.BeginTransaction())
                {
                    try
                    {
                        var query = sess.CreateQuery(baseQL.ToString());
                        query.SetTimeout(60000);
                        foreach (var item in columnAndValue) { query.SetParameter(item.Key, item.Value); }
                        query.ExecuteUpdate();
                        tranx.Commit();
                    }
                    catch (Exception exception)
                    {
                        logger.Error(exception.Message + exception.StackTrace);
                    }
                }
            }
        }
    }
}
