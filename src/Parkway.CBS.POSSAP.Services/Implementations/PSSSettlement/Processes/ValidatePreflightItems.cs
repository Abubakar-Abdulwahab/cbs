using System;
using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class ValidatePreflightItems
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementPreFlightItemsDAOManager _preFlightSettlementItems { get; set; }

        public IProcessComp _processCompo { get; set; }


        private void SetPreflightItemsDAOManager()
        {
            if (_preFlightSettlementItems == null) { _preFlightSettlementItems = new PSSSettlementPreFlightItemsDAOManager(UoW); }
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
        /// The goal here is to check the settlement batch table to see if we have any settlement for the give pre flight item details
        /// has already been queued for process.
        /// <para>What this means is that was take each pre flight item and we compare with what we have in the settlement batch, we compare the settlement Id and
        /// the start and end range. If for any items there is no match on the batch settlement table we then move the pre flight to the batch settlement for processing
        /// </para>
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="preflightBatchId"></param>
        public void DoValidation(string tenantName, long preflightBatchId)
        {
            log.Info($"About to start ValidatePreflightItems for pre flight batch Id {preflightBatchId}");

            try
            {
                SetUnitofWork(tenantName);
                SetPreflightItemsDAOManager();
                UoW.BeginTransaction();
                _preFlightSettlementItems.SetItemsForSettlementBatchInsertion(preflightBatchId);
                UoW.Commit();

                //once we are done we need to queue the next job
                StartHangFireService();
                BackgroundJob.Enqueue(() => new InsertBatchSettlements().MoveFromPreflightItemsToBatchSettlement(tenantName, preflightBatchId));
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
