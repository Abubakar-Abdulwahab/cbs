using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PaymentRequestSettlement.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.RemoteClient;
using Parkway.CBS.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Parkway.CBS.Police.Core.PaymentRequestSettlement
{
    public class PaymentRequestService : IPaymentRequestService
    {
        private readonly IAccountPaymentRequestItemManager<AccountPaymentRequestItem> _accountPaymentRequestItemManager;
        private readonly IAccountPaymentRequestManager<AccountPaymentRequest> _accountPaymentRequestManager;
        private readonly IAccountPaymentRequestSettlementDetailManager<AccountPaymentRequestSettlementDetail> _accountPaymentRequestSettlementDetailManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _serviceRequestFlowDefinitionLevelManager;
        private readonly IOrchardServices _orchardServices;
        private readonly IRemoteClient _remoteClient;
        public ILogger Logger { get; set; }

        public PaymentRequestService(IAccountPaymentRequestItemManager<AccountPaymentRequestItem> accountPaymentRequestItemManager, IAccountPaymentRequestManager<AccountPaymentRequest> accountPaymentRequestManager, IAccountPaymentRequestSettlementDetailManager<AccountPaymentRequestSettlementDetail> accountPaymentRequestSettlementDetailManager, IOrchardServices orchardServices, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> serviceRequestFlowDefinitionLevelManager)
        {
            _accountPaymentRequestItemManager = accountPaymentRequestItemManager;
            _accountPaymentRequestManager = accountPaymentRequestManager;
            _accountPaymentRequestSettlementDetailManager = accountPaymentRequestSettlementDetailManager;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _remoteClient = new RemoteClient.RemoteClient();
            _serviceRequestFlowDefinitionLevelManager = serviceRequestFlowDefinitionLevelManager;
        }

        /// <summary>
        /// Send request to settlement engine for processing
        /// </summary>
        /// <param name="paymentId"></param>
        public string BeginRequestPaymentProcess(string paymentId)
        {
            try
            {
                if (_accountPaymentRequestManager.Count(x => x.PaymentRequestStatus != (int)PaymentRequestStatus.UNDERAPPROVAL && x.PaymentRequestStatus != (int)PaymentRequestStatus.AWAITINGAPPROVAL && x.PaymentReference == paymentId) > 0)
                {
                    throw new InvalidOperationException($"Payment request with paymentId: {paymentId} has already been processed or not authorized");
                }

                return ProcessPayment(_accountPaymentRequestItemManager.GetPaymentRequestItems(paymentId), paymentId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        private string ProcessPayment(IEnumerable<AccountWalletPaymentRequestItemDTO> paymentRequestItems, string paymentId)
        {
            if (!paymentRequestItems.Any())
            {
                return $"No payment request items found with payment Id: {paymentId}.";
            }

            Logger.Information($"About to send payment with reference {paymentId} to settlement engine");
            string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
            StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
            var clientCode = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.POSSAPSettlementClientCode.ToString()).FirstOrDefault().Value;
            var secret = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.POSSAPSettlementSecret.ToString()).FirstOrDefault().Value;
            var settlementAuthTokenURL = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.SettlementEngineAuthTokenURL.ToString()).FirstOrDefault().Value;
            var settlementDirectPaymentURL = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.SettlementEnginePaymentURL.ToString()).FirstOrDefault().Value;

            var settlementCallbackURL = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.SettlementEnginePaymentCallbackURL.ToString()).FirstOrDefault().Value;

            if (string.IsNullOrEmpty(clientCode) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(settlementAuthTokenURL) || string.IsNullOrEmpty(settlementDirectPaymentURL) || string.IsNullOrEmpty(settlementCallbackURL))
            {
                throw new Exception("Unable to get details for calling settlement engine");
            }

            SettlementEngineAuthVM authRequest = new SettlementEngineAuthVM { ClientCode = clientCode, hmac = Util.HMACHash256(clientCode, secret) };
            string stoken = _remoteClient.SendRequest(new RequestModel
            {
                Headers = new Dictionary<string, dynamic> { },
                Model = authRequest,
                URL = settlementAuthTokenURL
            }, HttpMethod.Post, new Dictionary<string, string> { });

            SettlementEngineAuthResponseVM authtoken = JsonConvert.DeserializeObject<SettlementEngineAuthResponseVM>(stoken);

            AccountPaymentSettlementRequestVM paymentRequest = _accountPaymentRequestManager.GetWalletPaymentDetailForSettlementByPaymentId(paymentId);

            SettlementEnginePaymentRequestVM requestModel = new SettlementEnginePaymentRequestVM
            {
                SourceAccountNumber = paymentRequest.SourceAccountNumber,
                ReferenceNumber = paymentId,
                Narration = $"{paymentId}-{DateTime.Now.Date}",
                PaymentType = "OTHER",
                SourceBankCode = paymentRequest.BankCode,
                CallbackUrl = settlementCallbackURL,
                Items = paymentRequestItems.Select(x => new SettlementEnginePaymentItem
                {
                    ItemRef = x.PaymentReference,
                    AccountName = x.AccountName,
                    AccountNumber = x.AccountNumber,
                    Amount = x.Amount,
                    BankCode = x.BankCode,
                    Narration = x.ExpenditureHeadName,
                }).ToList()
            };

            string response = _remoteClient.SendRequest(new RequestModel
            {
                Headers = new Dictionary<string, dynamic> { { "Authorization", $"Bearer {authtoken.token}" } },
                Model = requestModel,
                URL = settlementDirectPaymentURL
            }, HttpMethod.Post, new Dictionary<string, string> { });

            _accountPaymentRequestSettlementDetailManager.Save(new AccountPaymentRequestSettlementDetail
            {
                PaymentReference = paymentId,
                SettlementEngineRequestJSON = PSSUtil.Truncate(JsonConvert.SerializeObject(requestModel), maxLength: 4000),
                SettlementEngineResponseJSON = PSSUtil.Truncate(response, maxLength: 4000),
            });

            Logger.Information($"Payment with reference {paymentId} sent to settlement engine successfully.");
            UpdatePaymentRequestFlow(paymentRequest.Id, paymentRequest.FlowDefinitionLevelPosition, paymentRequest.FlowDefinitionId, PaymentRequestStatus.WAITING);

            return $"₦{paymentRequestItems.Sum(x => x.Amount):n2} has been successfully approved with payment reference : {paymentId} on source account: {paymentRequest.SourceAccountNumber}.";

        }

        /// <summary>
        /// Updates the <see cref="AccountPaymentRequest.FlowDefinitionLevel"/> and <see cref="AccountPaymentRequest.PaymentRequestStatus"/> 
        /// Updates the <see cref="AccountPaymentRequestItem.FlowDefinitionLevel"/> and <see cref="AccountPaymentRequestItem.PaymentRequestStatus"/> 
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="currentDefinintionLevelPosition"></param>
        /// <param name="definitionId"></param>
        /// <param name="status"></param>
        public void UpdatePaymentRequestFlow(long paymentRequestId, int currentDefinintionLevelPosition,  int definitionId, PaymentRequestStatus status)
        {
            int nextFlowDefinitionLevelId = _serviceRequestFlowDefinitionLevelManager.GetNextLevelDefinitionId(definitionId, currentDefinintionLevelPosition).Id;

            _accountPaymentRequestItemManager.UpdatePaymentRequestStatusId(paymentRequestId, status);

            _accountPaymentRequestManager.UpdatePaymentRequestFlowId(paymentRequestId, nextFlowDefinitionLevelId, status);
        }

    }
}