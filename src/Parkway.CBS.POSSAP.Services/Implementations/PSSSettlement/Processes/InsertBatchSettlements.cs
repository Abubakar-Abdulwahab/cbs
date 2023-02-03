using System;
using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;
using Parkway.CBS.Police.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Linq;
using Parkway.CBS.Core.Utilities;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class InsertBatchSettlements
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementPreFlightItemsDAOManager _preFlightSettlementItems { get; set; }

        public IPSSSettlementScheduleUpdateDAOManager _pssSettlementScheduleUpdateDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }


        private void SetPreflightItemsDAOManager()
        {
            if (_preFlightSettlementItems == null) { _preFlightSettlementItems = new PSSSettlementPreFlightItemsDAOManager(UoW); }
        }

        private void SetPSSSettlementScheduleUpdateDAOManager()
        {
            if (_pssSettlementScheduleUpdateDAOManager == null) { _pssSettlementScheduleUpdateDAOManager = new PSSSettlementScheduleUpdateDAOManager(UoW); }
        }


        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSSettlementJob");
            }
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
        /// Move items from the pre flight items table to the batch settlement table for 
        /// items that have the AddToSettlementBatch set to true
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="preflightBatchId"></param>
        public void MoveFromPreflightItemsToBatchSettlement(string tenantName, long preflightBatchId)
        {
            log.Info($"About to insert into the settlement batch table items from the pre flight table where AddToSettlementBatch is true for pre flight batch Id {preflightBatchId}");

            try
            {
                SetUnitofWork(tenantName);
                SetPreflightItemsDAOManager();
                SetPSSSettlementScheduleUpdateDAOManager();
                UoW.BeginTransaction();
                _preFlightSettlementItems.InsertItemsForSettlementIntoSettlementBatchTable(preflightBatchId);
                _pssSettlementScheduleUpdateDAOManager.UpdateSettlementScheduleDate(preflightBatchId);
                UoW.Commit();
                //once we are done we need to queue the next job
                StartHangFireService();
                BackgroundJob.Enqueue(() => new BeginBatchSettlementQueueing().DoQueueing(tenantName));
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
                _preFlightSettlementItems = null;
                _processCompo = null;
            }
        }
    }
}