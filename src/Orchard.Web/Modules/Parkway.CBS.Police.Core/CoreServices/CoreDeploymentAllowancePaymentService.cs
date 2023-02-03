using Orchard;
using Orchard.Logging;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.RemoteClient;
using Parkway.CBS.Services.Models;
using System.Net.Http;
using Parkway.CBS.Police.Core.Utilities;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreDeploymentAllowancePaymentService : ICoreDeploymentAllowancePaymentService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRemoteClient _remoteClient;
        private readonly IDeploymentAllowancePaymentRequestManager<DeploymentAllowancePaymentRequest> _deploymentAllowancePaymentRequestManager;
        private readonly IDeploymentAllowancePaymentRequestItemManager<DeploymentAllowancePaymentRequestItem> _deploymentAllowancePaymentRequestItemManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _serviceRequestFlowDefinitionLevelManager;
        private readonly IRegularizationDeploymentAllowanceSettlementEngineDetailManager<RegularizationDeploymentAllowanceSettlementEngineDetail> _regularizationDeploymentAllowanceSettlementEngineDetailManager;
        ILogger Logger { get; set; }
        public CoreDeploymentAllowancePaymentService(IOrchardServices orchardServices, IDeploymentAllowancePaymentRequestManager<DeploymentAllowancePaymentRequest> deploymentAllowancePaymentRequestManager, IDeploymentAllowancePaymentRequestItemManager<DeploymentAllowancePaymentRequestItem> deploymentAllowancePaymentRequestItemManager, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> serviceRequestFlowDefinitionLevelManager, IRegularizationDeploymentAllowanceSettlementEngineDetailManager<RegularizationDeploymentAllowanceSettlementEngineDetail> regularizationDeploymentAllowanceSettlementEngineDetailManager)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _remoteClient = new RemoteClient.RemoteClient();
            _deploymentAllowancePaymentRequestManager = deploymentAllowancePaymentRequestManager;
            _deploymentAllowancePaymentRequestItemManager = deploymentAllowancePaymentRequestItemManager;
            _serviceRequestFlowDefinitionLevelManager = serviceRequestFlowDefinitionLevelManager;
            _regularizationDeploymentAllowanceSettlementEngineDetailManager = regularizationDeploymentAllowanceSettlementEngineDetailManager;
        }


        /// <summary>
        /// Process deployment allowance payment request
        /// </summary>
        /// <param name="paymentReference">Deployment Allowance Payment Request Payment Reference</param>
        /// <returns></returns>
        public string ProcessPayment(string paymentReference)
        {
            try
            {
                Logger.Information("Getting config values for calling settlement engine");
                string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
                var clientCode = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.POSSAPSettlementClientCode)).SingleOrDefault().Value;
                var secret = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.POSSAPSettlementSecret)).SingleOrDefault().Value;
                var settlementAuthTokenURL = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.SettlementEngineAuthTokenURL)).SingleOrDefault().Value;
                var settlementDirectPaymentURL = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.SettlementEnginePaymentURL)).SingleOrDefault().Value;

                var settlementCallbackURL = siteConfig.Node.Where(x => x.Key == nameof(PSSTenantConfigKeys.DeploymentAllowanceSettlementEnginePaymentCallbackURL)).SingleOrDefault()?.Value;
                if(settlementCallbackURL == null)
                {
                    throw new Exception("Unable to get DeploymentAllowanceSettlementEnginePaymentCallbackURL from web config");
                }

                if (string.IsNullOrEmpty(clientCode) || string.IsNullOrEmpty(secret) || string.IsNullOrEmpty(settlementAuthTokenURL) || string.IsNullOrEmpty(settlementDirectPaymentURL) || string.IsNullOrEmpty(settlementCallbackURL))
                {
                    throw new Exception("Unable to get details for calling settlement engine");
                }

                Logger.Information("Building settlement engine authentication request model");
                SettlementEngineAuthVM authRequest = new SettlementEngineAuthVM { ClientCode = clientCode, hmac = Util.HMACHash256(clientCode, secret) };
                Logger.Information($"Calling settlement engine for token. Request payload - {JsonConvert.SerializeObject(authRequest)}");
                string stoken = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = new Dictionary<string, dynamic> { },
                    Model = authRequest,
                    URL = settlementAuthTokenURL
                }, HttpMethod.Post, new Dictionary<string, string> { });

                Logger.Information($"Token received from settlement engine");
                SettlementEngineAuthResponseVM authtoken = JsonConvert.DeserializeObject<SettlementEngineAuthResponseVM>(stoken);

                Logger.Information($"Getting deployment allowance payment request model with payment reference - {paymentReference}");
                DeploymentAllowancePaymentRequestDTO paymentRequest = _deploymentAllowancePaymentRequestManager.GetDeploymentAllowancePaymentRequestDetailsWithPaymentRefForSettlement(paymentReference);
                if(paymentRequest == null)
                {
                    throw new Exception($"No deployment allowance payment request found with payment reference {paymentReference}");
                }

                Logger.Information($"Getting deployment allowance payment request items for deployment allowance payment request with payment reference - {paymentReference}");
                IEnumerable<DeploymentAllowancePaymentRequestItemDTO> paymentRequestItems = _deploymentAllowancePaymentRequestItemManager.GetItemsInDeploymentAllowancePaymentRequestWithPaymentRef(paymentReference);
                if(paymentRequestItems == null || !paymentRequestItems.Any())
                {
                    throw new Exception($"No deployment allowance payment request items found for deployment allowance payment request with payment reference {paymentReference}");
                }

                Logger.Information($"Building settlement engine direct settlement request model");
                SettlementEnginePaymentRequestVM requestModel = new SettlementEnginePaymentRequestVM
                {
                    SourceAccountNumber = paymentRequest.AccountNumber,
                    ReferenceNumber = paymentReference,
                    Narration = $"{paymentReference}-{DateTime.Now.Date}",
                    PaymentType = "OTHER",
                    SourceBankCode = paymentRequest.Bank.Code,
                    CallbackUrl = settlementCallbackURL,
                    Items = paymentRequestItems.Select(x => new SettlementEnginePaymentItem
                    {
                        ItemRef = x.PaymentReference,
                        AccountName = x.AccountName,
                        AccountNumber = x.AccountNumber,
                        Amount = x.Amount,
                        BankCode = x.Bank.Code,
                        Narration = $"DEPLOYMENT ALLOWANCE {x.CommandTypeName} {x.DayTypeName} FOR PERIOD {x.StartDateString} TO {x.EndDateString}",
                    }).ToList()
                };

                Logger.Information($"Calling settlement engine for direct settlement. Request payload - {JsonConvert.SerializeObject(requestModel)}");
                string response = _remoteClient.SendRequest(new RequestModel
                {
                    Headers = new Dictionary<string, dynamic> { { "Authorization", $"Bearer {authtoken.token}" } },
                    Model = requestModel,
                    URL = settlementDirectPaymentURL
                }, HttpMethod.Post, new Dictionary<string, string> { });
                Logger.Information($"Direct settlement api call successful. Response payload - {response}");


                Logger.Information("Logging request and response payload for direct settlement in RegularizationDeploymentAllowanceSettlementEngineDetail");
                if (!_regularizationDeploymentAllowanceSettlementEngineDetailManager.Save(new RegularizationDeploymentAllowanceSettlementEngineDetail
                {
                    PaymentReference = paymentReference,
                    SettlementEngineRequestJSON = PSSUtil.Truncate(JsonConvert.SerializeObject(requestModel), maxLength: 4000),
                    SettlementEngineResponseJSON = PSSUtil.Truncate(response, maxLength: 4000),
                }))
                {
                    throw new CouldNotSaveRecord("Could not save regularization deployment allowance settlement details");
                };

                Logger.Information($"Updating payment request status for deployment allowance payment request and deployment allowance payment request items to {nameof(PaymentRequestStatus.WAITING)}");
                UpdatePaymentRequestFlow(paymentRequest.Id, paymentRequest.PSServiceRequestFlowDefinitionLevel.Position, paymentRequest.PSServiceRequestFlowDefinitionLevel.DefinitionId, PaymentRequestStatus.WAITING);
                Logger.Information("Update successful");

                return $"₦{paymentRequestItems.Sum(x => x.Amount):n2} has been successfully approved with payment reference : {paymentReference} on source account: {paymentRequest.AccountNumber}.";
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Unable to perform direct settlement on settlement engine. Exception message - {exception.Message}");
                throw;
            }
        }


        /// <summary>
        /// Updates the <see cref="DeploymentAllowancePaymentRequest.FlowDefinitionLevel"/> and <see cref="DeploymentAllowancePaymentRequest.PaymentRequestStatus"/> 
        /// Updates the <see cref="DeploymentAllowancePaymentRequestItem.FlowDefinitionLevel"/> and <see cref="DeploymentAllowancePaymentRequestItem.PaymentRequestStatus"/> 
        /// </summary>
        /// <param name="paymentRequestId"></param>
        /// <param name="currentDefinintionLevelPosition"></param>
        /// <param name="definitionId"></param>
        /// <param name="status"></param>
        private void UpdatePaymentRequestFlow(long paymentRequestId, int currentDefinintionLevelPosition, int definitionId, PaymentRequestStatus status)
        {
            int nextFlowDefinitionLevelId = _serviceRequestFlowDefinitionLevelManager.GetNextLevelDefinitionId(definitionId, currentDefinintionLevelPosition).Id;

            _deploymentAllowancePaymentRequestItemManager.UpdatePaymentRequestStatusId(paymentRequestId, status);

            _deploymentAllowancePaymentRequestManager.UpdatePaymentRequestFlowId(paymentRequestId, nextFlowDefinitionLevelId, status);
        }
    }
}