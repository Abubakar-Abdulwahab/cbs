using System;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes.Contracts;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Core.Utilities;
using Hangfire;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes
{
    //This process would get the schedule, log schedule id along with start and end date then update the schedule
    public class ScheduleWalletStatementRequest
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IWalletStatementScheduleUpdateDAOManager _walletStatementScheduleUpdateDAOManager { get; set; }

        public IWalletStatementScheduleDAOManager _walletStatementScheduleDAOManager { get; set; }

        public IWalletStatementScheduleLogDAOManager _walletStatementScheduleLogDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }

        protected void SetUnitofWork(string tenantName)
        {
            if (UoW == null)
            {
                UoW = new UoW(tenantName.Replace(" ", "") + "_SessionFactory", "PSSWalletStatementRequestJob");
            }
        }

        private void SetWalletStatementScheduleUpdateDAOManager()
        {
            if (_walletStatementScheduleUpdateDAOManager == null) { _walletStatementScheduleUpdateDAOManager = new WalletStatementScheduleUpdateDAOManager(UoW); }
        }

        private void SetWalletStatementScheduleDAOManager()
        {
            if (_walletStatementScheduleDAOManager == null) { _walletStatementScheduleDAOManager = new WalletStatementScheduleDAOManager(UoW); }
        }

        private void SetWalletStatementScheduleLogDAOManager()
        {
            if (_walletStatementScheduleLogDAOManager == null) { _walletStatementScheduleLogDAOManager = new WalletStatementScheduleLogDAOManager(UoW); }
        }

        /// <summary>
        /// Get process composition instance
        /// </summary>
        private void StartHangFireService()
        {
            if (_processCompo == null) { _processCompo = new ProcessComp(); }
            _processCompo.StartHangFireService();
        }


        public void BeginWalletStatementRequest(string tenantName)
        {
            log.Info($"Starting wallet statement request process");
            try
            {
                SetUnitofWork(tenantName);
                SetWalletStatementScheduleDAOManager();
                SetWalletStatementScheduleUpdateDAOManager();
                SetWalletStatementScheduleLogDAOManager();
                WalletStatementScheduleVM walletStatementSchedule = _walletStatementScheduleDAOManager.GetSchedule();
                if(walletStatementSchedule == null)
                {
                    log.Info("No wallet statement schedule found");
                    return;
                }
                log.Info($"Processing wallet statement request for schedule with ID:{walletStatementSchedule.Id} Period Start Date:{walletStatementSchedule.PeriodStartDate} Period End Date:{walletStatementSchedule.PeriodEndDate}");
                UpdateSchedule(walletStatementSchedule);
                StartHangFireService();
                //queues the next job that accepts the schedule as a parameter, gets wallets in batches and queues them for the request
                BackgroundJob.Enqueue(() => new QueueWalletStatementRequest().EnqueueWalletStatementRequest(tenantName, walletStatementSchedule));
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
                _walletStatementScheduleDAOManager = null;
                _walletStatementScheduleUpdateDAOManager = null;
                _walletStatementScheduleLogDAOManager = null;
                _processCompo = null;
            }
        }


        private void UpdateSchedule(WalletStatementScheduleVM schedule)
        {
            try
            {
                DateTime NewScheduleDate = Util.GetNextDate(schedule.CronExpression, schedule.NextScheduleDate).Value;
                WalletStatementScheduleUpdate scheduleUpdate = new WalletStatementScheduleUpdate
                {
                    CurrentSchedule = schedule.NextScheduleDate,
                    NextScheduleDate = NewScheduleDate,
                    NextStartDate = NewScheduleDate.AddHours(-1),
                    NextEndDate = NewScheduleDate.AddMilliseconds(-1),
                    WalletStatementSchedule = new WalletStatementSchedule { Id = schedule.Id }
                };
                UoW.BeginTransaction();
                _walletStatementScheduleLogDAOManager.Add(new WalletStatementScheduleLog { WalletStatementSchedule = new WalletStatementSchedule { Id = schedule.Id }, PeriodStartDate = schedule.PeriodStartDate, PeriodEndDate = schedule.PeriodEndDate });
                _walletStatementScheduleUpdateDAOManager.Add(scheduleUpdate);
                _walletStatementScheduleUpdateDAOManager.UpdateScheduleDate(schedule.Id, scheduleUpdate.Id);
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error($"Error updating POSSAP wallet statement schedule with ID {schedule.Id}");
                log.Error(exception.Message, exception);
                UoW.Rollback();
                throw; 
            }
        }
    }
}
