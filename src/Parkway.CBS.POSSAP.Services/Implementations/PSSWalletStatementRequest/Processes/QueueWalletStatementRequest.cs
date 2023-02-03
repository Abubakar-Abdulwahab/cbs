using System;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes.Contracts;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.Core.Utilities;
using Hangfire;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes
{
    public class QueueWalletStatementRequest
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public ICommandWalletDetailsDAOManager _commandWalletDetailsDAOManager { get; set; }

        public IPSSFeePartyDAOManager _pssFeePartyDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSWalletStatementRequestJob");
            }
        }

        private void SetCommandWalletDetailsDAOManager()
        {
            if (_commandWalletDetailsDAOManager == null) { _commandWalletDetailsDAOManager = new CommandWalletDetailsDAOManager(UoW); }
        }


        private void SetPSSFeePartyDAOManager()
        {
            if (_pssFeePartyDAOManager == null) { _pssFeePartyDAOManager = new PSSFeePartyDAOManager(UoW); }
        }

        /// <summary>
        /// Get process composition instance
        /// </summary>
        private void StartHangFireService()
        {
            if (_processCompo == null) { _processCompo = new ProcessComp(); }
            _processCompo.StartHangFireService();
        }


        public void EnqueueWalletStatementRequest(string tenantName, WalletStatementScheduleVM schedule)
        {
            log.Info($"Queuing wallet statement requests in batches");
            try
            {
                SetUnitofWork(tenantName);
                SetCommandWalletDetailsDAOManager();
                SetPSSFeePartyDAOManager();
                StartHangFireService();
                EnqueueCommandWalletStatementRequest(tenantName, schedule);
                EnqueueFeePartyWalletStatementRequest(tenantName, schedule);
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP wallet statement request");
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                _commandWalletDetailsDAOManager = null;
                _pssFeePartyDAOManager = null;
                _processCompo = null;
            }
        }


        private void EnqueueCommandWalletStatementRequest(string tenantName, WalletStatementScheduleVM schedule)
        {
            try
            {
                Int64 recordCount = _commandWalletDetailsDAOManager.Count(x => x.IsActive);
                if (recordCount < 1) { log.Info("No command wallets found."); return; }

                int chunkSize = 10;

                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;

                while (stopper < pages)
                {
                    log.Info($"Queuing wallet statement requests for commands. Skipped:{skip} Taken:{chunkSize}");
                    //Queue job that gets chunk of wallets and fetches their statements, this job would accept the skip, chunkSize and schedule as parameters
                    BackgroundJob.Enqueue(() => new ProcessWalletStatementRequest().GetWalletStatement(tenantName, skip, chunkSize, schedule, Police.Core.Models.Enums.WalletIdentifierType.CommandWalletDetails));
                    log.Info($"Queued successfully");
                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception)
            {
                log.Error("Error processing command wallet statement request");
                throw;
            }
        }


        private void EnqueueFeePartyWalletStatementRequest(string tenantName, WalletStatementScheduleVM schedule)
        {
            try
            {
                Int64 recordCount = _pssFeePartyDAOManager.Count(x => x.IsActive);
                if (recordCount < 1) { log.Info("No fee party found."); return; }

                int chunkSize = 10;

                int pages = Util.Pages(chunkSize, recordCount);
                int stopper = 0;
                int skip = 0;

                while (stopper < pages)
                {
                    log.Info($"Queuing wallet statement requests for fee parties. Skipped:{skip} Taken:{chunkSize}");
                    //Queue job that gets chunk of wallets and fetches their statements, this job would accept the skip, chunkSize and schedule as parameters
                    BackgroundJob.Enqueue(() => new ProcessWalletStatementRequest().GetWalletStatement(tenantName, skip, chunkSize, schedule, Police.Core.Models.Enums.WalletIdentifierType.PSSFeeParty));
                    log.Info($"Queued successfully");
                    skip += chunkSize;
                    stopper++;
                }
            }
            catch (Exception)
            {
                log.Error("Error processing fee party wallet statement request");
                throw;
            }
        }
    }
}
