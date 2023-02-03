using Hangfire;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class CheckIfBatchHasCommandSplits
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementBatchDAOManager _batchDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }


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
        /// Checks if batch with specified id has command splits and updates the status accordingly
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        public void CheckIfSettlementBatchHasCommandSplits(string tenantName, long batchId)
        {
            log.Info($"Checking if batch with Id {batchId} has command splits");

            try
            {
                SetUnitofWork(tenantName);
                SetSettlementBatchDAOManager();
                if(_batchDAOManager.Count(x => (x.Id == batchId) && x.HasCommandSplits) > 0)
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.MatchingCommandsToTransactions, "Matching Commands To Transactions");
                    UoW.Commit();
                    StartHangFireService();
                    BackgroundJob.Enqueue(() => new PairRequestTransactionConfigurationCommand().DoProcessing(tenantName, batchId));
                }
                else
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.AddingNonAdditionalSplitFeePartiesToBatchAggregate, "Adding Non Additional Split Fee Parties To Batch Aggregate");
                    UoW.Commit();
                    //once we are done we need to queue the next job
                    StartHangFireService();
                    BackgroundJob.Enqueue(() => new AddNonAdditionalSplitFeePartiesToBatchAggregate().AddAggregateRecords(tenantName, batchId));
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
                _batchDAOManager = null;
                _processCompo = null;
            }
        }
    }
}
