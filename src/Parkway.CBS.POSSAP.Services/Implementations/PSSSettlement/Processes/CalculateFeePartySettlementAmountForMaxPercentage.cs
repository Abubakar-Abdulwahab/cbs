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
    public class CalculateFeePartySettlementAmountForMaxPercentage
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
        /// here we start the batch processing for the settlement
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        public void CalculateMaxPercentageSettlementAmount(string tenantName, long batchId)
        {
            log.Info($"About to take the records in the config and pair with the transactions for batch Id {batchId}");

            try
            {
                SetUnitofWork(tenantName);
                SetSettlementBatchDAOManager();
                PSSSettlementBatchVM batchDetails = _batchDAOManager.GetBatchDetails(batchId);

                SetFeePartyRequestTransactionConfigurationDAOManager();
                UoW.BeginTransaction();
                _feePartyPairManager.DoCalculationForMaxPercentage(batchId, batchDetails.ServiceId);
                _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.MaxSettlementPercentageCalculated, "Calculcated maximum percentage of settlement");
                UoW.Commit();
                //once we are done we need to queue the next job
                StartHangFireService();
                BackgroundJob.Enqueue(() => new CheckIfBatchHasCommandSplits().CheckIfSettlementBatchHasCommandSplits(tenantName, batchId));
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
                _feePartyPairManager = null;
                _processCompo = null;
            }
        }

    }
}
