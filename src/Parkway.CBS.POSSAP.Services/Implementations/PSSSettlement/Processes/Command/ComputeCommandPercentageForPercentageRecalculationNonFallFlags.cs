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
    public class ComputeCommandPercentageForPercentageRecalculationNonFallFlags
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementPercentageRecalculationFeePartyBatchAggregateDAOManager _percentageRecalculationFeePartyBatchAggregate { get; set; }

        public IPSSSettlementBatchDAOManager _batchDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }


        private void SetPercentageRecalculationFeePartyBatchAggragetDAOManager()
        {
            if (_percentageRecalculationFeePartyBatchAggregate == null) { _percentageRecalculationFeePartyBatchAggregate = new PSSSettlementPercentageRecalculationFeePartyBatchAggregateDAOManager(UoW); }
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
        /// here we start the move records from the compute table to the batch items table
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        public void ComputeCommandPercentage(string tenantName, long batchId)
        {
            log.Info($"Computing command percentage for non fall flag percentage recalculation fee party batch aggregate records for batch Id {batchId}");
            SetUnitofWork(tenantName);
            SetSettlementBatchDAOManager();
            PSSSettlementBatchVM batchDetails = _batchDAOManager.GetBatchDetails(batchId);

            try
            {
                if (!ProcessComp.CheckBatchStatus((Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status, Police.Core.Models.Enums.PSSSettlementBatchStatus.ComputeCommandPercentageForPercentageRecalculationNonFallFlags))
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateSettlementBatchHasErrorAndErrorMessage(batchId, true, $"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.ComputeCommandPercentageForPercentageRecalculationNonFallFlags}");
                    UoW.Commit();
                    throw new Exception($"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.ComputeCommandPercentageForPercentageRecalculationNonFallFlags}");
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

                SetPercentageRecalculationFeePartyBatchAggragetDAOManager();
                UoW.BeginTransaction();
                _percentageRecalculationFeePartyBatchAggregate.ComputeCommandPercentageForNonFallFlags(batchId);
                _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.ComputeCommandPercentageForPercentageRecalculationFallFlags, "Computing Command Percentage For Percentage Recalculation Fee Party Batch Aggregate Fall Flags");
                UoW.Commit();
                //once we are done we need to queue the next job
                StartHangFireService();
                BackgroundJob.Enqueue(() => new ComputeCommandPercentageForPercentageRecalculationFallFlags().ComputeCommandPercentage(tenantName, batchId));
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
                _percentageRecalculationFeePartyBatchAggregate = null;
                _processCompo = null;
            }
        }
    }
}
