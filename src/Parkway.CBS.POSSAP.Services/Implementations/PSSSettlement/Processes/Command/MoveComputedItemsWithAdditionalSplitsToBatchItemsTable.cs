using System;
using Hangfire;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Command
{
    public class MoveComputedItemsWithAdditionalSplitsToBatchItemsTable
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementBatchItemsDAOManager _batchItemsManager { get; set; }

        public IPSSSettlementBatchDAOManager _batchDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }

        public IPSSSettlementFeePartyDAOManager _settlementFeePartyDAOManager { get; set; }

        private void SetFeePartyRequestTransactionConfigurationDAOManager()
        {
            if (_batchItemsManager == null) { _batchItemsManager = new PSSSettlementBatchItemsDAOManager(UoW); }
        }


        private void SetSettlementBatchDAOManager()
        {
            if (_batchDAOManager == null) { _batchDAOManager = new PSSSettlementBatchDAOManager(UoW); }
        }

        private void SetSettlementFeePartyDAOManager()
        {
            if (_settlementFeePartyDAOManager == null) { _settlementFeePartyDAOManager = new PSSSettlementFeePartyDAOManager(UoW); }
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
        /// here we start the move records from the compute table to the batch items table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        public void MoveRecords(string tenantName, long batchId)
        {
            log.Info($"About to take the records in the split compute with additional splits for batch Id {batchId} and move them to batch items");
            SetUnitofWork(tenantName);
            SetSettlementBatchDAOManager();
            PSSSettlementBatchVM batchDetails = _batchDAOManager.GetBatchDetails(batchId);

            try
            {
                if (!ProcessComp.CheckBatchStatus((Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status, Police.Core.Models.Enums.PSSSettlementBatchStatus.MoveComputedItemsWithAdditionalSplitsToBatchItemsTable))
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateSettlementBatchHasErrorAndErrorMessage(batchId, true, $"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.MoveComputedItemsWithAdditionalSplitsToBatchItemsTable}");
                    UoW.Commit();
                    throw new Exception($"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.MoveComputedItemsWithAdditionalSplitsToBatchItemsTable}");
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP settlement");
                log.Error(exception.Message, exception);
                throw;
            }


            try
            {
                SetFeePartyRequestTransactionConfigurationDAOManager();
                SetSettlementFeePartyDAOManager();
                System.Collections.Generic.List<string> adapters = _settlementFeePartyDAOManager.GetActiveSettlementAdapters(batchDetails.PSSSettlementId);
                UoW.BeginTransaction();
                if (adapters.Count > 0)
                {
                    foreach (string adapter in adapters)
                    {
                        _batchItemsManager.MoveAdditionalSplitRecordsFromSplitComputeToBatchItemsForAdapterCommand(batchId, adapter);
                    }
                }
                _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.MarkTransactionsAsSettled, "Marking Transactions As Settled");
                UoW.Commit();

                //once we are done we need to queue the next job
                StartHangFireService();
                BackgroundJob.Enqueue(() => new MarkSettledTransaction().DoProcessing(tenantName, batchId, batchDetails.SettlementDate.Value));
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
                _batchDAOManager = null;
                _batchItemsManager = null;
                _processCompo = null;
            }
        }
    }
}
