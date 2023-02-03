using System;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes.Contracts;
using Parkway.CBS.POSSAP.Services.HelperModel;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.RemoteClient;
using System.Net.Http;
using Newtonsoft.Json;
using Parkway.CBS.Police.Core.Models;
using System.Data;
using System.Linq;
using System.Configuration;
using Hangfire;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSWalletStatementRequest.Processes
{
    public class ProcessWalletStatementRequest
    {
        private static readonly ILogger log = new Log4netLogger();

        private int maxNumberOfWalletStatementsReturnedPerCall;

        private DataTable dataTable;

        private string reference;

        public IUoW UoW { get; set; }

        public ICommandWalletDetailsDAOManager _commandWalletDetailsDAOManager { get; set; }

        public IPSSFeePartyDAOManager _pssFeePartyDAOManager { get; set; }

        public IWalletStatementCallLogDAOManager _walletStatementCallLogDAOManager { get; set; }

        public IFailedWalletStatementRequestCallLogDAOManager _failedWalletStatementRequestCallLogDAOManager { get; set; }

        public IWalletStatementStagingDAOManager _walletStatementStagingDAOManager;

        public IProcessComp _processCompo { get; set; }

        private IRemoteClient _remoteClient;

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


        private void SetWalletStatementCallLogDAOManager()
        {
            if (_walletStatementCallLogDAOManager == null) { _walletStatementCallLogDAOManager = new WalletStatementCallLogDAOManager(UoW); }
        }

        private void SetFailedWalletStatementRequestCallLogDAOManager()
        {
            if (_failedWalletStatementRequestCallLogDAOManager == null) { _failedWalletStatementRequestCallLogDAOManager = new FailedWalletStatementRequestCallLogDAOManager(UoW); }
        }

        private void SetWalletStatementStagingDAOManager()
        {
            if (_walletStatementStagingDAOManager == null) { _walletStatementStagingDAOManager = new WalletStatementStagingDAOManager(UoW); }
        }

        /// <summary>
        /// Get process composition instance
        /// </summary>
        private void StartHangFireService()
        {
            if (_processCompo == null) { _processCompo = new ProcessComp(); }
            _processCompo.StartHangFireService();
        }

        public void GetWalletStatement(string tenantName, int skip, int take, WalletStatementScheduleVM schedule, WalletIdentifierType walletIdentifierType)
        {
            log.Info($"Processing wallet statement requests in batches");
            try
            {
                SetUnitofWork(tenantName);
                SetWalletStatementStagingDAOManager();
                SetWalletStatementCallLogDAOManager();
                SetFailedWalletStatementRequestCallLogDAOManager();
                reference = $"WAL-STATMNT-REQ-{walletIdentifierType}-{skip}-{DateTime.Now.Ticks}-{schedule.PeriodStartDate}-{schedule.PeriodEndDate}";
                var sMaxWalletStatementsReturnedPerCall = ConfigurationManager.AppSettings["MaxNumberOfWalletStatementsReturnedPerCall"];
                if (!int.TryParse(sMaxWalletStatementsReturnedPerCall, out maxNumberOfWalletStatementsReturnedPerCall)) { maxNumberOfWalletStatementsReturnedPerCall = 10; };

                dataTable = new DataTable("Parkway_CBS_Police_Core" + typeof(WalletStatementStaging).Name);
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.TransactionDate), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.ValueDate), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.Narration), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.TransactionTypeId), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.Amount), typeof(decimal)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.TransactionReference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.WalletId), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.WalletIdentifierType), typeof(int)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.Reference), typeof(string)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.CreatedAtUtc), typeof(DateTime)));
                dataTable.Columns.Add(new DataColumn(nameof(WalletStatementStaging.UpdatedAtUtc), typeof(DateTime)));

                _remoteClient = new RemoteClient.RemoteClient();

                switch (walletIdentifierType)
                {
                    case WalletIdentifierType.CommandWalletDetails:
                        ProcessCommandWalletStatementRequest(skip, take, schedule);
                        break;
                    case WalletIdentifierType.PSSFeeParty:
                        ProcessFeePartyWalletStatementRequest(skip, take, schedule);
                        break;
                }

                StartHangFireService();
                //queues job that moves wallet statements from staging to main table, this job would accept the reference as a parameter
                BackgroundJob.Enqueue(() => new MoveWalletStatementsFromStagingToMainTable().MoveWalletStatementsFromStagingToMain(tenantName, reference));
            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP wallet statement request. Exception message - {exception.Message}");
                log.Error(exception.Message, exception);
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                _commandWalletDetailsDAOManager = null;
                _pssFeePartyDAOManager = null;
                _walletStatementCallLogDAOManager = null;
                _failedWalletStatementRequestCallLogDAOManager = null;
                _walletStatementStagingDAOManager = null;
                _processCompo = null;
            }
        }


        private void ProcessCommandWalletStatementRequest(int skip, int take, WalletStatementScheduleVM schedule)
        {
            try
            {
                SetCommandWalletDetailsDAOManager();
                IEnumerable<CommandWalletDetailsVM> commandWalletDetails = _commandWalletDetailsDAOManager.GetCommandWalletDetails(skip, take);
                foreach (var wallet in commandWalletDetails)
                {
                    dataTable.Clear();
                    log.Info($"Getting statement for command wallet with id:{wallet.Id}");
                    GetWalletStatementsForWalletWithIdAndType(wallet.Id, WalletIdentifierType.CommandWalletDetails, schedule);
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error processing command wallet statement request. Exception message - {exception.Message}");
                throw;
            }
        }


        private void ProcessFeePartyWalletStatementRequest(int skip, int take, WalletStatementScheduleVM schedule)
        {
            try
            {
                SetPSSFeePartyDAOManager();
                IEnumerable<PSSFeePartyVM> feeParties = _pssFeePartyDAOManager.GetFeeParties(skip, take);
                foreach (var feeParty in feeParties)
                {
                    dataTable.Clear();
                    log.Info($"Getting statement for fee party with id:{feeParty.Id}");
                    GetWalletStatementsForWalletWithIdAndType(feeParty.Id, WalletIdentifierType.PSSFeeParty, schedule);
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error in processing fee party wallet statement request. Exception message - {exception.Message}");
                throw;
            }
        }


        private void GetWalletStatementsForWalletWithIdAndType(int walletId, WalletIdentifierType type, WalletStatementScheduleVM schedule, int skip = 0)
        {
            string errorMessage = string.Empty;
            try
            {
                string statementResponse = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = new Dictionary<string, dynamic> { },
                    Model = new { StartDate = schedule.PeriodStartDate.ToString(), EndDate = schedule.PeriodEndDate.ToString(), Skip = skip },
                    URL = "http://pss.cbs/Admin/Police/Mock/Get-Wallet-Statements"
                }, HttpMethod.Post, new Dictionary<string, string> { });

                //check if results in the response is max returned by endpoint
                WalletStatementMockAPIResponseModel statementsResponseObj = JsonConvert.DeserializeObject<WalletStatementMockAPIResponseModel>(statementResponse);
                if (statementsResponseObj.Error) { errorMessage = statementsResponseObj.ErrorMessage; throw new Exception(); }
                UpdateWalletStatementItems(walletId, type, statementsResponseObj);
                if (statementsResponseObj.items.Count == maxNumberOfWalletStatementsReturnedPerCall)
                {
                    GetWalletStatementsForWalletWithIdAndType(walletId, type, schedule, skip + maxNumberOfWalletStatementsReturnedPerCall);
                    return;
                }

                UoW.BeginTransaction();
                SaveToWalletStatementCallLog(walletId, type, schedule);
                SaveWalletStatementToStaging();
                UoW.Commit();
            }
            catch (Exception exception)
            {
                log.Error($"Error getting statements for wallet with id:{walletId} and wallet identifier type:{type}. Exception message - {exception.Message}");
                errorMessage = string.IsNullOrEmpty(errorMessage) ? exception.Message : errorMessage;
                if (!UoW.Session.Transaction.IsActive) { UoW.BeginTransaction(); }
                SaveToFailedCallLog(walletId, type, schedule, errorMessage);
                UoW.Commit();
            }
        }


        private void SaveToWalletStatementCallLog(int walletId, WalletIdentifierType type, WalletStatementScheduleVM schedule)
        {
            try
            {
                _walletStatementCallLogDAOManager.Add(new WalletStatementCallLog
                {
                    WalletId = walletId,
                    WalletIdentifierType = (int)type,
                    StartDate = schedule.PeriodStartDate,
                    EndDate = schedule.PeriodEndDate
                });
            }
            catch (Exception exception)
            {
                log.Error($"Error saving to wallet statement call log for wallet with id:{walletId} and wallet identifier type:{type}. Exception message - {exception.Message}");
                UoW.Rollback();
                throw;
            }
        }


        private void SaveToFailedCallLog(int walletId, WalletIdentifierType type, WalletStatementScheduleVM schedule, string errorMessage)
        {
            try
            {
                _failedWalletStatementRequestCallLogDAOManager.Add(new FailedWalletStatementRequestCallLog
                {
                    WalletId = walletId,
                    WalletIdentifierType = (int)type,
                    RetryCount = 0,
                    ErrorMessage = errorMessage,
                    StartDate = schedule.PeriodStartDate,
                    EndDate = schedule.PeriodEndDate
                });
            }
            catch (Exception exception)
            {
                log.Error($"Error saving to failed wallet statement request call log for wallet with id:{walletId} and wallet identifier type:{type}. Exception message - {exception.Message}");
                throw;
            }
        }


        private void UpdateWalletStatementItems(int walletId, WalletIdentifierType type, dynamic statementResponseObj)
        {
            try
            {
                foreach (var item in statementResponseObj.items)
                {
                    DataRow row = dataTable.NewRow();
                    row[nameof(WalletStatementStaging.TransactionDate)] = DateTime.Parse(item.date.ToString());
                    row[nameof(WalletStatementStaging.ValueDate)] = DateTime.Parse(item.date.ToString());
                    row[nameof(WalletStatementStaging.Narration)] = item.longDescription.ToString();
                    row[nameof(WalletStatementStaging.TransactionTypeId)] = int.Parse(item.tranType.ToString());
                    row[nameof(WalletStatementStaging.Amount)] = decimal.Parse(item.amount.ToString());
                    row[nameof(WalletStatementStaging.TransactionReference)] = item.ourReference.ToString();
                    row[nameof(WalletStatementStaging.WalletId)] = walletId;
                    row[nameof(WalletStatementStaging.WalletIdentifierType)] = (int)type;
                    row[nameof(WalletStatementStaging.Reference)] = reference;
                    row[nameof(WalletStatementStaging.CreatedAtUtc)] = DateTime.Now.ToLocalTime();
                    row[nameof(WalletStatementStaging.UpdatedAtUtc)] = DateTime.Now.ToLocalTime();
                    dataTable.Rows.Add(row);
                }
            }
            catch (Exception exception)
            {
                log.Error($"Error building wallet statement staging data table for wallet with id:{walletId} and wallet identifier type:{type}. Exception message - {exception.Message}");
                throw;
            }
        }


        private void SaveWalletStatementToStaging()
        {
            try
            {
                _walletStatementStagingDAOManager.SaveBundle(new List<DataTable> { dataTable }, "Parkway_CBS_Police_Core_" + typeof(WalletStatementStaging).Name);
            }
            catch (Exception exception)
            {
                log.Error($"Error saving wallet statements to wallet statement staging table. Exception message - {exception.Message}");
                UoW.Rollback();
                throw;
            }
        }
    }
}
