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
    public class MatchCommandFeePartyRequestTransactionForCommandSplit
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementFeePartyRequestTransactionCommandSplitComputeDAOManager _feePartyCommandComputeManager { get; set; }

        public IPSSSettlementBatchDAOManager _batchDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }

        public IPSSSettlementFeePartyDAOManager _settlementFeePartyDAOManager { get; set; }

        private void SetFeePartyRequestTransactionConfigurationDAOManager()
        {
            if (_feePartyCommandComputeManager == null) { _feePartyCommandComputeManager = new PSSSettlementFeePartyRequestTransactionCommandSplitComputeDAOManager(UoW); }
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
        /// here we start the batch processing for the settlement
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        public void DoMatchingOfCommandFeePartiesTransactionsAndCommands(string tenantName, long batchId)
        {
            log.Info($"About to take the records in the config and pair with the transactions for batch Id {batchId}");
            SetUnitofWork(tenantName);
            SetSettlementBatchDAOManager();
            PSSSettlementBatchVM batchDetails = _batchDAOManager.GetBatchDetails(batchId);

            try
            {
                if (!ProcessComp.CheckBatchStatus((Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status, Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandFeePartiesWithTransactionsAndCommands))
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateSettlementBatchHasErrorAndErrorMessage(batchId, true, $"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandFeePartiesWithTransactionsAndCommands}");
                    UoW.Commit();
                    throw new Exception($"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandFeePartiesWithTransactionsAndCommands}");
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
                if(adapters.Count > 0)
                {
                    foreach(string adapter in adapters)
                    {
                        _feePartyCommandComputeManager.ForAdapterCommandSplitsCombineFeePartyRequestTransactionWithCommands(batchId, adapter);
                    }
                }
                _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.SettingFallFlagForFeePartyCommandCombination, "Setting Fall Flag For Fee Party Command Combinations");
                UoW.Commit();

                //once we are done we need to queue the next job
                StartHangFireService();
                BackgroundJob.Enqueue(() => new SetFallFlagForFeePartyCommandCombination().SetFallFlag(tenantName, batchId));
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
                _feePartyCommandComputeManager = null;
                _processCompo = null;
            }
        }

    }
}