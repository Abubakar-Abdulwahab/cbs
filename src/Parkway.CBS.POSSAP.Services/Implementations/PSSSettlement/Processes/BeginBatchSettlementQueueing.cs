using System;
using Hangfire;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;


namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class BeginBatchSettlementQueueing
    {

        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementBatchDAOManager _pSSSettlementBatchManager;

        public IPSSHangfireSettlementReferenceDAOManager _pSSHangfireSettlementReferenceManager;

        public IProcessComp _processCompo { get; set; }


        private void SetSettlementBatchDAOManager()
        {
            if (_pSSSettlementBatchManager == null) { _pSSSettlementBatchManager = new PSSSettlementBatchDAOManager(UoW); }
        }


        private void SetHangfireSettlementReferenceManager()
        {
            if(_pSSHangfireSettlementReferenceManager == null) { _pSSHangfireSettlementReferenceManager = new PSSHangfireSettlementReferenceDAOManager(UoW); }
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
        /// Here we queue all the settlement in the batch Item table
        /// </summary>
        /// <param name="tenantName"></param>
        public void DoQueueing(string tenantName)
        {
            log.Info($"About to start Begin Batch Settlement Queueing for {tenantName}");

            try
            {
                SetUnitofWork(tenantName);
                SetSettlementBatchDAOManager();
                UoW.BeginTransaction();
                _pSSSettlementBatchManager.SetSettlementBatchStatusToReadyForScheduling();
                UoW.Commit();
                //once we are done with the update we can now start queuing 
                bool hasMoreRecords = true;
                string chunkSizeStringValue = ConfigurationManager.AppSettings["ChunkSize"];
                int chunkSize = 100;

                if (!string.IsNullOrEmpty(chunkSizeStringValue))
                {
                    int.TryParse(chunkSizeStringValue, out chunkSize);
                }

                //get the pages
                int skip = 0;
                IEnumerable<PSSSettlementBatchVM> items = new List<PSSSettlementBatchVM>(chunkSize);
                IList<PSSHangfireSettlementReference> hangfireRefs = new List<PSSHangfireSettlementReference>(chunkSize);

                SetHangfireSettlementReferenceManager();

                //StartHangFireService();

                while (hasMoreRecords)
                {
                    items = _pSSSettlementBatchManager.GetBatch(chunkSize, skip);
                    if(items.Count() < 1) { hasMoreRecords = false; break; }
                    foreach (var item in items)
                    {
                        hangfireRefs.Add(new PSSHangfireSettlementReference
                        {
                            HangfireJobId = ScheduleItem(tenantName, item),
                            ReferenceId = item.Id,
                            ReferenceType = (int) HangfireReferenceType.SettlementBatch,
                        });
                    }
                    //save bunch
                    _pSSHangfireSettlementReferenceManager.SaveBundle(hangfireRefs);
                    skip += chunkSize;
                    hangfireRefs.Clear();
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error processing Begin Batch Settlement Queueing DoQueueing settlement");
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                _pSSSettlementBatchManager = null;
                _pSSHangfireSettlementReferenceManager = null;
                _processCompo = null;
            }
        }


        private string ScheduleItem(string tenantName, PSSSettlementBatchVM batchObj) => BackgroundJob.Schedule(() => new StartBatchServiceProcessing().DoProcessing(tenantName, batchObj.Id), batchObj.ScheduleDate);



    }
}