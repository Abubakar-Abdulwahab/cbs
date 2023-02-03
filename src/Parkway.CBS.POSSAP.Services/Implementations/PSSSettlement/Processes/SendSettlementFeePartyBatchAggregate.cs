using System;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.HangFireInterface.Logger;
using Parkway.CBS.POSSAP.Services.PSSRepositories;
using Parkway.CBS.HangFireInterface.Logger.Contracts;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes.Contracts;
using Parkway.CBS.POSSAP.Services.HelperModel;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient.Contracts;
using Parkway.CBS.HangFireInterface.Notification.RemoteClient;
using Newtonsoft.Json;
using System.Net.Http;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.RemoteClient;
using Parkway.CBS.Police.Core.DTO;
using Hangfire;
using System.Globalization;

namespace Parkway.CBS.POSSAP.Services.Implementations.PSSSettlement.Processes
{
    public class SendSettlementFeePartyBatchAggregate
    {
        private static readonly ILogger log = new Log4netLogger();

        public IUoW UoW { get; set; }

        public IPSSSettlementBatchAggregateDAOManager _pSSSettlementBatchAggregateDAOManager { get; set; }

        public IPSSSettlementFeePartyBatchAggregateDAOManager _pSSSettlementFeePartyBatchAggregateDAOManager { get; set; }

        public IPSSSettlementBatchDAOManager _batchDAOManager { get; set; }

        public IProcessComp _processCompo { get; set; }


        private void SetPSSSettlementFeePartyBatchAggregateDAOManager()
        {
            if (_pSSSettlementFeePartyBatchAggregateDAOManager == null) { _pSSSettlementFeePartyBatchAggregateDAOManager = new PSSSettlementFeePartyBatchAggregateDAOManager(UoW); }
        }


        private void SetSettlementBatchDAOManager()
        {
            if (_batchDAOManager == null) { _batchDAOManager = new PSSSettlementBatchDAOManager(UoW); }
        }


        private void SetSettlementBatchAggregateDAOManager()
        {
            if (_pSSSettlementBatchAggregateDAOManager == null) { _pSSSettlementBatchAggregateDAOManager = new PSSSettlementBatchAggregateDAOManager(UoW); }
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
        /// here we get settlement fee party batch aggregate records and send to settlement engine
        /// </summary>
        /// <param name="tenantName"></param>
        /// <param name="batchId"></param>
        public void SendFeePartyBatchAggregate(string tenantName, long batchId)
        {
            log.Info($"Fetching pss settlement fee party batch aggregate records for batch with id {batchId} and send to settlement engine ");
            SetUnitofWork(tenantName);
            SetSettlementBatchDAOManager();
            PSSSettlementBatchVM batchDetails = _batchDAOManager.GetBatchDetails(batchId);

            try
            {
                if (!ProcessComp.CheckBatchStatus((Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status, Police.Core.Models.Enums.PSSSettlementBatchStatus.FinalProcessing))
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateSettlementBatchHasErrorAndErrorMessage(batchId, true, $"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.FinalProcessing}");
                    UoW.Commit();
                    throw new Exception($"Status mismatch for settlement batch with id {batchId}. Batch status - {(Police.Core.Models.Enums.PSSSettlementBatchStatus)batchDetails.Status} Expected status - {Police.Core.Models.Enums.PSSSettlementBatchStatus.FinalProcessing}");
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
                var clientCode = ConfigurationManager.AppSettings["POSSAPSettlementClientCode"];
                var secret = ConfigurationManager.AppSettings["POSSAPSettlementSecret"];
                var settlementAuthTokenURL = ConfigurationManager.AppSettings["SettlementEngineAuthTokenURL"];
                var settlementDirectSettlementURL = ConfigurationManager.AppSettings["SettlementEngineDirectSettlementURL"];

                if (string.IsNullOrEmpty(clientCode) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(settlementAuthTokenURL) || string.IsNullOrEmpty(settlementDirectSettlementURL))
                {
                    throw new Exception("Unable to get details for calling settlement engine");
                }

                IRemoteClient _remoteClient = new RemoteClient.RemoteClient();
                PSSSettlementFeePartyBatchAggregateSettlementRequestModel requestModel = null;
                
                SetPSSSettlementFeePartyBatchAggregateDAOManager();
                SetSettlementBatchAggregateDAOManager();
                if (_pSSSettlementFeePartyBatchAggregateDAOManager.Count(x => (x.Batch.Id == batchId)) > 0)
                {
                    if (_pSSSettlementBatchAggregateDAOManager.Count(x => x.SettlementBatch.Id == batchId) == 0)
                    {
                        PSSSettlementFeePartyBatchAggregateSettlementRequestModel feePartyBatchAggregateRequestModel = _batchDAOManager.GetSettlementBatchInfoForFeePartyBatchAggregateRequestModel(batchId);
                        feePartyBatchAggregateRequestModel.Items = _pSSSettlementFeePartyBatchAggregateDAOManager.GetFeePartyBatchAggregateItemsForRequest(batchId);
                        feePartyBatchAggregateRequestModel.ReferenceNumber = string.Format("PSS-BATCH-{0}-{1}-{2}", batchId, feePartyBatchAggregateRequestModel.StartDate, feePartyBatchAggregateRequestModel.EndDate);

                        requestModel = feePartyBatchAggregateRequestModel;
                        decimal settlementAmount = feePartyBatchAggregateRequestModel.Items.Sum(x => x.Amount);
                        UoW.BeginTransaction();
                        _pSSSettlementBatchAggregateDAOManager.Add(new PSSSettlementBatchAggregate
                        {
                            SettlementBatch = new PSSSettlementBatch { Id = batchId },
                            RetryCount = 0,
                            TransactionCount = 1,
                            Error = false,
                            Amount = settlementAmount,
                            TimeFired = DateTime.Now.ToLocalTime(),
                            SettlementEngineRequestJSON = JsonConvert.SerializeObject(requestModel),
                            RequestReference = feePartyBatchAggregateRequestModel.ReferenceNumber,
                            ErrorType = (int)Police.Core.Models.Enums.ErrorType.None
                        });
                        _batchDAOManager.UpdateSettlementAmountForBatch(batchId, settlementAmount);
                        UoW.Commit();
                    }
                    else
                    {
                        PSSSettlementBatchAggregateVM batchAggregate = _pSSSettlementBatchAggregateDAOManager.GetBatchAggregate(batchId);
                        requestModel = JsonConvert.DeserializeObject<PSSSettlementFeePartyBatchAggregateSettlementRequestModel>(batchAggregate.SettlementEngineRequestJSON);
                        int retryCount = batchAggregate.RetryCount + 1;
                        UoW.BeginTransaction();
                        _pSSSettlementBatchAggregateDAOManager.UpdateBatchAggregateRetryCount(batchId, retryCount);
                        UoW.Commit();
                    }

                    SettlementEngineAuthVM authRequest = new SettlementEngineAuthVM { ClientCode = clientCode, hmac = Core.Utilities.Util.HMACHash256(clientCode, secret) };
                    string stoken = _remoteClient.SendRequest(new RequestModel
                    {
                        Headers = new Dictionary<string, dynamic> { },
                        Model = authRequest,
                        URL = settlementAuthTokenURL
                    }, HttpMethod.Post, new Dictionary<string, string> { });
                    SettlementEngineAuthResponseVM authtoken = JsonConvert.DeserializeObject<SettlementEngineAuthResponseVM>(stoken);

                    var response = _remoteClient.SendRequest(new RequestModel
                    {
                        Headers = new Dictionary<string, dynamic> { { "Authorization", $"Bearer {authtoken.token}" } },
                        Model = requestModel,
                        URL = settlementDirectSettlementURL
                    }, HttpMethod.Post, new Dictionary<string, string> { });

                    log.Info($"Response From Settlement Engine Direct Settlement - {response}");

                    SettlementEngineResponse responseModel = JsonConvert.DeserializeObject<SettlementEngineResponse>(response);

                    if (responseModel.Error)
                    {
                        UoW.BeginTransaction();
                        _pSSSettlementBatchAggregateDAOManager.UpdateErrorTypeAndErrorMessage(batchId, (int)Police.Core.Models.Enums.ErrorType.SettlementEngine, responseModel.ErrorMessage, response);
                        UoW.Commit();
                    }
                    else
                    {
                        if (!DateTime.TryParse(responseModel.ResponseObject.ProcessDate.ToString(), out DateTime settlementDate))
                        {
                            throw new Exception("Unable to parse process date from settlement engine");
                        }
                        UoW.BeginTransaction();
                        _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.MoveComputedItemsToBatchItemsTable, "Moving Computed Items To Batch Items", settlementDate);
                        _pSSSettlementBatchAggregateDAOManager.UpdateBatchAggregateWithSettlementEngineResponse(batchId, Truncate(response, 4000));
                        UoW.Commit();
                        StartHangFireService();
                        BackgroundJob.Enqueue(() => new MoveComputedItemsToBatchItemsTable().MoveRecords(tenantName, batchId));
                    }
                }
                else
                {
                    UoW.BeginTransaction();
                    _batchDAOManager.UpdateBatchStatus(batchId, Police.Core.Models.Enums.PSSSettlementBatchStatus.TransactionsMarkedAsSettled, "No Transactions For This Period", DateTime.Now);
                    UoW.Commit();
                }

            }
            catch (Exception exception)
            {
                log.Error($"Error processing POSSAP settlement");
                log.Error(exception.Message, exception);
                UoW.BeginTransaction();
                _pSSSettlementBatchAggregateDAOManager.UpdateErrorTypeAndErrorMessage(batchId, (int)Police.Core.Models.Enums.ErrorType.Application, exception.Message);
                UoW.Commit();
                throw;
            }
            finally
            {
                if (UoW != null) { UoW.Dispose(); UoW = null; }
                _batchDAOManager = null;
                _pSSSettlementFeePartyBatchAggregateDAOManager = null;
                _processCompo = null;
                _pSSSettlementBatchAggregateDAOManager = null;
            }
        }


        public string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
