using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Configuration;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Newtonsoft.Json;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient;
using System.Net.Http;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement
{
    [DisableConcurrentExecution(10 * 60)]
    public class PSSSettlementService
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public ISettlementRuleDAOManager settlementRuleDAOManager { get; set; }

        public IPSSPresettlementDeductionConfigurationDAOManager presettlementDeductionConfigurationDAOManager { get; set; }

        public IPSSServiceSettlementConfigurationDAOManager serviceSettlementConfigurationDAOManager { get; set; }

        public IPSSSettlementBatchDAOManager settlementBatchDAOManager { get; set; }

        public IPSSSettlementBatchItemsDAOManager settlementBatchItemsDAOManager { get; set; }

        public IPoliceCollectionLogDAOManager policeCollectionLogDAOManager { get; set; }

        public IPSSSettlementBatchAggregateDAOManager settlementBatchAggregateDAOManager { get; set; }

        public IHangfireJobReferenceDAOManager hangfireJobReferenceDAOManager { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSSettlementJob");
            }
        }

        private void SetSettlementRuleDAOManager()
        {
            if (settlementRuleDAOManager == null) { settlementRuleDAOManager = new SettlementRuleDAOManager(UoW); }
        }

        private void SetPresettlementDeductionConfigurationDAOManager()
        {
            if (presettlementDeductionConfigurationDAOManager == null) { presettlementDeductionConfigurationDAOManager = new PSSPresettlementDeductionConfigurationDAOManager(UoW); }
        }

        private void SetServiceSettlementConfigurationDAOManager()
        {
            if (serviceSettlementConfigurationDAOManager == null) { serviceSettlementConfigurationDAOManager = new PSSServiceSettlementConfigurationDAOManager(UoW); }
        }

        private void SetSettlementBatchDAOManager()
        {
            if (settlementBatchDAOManager == null) { settlementBatchDAOManager = new PSSSettlementBatchDAOManager(UoW); }
        }

        private void SetSettlementBatchItemsDAOManager()
        {
            if (settlementBatchItemsDAOManager == null) { settlementBatchItemsDAOManager = new PSSSettlementBatchItemsDAOManager(UoW); }
        }

        private void SetPoliceCollectionLogDAOManager()
        {
            if (policeCollectionLogDAOManager == null) { policeCollectionLogDAOManager = new PoliceCollectionLogDAOManager(UoW); }
        }

        private void SetSettlementBatchAggregateDAOManager()
        {
            if (settlementBatchAggregateDAOManager == null) { settlementBatchAggregateDAOManager = new PSSSettlementBatchAggregateDAOManager(UoW); }
        }

        private void SetHangfireJobReferenceDAOManager()
        {
            if (hangfireJobReferenceDAOManager == null) { hangfireJobReferenceDAOManager = new HangfireJobReferenceDAOManager(UoW); }
        }


        /// <summary>
        /// Start settlement processing
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public string BeginSettlementProcessing(string tenantName)
        {
            log.Info($"About to start POSSAP settlement");
            try
            {
                SetUnitofWork(tenantName);
                SetSettlementRuleDAOManager();
                SetHangfireJobReferenceDAOManager();

                DateTime today = DateTime.Now.ToLocalTime().Date;// new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 0, 0);
                Int64 recordCount = settlementRuleDAOManager.Count(x => ((x.IsActive) && ((today <= x.NextScheduleDate) && (x.NextScheduleDate <= today.AddDays(1).AddMilliseconds(-1)))));
                log.Info($"About to start POSSAP settlements processing");
                if (recordCount < 1) return "No active POSSAP settlement to be processed";

                var chunkSizeStringValue = ConfigurationManager.AppSettings["ChunkSize"];
                int chunkSize = 100;

                if (!string.IsNullOrEmpty(chunkSizeStringValue))
                {
                    int.TryParse(chunkSizeStringValue, out chunkSize);
                }

                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;
                StartHangfireServer();
                List<PSSSettlementRuleVM> activeSettlements = null;
                while (stopper < pages)
                {
                    activeSettlements = null;// settlementRuleDAOManager.GetBatchActivePOSSAPSettlements(chunkSize, skip, today);
                    foreach (PSSSettlementRuleVM settlement in activeSettlements)
                    {
                        log.Info($"About to check settlement rule {settlement.SettlementEngineRuleIdentifier} next schedule date. The next schedule date is {settlement.NextScheduleDate}. Settlement Rule Id: {settlement.SettlemntRuleId}");

                        log.Info($"About to queue settlement rule {settlement.SettlementEngineRuleIdentifier} for processing. Settlement Rule Id: {settlement.SettlemntRuleId}");
                        string scheduleJobId = BackgroundJob.Schedule(() => StartBatchServiceProcessing(tenantName, settlement), settlement.NextScheduleDate);
                        SaveHangfireJobReferenceDetail(scheduleJobId, settlement.SettlementEngineRuleIdentifier);
                        log.Info($"Settlement rule {settlement.SettlementEngineRuleIdentifier} queued successfully. Settlement Rule Id: {settlement.SettlemntRuleId}");
                    }
                    activeSettlements.Clear();
                    skip += chunkSize;
                    stopper++;
                }

                log.Info($"POSSAP settlements queued job done!!!");
                return "POSSAP settlements queued job done!!!";
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP settlement");
                log.Error(exception.Message, exception);
                throw;
            }
        }

        /// <summary>
        /// Start batch processing of the setttlement per configured settlement rule
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public string StartBatchServiceProcessing(string tenantName, PSSSettlementRuleVM settlementRuleVM)
        {
            log.Info($"About to start settlement processing for {settlementRuleVM.SettlementEngineRuleIdentifier}");
            try
            {
                //SetUnitofWork(tenantName);
                //SetPresettlementDeductionConfigurationDAOManager();
                //SetServiceSettlementConfigurationDAOManager();
                //SetSettlementBatchDAOManager();

                //List<PSSServiceSettlementConfigurationVM> settlementConfigurations = serviceSettlementConfigurationDAOManager.GetSettlementRuleConfigurations(settlementRuleVM.SettlemntRuleId);

                //StartHangfireServer();
                //foreach (PSSServiceSettlementConfigurationVM settlementConfiguration in settlementConfigurations)
                //{
                //    PSSPresettlementDeductionConfigurationVM presettlementDeduction = presettlementDeductionConfigurationDAOManager.GetPresettlementDeductionConfiguration(settlementConfiguration.SettlemntRuleId, settlementConfiguration.RevenueHeadId, settlementConfiguration.ServiceId, settlementConfiguration.PaymentProviderId, settlementConfiguration.DefinitionLevelId);
                //    if (presettlementDeduction != null)
                //    {
                //        log.Info($"About to process settlement with cost of service configured. Params:::Rule Code => {settlementRuleVM.SettlementEngineRuleIdentifier}, MDA Id => {settlementConfiguration.MDAId}, Revenue Head Id => {settlementConfiguration.RevenueHeadId}, Provider => {settlementConfiguration.PaymentProviderId}, Channel => {settlementConfiguration.Channel}, Service Id => {settlementConfiguration.ServiceId}, Definition LevelId => {settlementConfiguration.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");
                //        int settlementBatchId = settlementBatchDAOManager.SaveBatch(settlementRuleVM);
                //        ProcessSettlementWithDeductions(tenantName, settlementConfiguration, presettlementDeduction, settlementRuleVM, settlementBatchId);
                //    }
                //    else
                //    {
                //        log.Info($"About to process settlement without cost of service. Params::: Rule Code => {settlementRuleVM.SettlementEngineRuleIdentifier}, MDA Id => {settlementConfiguration.MDAId}, Revenue Head Id => {settlementConfiguration.RevenueHeadId}, Provider => {settlementConfiguration.PaymentProviderId}, Channel => {settlementConfiguration.Channel}, Service Id => {settlementConfiguration.ServiceId}, Definition LevelId => {settlementConfiguration.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");
                //        int settlementBatchId = settlementBatchDAOManager.SaveBatch(settlementRuleVM);
                //        ProcessSettlementWithoutDeductions(tenantName, settlementConfiguration, settlementRuleVM, settlementBatchId);
                //    }
                //}

                //log.Info($"Completed settlement processing for {settlementRuleVM.SettlementEngineRuleIdentifier}");
                return $"Completed settlement processing for {settlementRuleVM.SettlementEngineRuleIdentifier}!!!";
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP settlement");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
        }

        private void ProcessSettlementWithDeductions(string tenantName, PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSPresettlementDeductionConfigurationVM pssPresettlementDeduction, PSSSettlementRuleVM settlementRuleVM, int settlementBatchId)
        {
            try
            {
                SetUnitofWork(tenantName);
                SetPoliceCollectionLogDAOManager();
                SetSettlementBatchItemsDAOManager();
                SetSettlementRuleDAOManager();
                SetHangfireJobReferenceDAOManager();

                long recordCount = policeCollectionLogDAOManager.GetCollectionLogCount(pssServiceSettlement, settlementRuleVM).FirstOrDefault().TotalRecordCount;
                log.Info($"Settlement with deduction count {recordCount} for configured params:::Rule Code => {settlementRuleVM.SettlementEngineRuleIdentifier}, MDA Id => {pssServiceSettlement.MDAId}, Revenue Head Id => {pssServiceSettlement.RevenueHeadId}, Provider => {pssServiceSettlement.PaymentProviderId}, Channel => {pssServiceSettlement.Channel}, Service Id => {pssServiceSettlement.ServiceId}, Definition LevelId => {pssServiceSettlement.DefinitionLevelId}, Schudule Date => {settlementRuleVM.NextScheduleDate.ToString("yyyy-MM-dd")}");

                if (recordCount < 1) return;

                var chunkSizeStringValue = ConfigurationManager.AppSettings["ChunkSize"];
                int chunkSize = 100;

                if (!string.IsNullOrEmpty(chunkSizeStringValue))
                {
                    bool parsed = int.TryParse(chunkSizeStringValue, out chunkSize);
                }

                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;
                UoW.BeginTransaction();
                List<PoliceCollectionLogVM> collectionLogs = null;
                while (stopper < pages)
                {
                    collectionLogs = policeCollectionLogDAOManager.GetPagedCollectionLogs(pssServiceSettlement, settlementRuleVM, chunkSize, skip);

                    ConcurrentQueue<PoliceCollectionLogVM> collectionLogVMs = policeCollectionLogDAOManager.ComputeCostofService(pssPresettlementDeduction, collectionLogs);

                    //Save records to the db
                    settlementBatchItemsDAOManager.SaveRecords(pssServiceSettlement, settlementRuleVM, settlementBatchId, collectionLogVMs, chunkSize);
                    collectionLogs.Clear();
                    skip += chunkSize;
                    stopper++;
                }

                //Set next run date
                settlementRuleDAOManager.UpdateNextScheduleDate(GetNextScheduleDate(settlementRuleVM.CronExpression, settlementRuleVM.NextScheduleDate), settlementRuleVM.SettlemntRuleId);
                UoW.Commit();

                StartHangfireServer();
                string scheduleJobId = BackgroundJob.Enqueue(() => SendSettlementToSettlementEngine(tenantName, pssServiceSettlement, settlementRuleVM, settlementBatchId));
                SaveHangfireJobReferenceDetail(scheduleJobId, $"{settlementRuleVM.SettlementEngineRuleIdentifier}-psssetconfigid-{pssServiceSettlement.SettlemntConfigurationId}");
            }
            catch (Exception exception)
            {
                log.Error($"Error while moving record to settlement batch items table and calling settlement engine for {settlementRuleVM.SettlementEngineRuleIdentifier} and settlement configuration Id {pssServiceSettlement.SettlemntConfigurationId}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
        }

        private void ProcessSettlementWithoutDeductions(string tenantName, PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int settlementBatchId)
        {
            try
            {
                log.Info($"Start moving settlement items into batch items table for {settlementRuleVM.SettlementEngineRuleIdentifier}. Settlement Configuration Id {pssServiceSettlement.SettlemntConfigurationId}");
                SetUnitofWork(tenantName);
                SetSettlementBatchItemsDAOManager();
                SetSettlementRuleDAOManager();
                SetHangfireJobReferenceDAOManager();

                UoW.BeginTransaction();
                //Move the items to be settled for this batch from PoliceCollectionLog to PSSSettlementBatchItem
                settlementBatchItemsDAOManager.MoveRecordFromPoliceCollectionLogToSettlementBatchItems(pssServiceSettlement, settlementRuleVM, settlementBatchId);

                //Set next run date
                settlementRuleDAOManager.UpdateNextScheduleDate(GetNextScheduleDate(settlementRuleVM.CronExpression, settlementRuleVM.NextScheduleDate), settlementRuleVM.SettlemntRuleId);
                UoW.Commit();

                StartHangfireServer();
                string scheduleJobId = BackgroundJob.Enqueue(() => SendSettlementToSettlementEngine(tenantName, pssServiceSettlement, settlementRuleVM, settlementBatchId));
                SaveHangfireJobReferenceDetail(scheduleJobId, $"{settlementRuleVM.SettlementEngineRuleIdentifier}-psssetconfigid-{pssServiceSettlement.SettlemntConfigurationId}");
            }
            catch (Exception exception)
            {
                log.Error($"Error while moving record to settlement batch items table and calling settlement engine for {settlementRuleVM.SettlementEngineRuleIdentifier} and settlement configuration Id {pssServiceSettlement.SettlemntConfigurationId}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
        }

        public void SendSettlementToSettlementEngine(string tenantName, PSSServiceSettlementConfigurationVM pssServiceSettlement, PSSSettlementRuleVM settlementRuleVM, int settlementBatchId)
        {
            try
            {
                log.Info($"About to send request to settlement engine for {settlementRuleVM.SettlementEngineRuleIdentifier}. Settlement Batch Id {settlementBatchId}");
                SetUnitofWork(tenantName);
                SetSettlementBatchItemsDAOManager();

                var clientCode = ConfigurationManager.AppSettings["POSSAPSettlementClientCode"];
                var secret = ConfigurationManager.AppSettings["POSSAPSettlementSecret"];
                var settlementBaseURL = ConfigurationManager.AppSettings["SettlementEngineBaseURL"];

                if (string.IsNullOrEmpty(clientCode) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(settlementBaseURL))
                {
                    throw new Exception("Settlement details was not found");
                }

                //Get Aggregate amount
                dynamic aggregateVal = settlementBatchItemsDAOManager.GetBatchAggregateAmount(pssServiceSettlement, settlementBatchId);

                ComputeRuleRequestModel sttlmtmodel = new ComputeRuleRequestModel
                {
                    Amount = aggregateVal.TotalAmount,
                    NumberOfTransactions = aggregateVal.TotalCount,
                    Narration = settlementRuleVM.NextScheduleDate.ToString("dd/MM/yyyy") + " " + settlementRuleVM.SettlementEngineRuleIdentifier + " " + ((PaymentChannel)pssServiceSettlement.Channel).ToDescription() + " PAYMENT SETTLEMENT ",
                    RuleCode = settlementRuleVM.SettlementEngineRuleIdentifier,
                    SettlementDate = DateTime.Now.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                    ReferenceNumber = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", pssServiceSettlement.PaymentProviderName.Replace(" ","-"), ((PaymentChannel)pssServiceSettlement.Channel).ToDescription().Replace(" ", "-"), settlementRuleVM.NextScheduleDate.ToString("dd/MM/yyyy"), settlementRuleVM.SettlementEngineRuleIdentifier, pssServiceSettlement.MDAId, pssServiceSettlement.RevenueHeadId, settlementBatchId)
                };

                IClient _remoteClient = new Client();
                SettlementEngineAuthVM authRequest = new SettlementEngineAuthVM { ClientCode = clientCode, hmac = Util.HMACHash256(clientCode, secret) };
                string authRequestModel = JsonConvert.SerializeObject(authRequest);
                string stoken = _remoteClient.SendRequest(authRequestModel, $"{settlementBaseURL}/auth/gettoken", HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { });
                SettlementEngineAuthResponseVM authtoken = JsonConvert.DeserializeObject<SettlementEngineAuthResponseVM>(stoken);

                string ssttlmtmodel = JsonConvert.SerializeObject(sttlmtmodel);
                string response = _remoteClient.SendRequest(ssttlmtmodel, $"{settlementBaseURL}/rule/compute", HttpMethod.Post, new Dictionary<string, string> { }, new Dictionary<string, string> { { "Bearer", authtoken.token } });
                SettlementEngineResponse responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(response);

                SetSettlementBatchAggregateDAOManager();
                UoW.BeginTransaction();
                settlementBatchAggregateDAOManager.Add(new PSSSettlementBatchAggregate
                {
                    SettlementBatch = new PSSSettlementBatch { Id = settlementBatchId },
                    Amount = aggregateVal.TotalAmount,
                    Error = responseModel.Error,
                    TimeFired = DateTime.Now,
                    SettlementEngineResponseJSON = response,
                    RequestReference = sttlmtmodel.ReferenceNumber,
                    RetryCount = 1,
                    TransactionCount = aggregateVal.TotalCount,
                });
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error($"Error while sending request to settlement engine for {settlementRuleVM.SettlementEngineRuleIdentifier}. Settlement Batch Id {settlementBatchId}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw;
            }
        }

        private void StartHangfireServer()
        {
            var conStringName = ConfigurationManager.AppSettings["HangfireConnectionStringName"];

            if (string.IsNullOrEmpty(conStringName))
            {
                throw new Exception("Unable to get the hangfire connection string name");
            }

            //Get the connection string
            string dbConnectionString = HangFireScheduler.GetConnectionString(conStringName);

            //Call the Hangfire storage
            GlobalConfiguration.Configuration.UseSqlServerStorage(dbConnectionString);
        }

        /// <summary>
        /// Get next running date for the job using the cron expression and the current running date
        /// </summary>
        /// <param name="cronExpression"></param>
        /// <param name="currentRunningDate"></param>
        /// <returns></returns>
        private DateTime GetNextScheduleDate(string cronExpression, DateTime currentRunningDate)
        {
            try
            {
                return (DateTime)Util.GetNextDate(cronExpression, currentRunningDate);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SaveHangfireJobReferenceDetail(string hangfireJobId, string jobReferenceNumber)
        {
            try
            {
                UoW.BeginTransaction();
                hangfireJobReferenceDAOManager.Add(new HangfireJobReference { HangfireJobId = hangfireJobId, JobReferenceNumber = jobReferenceNumber });
                UoW.Commit();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Cannot save hangfire job reference details for reference number {0} and job id {1}", jobReferenceNumber, hangfireJobId), ex);
            }
        }
    }
}
