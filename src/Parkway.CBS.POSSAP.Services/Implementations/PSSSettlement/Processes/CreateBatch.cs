using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class CreateBatch
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementDAOManager _pssSettlementDAOManager { get; set; }

        public IPSSSettlementPreFlightBatchDAOManager _settlementPreFlightDAOManager { get; set; }

        public IPSSSettlementPreFlightItemsDAOManager _preFlightSettlementItems { get; set; }

        public IPSSSettlementScheduleUpdateDAOManager _pssSettlementScheduleUpdateDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; } 


        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSSettlementJob");
            }
        }


        private void SetPreflightItemsDAOManager()
        {
            if(_preFlightSettlementItems == null) { _preFlightSettlementItems = new PSSSettlementPreFlightItemsDAOManager(UoW); }
        }

        private void SetSettlementPreFlightBatchDAOManager()
        {
            if (_settlementPreFlightDAOManager == null) { _settlementPreFlightDAOManager = new PSSSettlementPreFlightBatchDAOManager(UoW); }
        }

        private void SetPSSSettlementDAOManager()
        {
            if (_pssSettlementDAOManager == null) { _pssSettlementDAOManager = new PSSSettlementDAOManager(UoW); }
        }

        private void SetPSSSettlementScheduleDAOManager()
        {
            if (_pssSettlementScheduleUpdateDAOManager == null) { _pssSettlementScheduleUpdateDAOManager = new PSSSettlementScheduleUpdateDAOManager(UoW); }
        }


        /// <summary>
        /// Get process composition instance
        /// </summary>
        private void StartHangFireService()
        {
            if (_processCompo == null) { _processCompo = new ProcessComp(); }
            _processCompo.StartHangFireService();
        }


        /// <summary>
        /// Start settlement processing
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        public void BeginSettlementProcessing(string tenantName)
        {
            log.Info($"About to start POSSAP settlement");
            try
            {
                SetUnitofWork(tenantName);
                SetPSSSettlementDAOManager();
                //DateTime startDateRange = DateTime.Now.ToLocalTime().Date;
                //DateTime endDateRange = startDateRange.AddDays(1).AddMilliseconds(-1);
                ////get the count of settlements that would happen today
                //int recordCount = _pssSettlementDAOManager.IntCount(x => ((x.IsActive) && ((x.SettlementRule.NextScheduleDate >= startDateRange) && (x.SettlementRule.NextScheduleDate <= endDateRange))));

                //get next start date for settlements to run every hour using cron expression for every hour
                DateTime startDateRange = Util.GetNextDate("0 0 0/1 1/1 * ? *", DateTime.Now.AddHours(-1).ToLocalTime()).Value;
                log.Info($"Next Start Date - {startDateRange}");
                DateTime endDateRange = startDateRange.AddHours(1).AddMilliseconds(-1);
                log.Info($"Next End Date - {endDateRange}");
                //get the count of settlements that would happen this hour of today
                int recordCount = _pssSettlementDAOManager.IntCount(x => ((x.IsActive) && ((x.SettlementRule.NextScheduleDate >= startDateRange) && (x.SettlementRule.NextScheduleDate <= endDateRange))));

                log.Info($"About to start POSSAP settlements processing for settlement range {startDateRange} - {endDateRange}");
                if (recordCount < 1)
                {
                    log.Info("No active POSSAP settlement to be processed");
                    return;
                }

                string chunkSizeStringValue = ConfigurationManager.AppSettings["ChunkSize"];
                int chunkSize = 100;

                if (!string.IsNullOrEmpty(chunkSizeStringValue))
                {
                    int.TryParse(chunkSizeStringValue, out chunkSize);
                }
                //get the pages
                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;

                //lets create the pre flight batch
                SetSettlementPreFlightBatchDAOManager();
                //start transaction to create pre flight batch
                PSSSettlementPreFlightBatch preFlightBatch = new PSSSettlementPreFlightBatch { };
                UoW.BeginTransaction();
                _settlementPreFlightDAOManager.Add(preFlightBatch);
                UoW.Commit();

                IEnumerable<PSSSettlementRuleVM> activeSettlements = new List<PSSSettlementRuleVM>(chunkSize);
                List<PSSSettlementPreFlightItems> preFligtSettlementItems = new List<PSSSettlementPreFlightItems>(chunkSize);
                List<PSSSettlementScheduleUpdate> settlementScheduleUpdateItems = new List<PSSSettlementScheduleUpdate>(chunkSize);

                SetPreflightItemsDAOManager();
                SetPSSSettlementScheduleDAOManager();
                //lets try to 
                while (stopper < pages)
                {
                    //get settlments by chunksize
                    activeSettlements = _pssSettlementDAOManager.GetBatchActivePOSSAPSettlements(chunkSize, skip, startDateRange, endDateRange);

                    foreach (PSSSettlementRuleVM settlement in activeSettlements)
                    {
                        log.Info($"About to add pre flight settlement rule {settlement.Name} - {settlement.SettlementEngineRuleIdentifier} next schedule date. The next schedule date is {settlement.NextScheduleDate}. Settlement Rule PSS Settlement Id: {settlement.PSSSettlementId} {settlement.SettlemntRuleId}");

                        log.Info($"About to save pre flight settlement for rule {settlement.Name} - {settlement.SettlementEngineRuleIdentifier} for processing. Settlement Rule Id: {settlement.PSSSettlementId}");

                        preFligtSettlementItems.Add(BuildPreflightOBJ(preFlightBatch, settlement));
                        settlementScheduleUpdateItems.Add(BuildScheduleUpdate(preFlightBatch, settlement));
                        log.Info($"Pre flight settlement item {settlement.SettlementEngineRuleIdentifier} saved successfully. Settlement Rule PSS Settlement Id: {settlement.PSSSettlementId} {settlement.SettlemntRuleId} and Pre flight Id: {preFlightBatch.Id}");
                    }
                    //once we are done with that we have the batch, now we want to save the batch, then queue the job
                    if (!_preFlightSettlementItems.SaveItems(preFligtSettlementItems))
                    {
                        log.Info($"Error saving pre flight items");
                    }
                    //save the settlement schedule update items
                    if (!_pssSettlementScheduleUpdateDAOManager.SaveItems(settlementScheduleUpdateItems))
                    {
                        log.Info("Error saving settlement schedule update items");
                    }
                    activeSettlements = Enumerable.Empty<PSSSettlementRuleVM>();
                    preFligtSettlementItems.Clear();
                    skip += chunkSize;
                    stopper++;
                }
                log.Info($"POSSAP settlements has saved the preflight items with batch Id {preFlightBatch.Id}");
                //we need to schedule the next job
                //the next thing to do is to move the preflights to the  settlement batch table

                StartHangFireService();
                BackgroundJob.Enqueue(() => new ValidatePreflightItems().DoValidation(tenantName, preFlightBatch.Id));
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP settlement");
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                _pssSettlementDAOManager = null;
                _settlementPreFlightDAOManager = null;
                _preFlightSettlementItems = null;
                _processCompo = null;
            }
        }


        /// <summary>
        /// create pre flight item
        /// </summary>
        /// <param name="settlement"></param>
        /// <returns></returns>
        private PSSSettlementPreFlightItems BuildPreflightOBJ(PSSSettlementPreFlightBatch batch, PSSSettlementRuleVM settlement) => new PSSSettlementPreFlightItems
        {
            Batch = batch,
            PSSSettlement = new Police.Core.Models.PSSSettlement { Id = settlement.PSSSettlementId },
            StartRange = settlement.SettlementPeriodStartDate,
            EndRange = settlement.SettlementPeriodEndDate,
            AddToSettlementBatch = true,
            SettlementScheduleDate = settlement.NextScheduleDate
        };

        /// <summary>
        /// create settlement schedule update item
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="settlement"></param>
        /// <returns></returns>
        private PSSSettlementScheduleUpdate BuildScheduleUpdate(PSSSettlementPreFlightBatch batch, PSSSettlementRuleVM settlement)
        {
            DateTime NewScheduleDate = Util.GetNextDate(settlement.CronExpression, settlement.NextScheduleDate).Value;
            return new PSSSettlementScheduleUpdate
            {
                CurrentSchedule = settlement.NextScheduleDate,
                NextSchedule = NewScheduleDate,
                NextStartDate = NewScheduleDate.AddHours(-1),
                NextEndDate = NewScheduleDate.AddMilliseconds(-1),
                PSSSettlementPreFlightBatch = batch,
                PSSSettlement = new Police.Core.Models.PSSSettlement { Id = settlement.PSSSettlementId },
                SettlementRule = new Core.Models.SettlementRule { Id = settlement.SettlemntRuleId }
            };
        }
    }
}