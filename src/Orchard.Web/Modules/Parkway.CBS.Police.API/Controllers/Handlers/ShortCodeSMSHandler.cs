using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Linq;
using System.Net;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class ShortCodeSMSHandler : IShortCodeSMSHandler
    {
        public ILogger Logger { get; set; }
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private IOrchardServices _orchardServices;
        private readonly Lazy<IExtractDetailsManager<ExtractDetails>> _extractDetailManager;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;

        public ShortCodeSMSHandler(IOrchardServices orchardServices, Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<IExtractDetailsManager<ExtractDetails>> extractDetailManager, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _extractDetailManager = extractDetailManager;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
        }

        /// <summary>
        /// This helps to confirm if the number the sms request
        /// is coming from was authorized to update the specified file number content
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>bool</returns>
        private bool ConfirmNumberAuthorizationStatus(string phoneNumber)
        {
            try
            {
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.AuthorizedApprovalNumbers.ToString()).FirstOrDefault();
                if (node == null || string.IsNullOrEmpty(node.Value))
                {
                    return false;
                }

                string[] numbers = node.Value.Split(',');
                string number = numbers.Where(num => num.Trim() == phoneNumber).FirstOrDefault();

                if (!string.IsNullOrEmpty(number))
                {
                    return true;
                }
                return false;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return false;
            }
        }

        /// <summary>
        /// This helps to confirm the SMS originator short code number
        /// </summary>
        /// <param name="shortCodeNumber"></param>
        /// <returns>bool</returns>
        private bool ConfirmOriginatorShortCodeNumber(string shortCodeNumber)
        {
            try
            {
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.SMSShortCode.ToString()).FirstOrDefault();
                if (node == null || string.IsNullOrEmpty(node.Value) || shortCodeNumber != node.Value)
                {
                    return false;
                }

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return false;
            }
        }


        /// <summary>
        /// Process shortcode SMS content update request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        public APIResponse ProcessRequest(ShortCodeSMSRequestModel model)
        {
            //SMS format is in the form of FileNumber*Content
            HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
            try
            {
                int phoneTrimStartIndex;
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.PhoneNumberTrimIndex.ToString()).FirstOrDefault();
                if (node == null || string.IsNullOrEmpty(node.Value))
                {
                    phoneTrimStartIndex = 3;
                }

                bool parsed = int.TryParse(node.Value, out phoneTrimStartIndex);
                model.PhoneNumber = $"0{model.PhoneNumber.Substring(phoneTrimStartIndex)}";

                bool confirmPhoneNumberAuthStatus = _serviceRequestFlowApprover.Value.UserIsValidApprover(model.PhoneNumber);
                if (!confirmPhoneNumberAuthStatus)
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "You are not authorized to perform this action" };
                }

                bool confirmShortCodeSource = ConfirmOriginatorShortCodeNumber(model.ServiceCode);
                if (!confirmShortCodeSource)
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "Unable to confirm the source of the SMS" };
                }

                PSSRequestVM requestDet = _requestManager.Value.GetRequestDetails(model.FileNumber);
                if (requestDet == null)
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "File Number not found" };
                }

                confirmPhoneNumberAuthStatus = _serviceRequestFlowApprover.Value.UserIsValidApproverForDefinitionLevel(model.PhoneNumber, requestDet.FlowDefinitionLevelId);
                if (!confirmPhoneNumberAuthStatus)
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "You are not authorized to perform this action" };
                }

                if (requestDet.Status != PSSRequestStatus.PendingApproval || requestDet.ServiceTypeId != (int)PSSServiceTypeDefinition.Extract)
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "Request not pending approval or mismatch File Number" };
                }

                if (string.IsNullOrEmpty(model.Content))
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "Content field value is required" };
                }

                if (model.Content.Trim().Length < 10 || model.Content.Trim().Length > 1000)
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "Content field value must be between 10 and 1000 characters" };
                }

                if (_extractDetailManager.Value.CheckExtractContentDetails(model.FileNumber))
                {
                    return new APIResponse { Error = true, StatusCode = httpStatusCode, ResponseObject = "Content has already been updated" };
                }

                _extractDetailManager.Value.UpdateExtractDetailsContent(requestDet.Id, model.Content.Trim());
                return new APIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Content updated successfully" };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}