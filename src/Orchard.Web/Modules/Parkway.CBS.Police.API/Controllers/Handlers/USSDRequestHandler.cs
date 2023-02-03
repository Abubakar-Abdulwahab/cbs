using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core;
using Parkway.CBS.Police.Core.Exceptions;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class USSDRequestHandler : IUSSDRequestHandler
    {
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;
        private readonly IEnumerable<Lazy<IUSSDRequestTypeHandler>> _ussdRequestTypeHandlerImpl;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;

        public USSDRequestHandler(IOrchardServices orchardServices, IEnumerable<Lazy<IUSSDRequestTypeHandler>> ussdRequestTypeHandlerImpl, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _ussdRequestTypeHandlerImpl = ussdRequestTypeHandlerImpl;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
        }

        /// <summary>
        /// This helps to confirm if the number the ussd approval request
        /// is coming from was authorized to approve request.
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
        /// Process ussd approval request
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse ProcessApprovalRequest(USSDRequestModel model)
        {
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

                bool canApproveRequest = _serviceRequestFlowApprover.Value.UserIsValidApprover(model.PhoneNumber);

                if (model.Text.Equals("", StringComparison.Ordinal))
                {
                    return StartMenu(canApproveRequest);
                }

                string[] requestStage = model.Text.Split('|');
                parsed = int.TryParse(requestStage[0], out int requestTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                parsed = Enum.IsDefined(typeof(USSDRequestType), requestTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                if (!canApproveRequest && requestTypeId == (int)USSDRequestType.Approval)
                {
                    throw new UserNotAuthorizedForThisActionException("User not authorized to perfrom this action.");
                }

                foreach (var impl in _ussdRequestTypeHandlerImpl)
                {
                    if ((USSDRequestType)requestTypeId == impl.Value.GetRequestType)
                    {
                        return impl.Value.StartRequest(model);
                    }
                }
                throw new DirtyFormDataException("Invalid input, please try again.");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception, exception.Message);
                return new USSDAPIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), StatusCode = HttpStatusCode.BadRequest, ResponseObject = new ErrorModel { ErrorMessage = exception.Message, FieldName = "ServiceSelection" } };
            }
            catch (NoRecordFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                return new USSDAPIResponse { Error = true, ErrorCode = ErrorCode.PPREC404.ToString(), StatusCode = HttpStatusCode.BadRequest, ResponseObject = new ErrorModel { ErrorMessage = exception.Message, FieldName = "File" } };
            }
            catch (UserNotAuthorizedForThisActionException exception)
            {
                Logger.Error(exception, exception.Message);
                return new USSDAPIResponse { Error = true, ErrorCode = ErrorCode.PPUSER203.ToString(), StatusCode = HttpStatusCode.BadRequest, ResponseObject = new ErrorModel { ErrorMessage = exception.Message, FieldName = "Authorization" } };
            }
            catch (PSSRequestNotPendingApprovalException exception)
            {
                Logger.Error(exception, exception.Message);
                return new USSDAPIResponse { Error = true, ErrorCode = PSSErrorCode.PSSRNPA.ToString(), StatusCode = HttpStatusCode.BadRequest, ResponseObject = new ErrorModel { ErrorMessage = exception.Message, FieldName = "FileNumber" } };
            }
            catch (PSSRequestNotApprovedException exception)
            {
                Logger.Error(exception, exception.Message);
                return new USSDAPIResponse { Error = true, ErrorCode = PSSErrorCode.PSSRNA.ToString(), StatusCode = HttpStatusCode.BadRequest, ResponseObject = new ErrorModel { ErrorMessage = exception.Message, FieldName = "FileNumber" } };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return new USSDAPIResponse { Error = true, ErrorCode = ErrorCode.PPIE.ToString(), StatusCode = HttpStatusCode.BadRequest, ResponseObject = new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "ServiceSelection" } };
            }
        }

        /// <summary>
        /// List available request type for user to pick
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private USSDAPIResponse StartMenu(bool canApproveRequest)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("Welcome to POSSAP Service.\n");
                sb.Append("Please select the service you will like to perform\n");
                sb.Append("\n");
                sb.Append("1.Service Verification\n");
                sb.Append("2.Request Validation\n");

                if (canApproveRequest)
                {
                    sb.Append("10.Request Approval\n");
                }
                return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = sb.ToString() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

    }
}