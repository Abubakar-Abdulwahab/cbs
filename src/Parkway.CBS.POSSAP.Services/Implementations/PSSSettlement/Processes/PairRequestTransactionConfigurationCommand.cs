using System;
using Hangfire;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class PairRequestTransactionConfigurationCommand
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSServiceSettlementConfigTranxCommandDAOManager _pssTransactionCommand { get; set; }

        public IPSSSettlementBatchDAOManager _batchDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }


        private void SetTransactionCommandDAOManager()
        {
            if (_pssTransactionCommand == null) { _pssTransactionCommand = new PSSServiceSettlementConfigTranxCommandDAOManager(UoW); }
        }


        private void SetSettlementBatchDAOManager()
        {
            if (_batchDAOManager == null) { _batchDAOManager = new PSSSettlementBatchDAOManager(UoW); }
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
        public void DoProcessing(string tenantName, long batchId)
        {
            log.Info($"About to take the records in the config and pair with the transactions for batch Id {batchId}");
            SetUnitofWork(tenantName);
            SetSettlementBatchDAOManager();
            PSSSettlementBatchVM batchDetails = _batchDAOManager.GetBatchDetails(batchId);

            try
            {
                if (!ProcessComp.CheckBatchStatus((Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status, Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandsToTransactions))
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateSettlementBatchHasErrorAndErrorMessage(batchId, true, $"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandsToTransactions}");
                    UoW.Commit();
                    throw new Exception($"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandsToTransactions}");
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
                SetTransactionCommandDAOManager();
                UoW.BeginTransaction();
                _pssTransactionCommand.PairTransactionWithCommand(batchId);
                _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingFeePartiesWithRequestCommands, "Matching Fee Parties With Request Commands");
                UoW.Commit();
                //once we are done we need to queue the next job
                StartHangFireService();
                BackgroundJob.Enqueue(() => new MatchFeePartRequestTransactionForNonAdditionalSplitToCommands().DoMatchingOfFeePartiesTransactionsAndCommands(tenantName, batchId));
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
                _pssTransactionCommand = null;
                _processCompo = null;
            }
        }

    }
}