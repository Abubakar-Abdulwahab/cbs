using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Command;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class CheckIfAnyFeePartyInBatchHasAdditionalSplits
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IFeePartyRequestTransactionDAOManager _feePartyPairManager { get; set; }

        public IPSSSettlementBatchDAOManager _batchDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }


        private void SetFeePartyRequestTransactionConfigurationDAOManager()
        {
            if (_feePartyPairManager == null) { _feePartyPairManager = new FeePartyRequestTransactionDAOManager(UoW); }
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
        /// Checks if any fee party in the batch with specified id has additional splits
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        public void CheckIfFeePartyInBatchHasAdditionalSplits(string tenantName, long batchId)
        {
            log.Info($"Checking to see if any fee party in batch with Id {batchId} has additional splits");
            SetUnitofWork(tenantName);
            SetSettlementBatchDAOManager();
            PSSSettlementBatchVM batchDetails = _batchDAOManager.GetBatchDetails(batchId);

            try
            {
                if (!ProcessComp.CheckBatchStatus((Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status, Police.Core.Models.Enums.PSSSettlementBatchStatus.CheckingIfFeePartyInBatchHasAdditionalSplits))
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateSettlementBatchHasErrorAndErrorMessage(batchId, true, $"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.CheckingIfFeePartyInBatchHasAdditionalSplits}");
                    UoW.Commit();
                    throw new Exception($"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.CheckingIfFeePartyInBatchHasAdditionalSplits}");
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

                if (_feePartyPairManager.Count(x => (x.Batch.Id == batchId) && x.HasAdditionalSplit) > 0)
                {
                    //update status to calculating with commands
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandFeePartiesWithTransactionsAndCommands, "Matching Command Fee Parties With Transactions And Commands");
                    UoW.Commit();
                    StartHangFireService();
                    BackgroundJob.Enqueue(() => new MatchCommandFeePartyRequestTransactionForCommandSplit().DoMatchingOfCommandFeePartiesTransactionsAndCommands(tenantName, batchId));
                }
                else
                {
                    //update status to ready for final processing
                    UoW.BeginTransaction();
                    //_batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.FinalProcessing, "Final Processing");
                    _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.SettingFallFlagForFeePartyCommandCombination, "Setting Fall Flag For Fee Party Command Combinations");
                    UoW.Commit();
                    //StartHangFireService();
                    //BackgroundJob.Enqueue(() => new SendSettlementFeePartyBatchAggregate().SendFeePartyBatchAggregate(tenantName, batchId));
                    BackgroundJob.Enqueue(() => new SetFallFlagForFeePartyCommandCombination().SetFallFlag(tenantName, batchId));
                }

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
                _feePartyPairManager = null;
                _processCompo = null;
            }
        }
    }
}
