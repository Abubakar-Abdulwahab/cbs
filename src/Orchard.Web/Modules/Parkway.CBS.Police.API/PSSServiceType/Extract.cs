using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;

namespace Parkway.CBS.Police.Core.PSSServiceType
{
    public class Extract : IPSSServiceTypeUSSDApprovalImpl, IPSSServiceTypeUSSDApprovalValidatorImpl
    {
        private IOrchardServices _orchardServices;
        public PSSServiceTypeDefinition GetServiceTypeDefinition => PSSServiceTypeDefinition.Extract;
        public ILogger Logger { get; set; }
        private readonly Lazy<IUSSDRequestApprovalHandler> _requestApprovalHandler;
        private readonly Lazy<IExtractDetailsManager<ExtractDetails>> _extractDetailManager;
        private readonly Lazy<ITypeImplComposer> _typeImplComposer;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;
        private readonly Lazy<IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog>> _requestCommandWorkFlowLogManager;
        private const int DiaryAndDeclineCommentStage = 4;
        private const int IncidentDateAndTimeStage = 5;
        private const int CrossReferenceStage = 6;
        private const int SecondApproverDPOForceNoStage = 4;

        public Extract(Lazy<IUSSDRequestApprovalHandler> requestApprovalHandler, Lazy<IExtractDetailsManager<ExtractDetails>> extractDetailManager, IOrchardServices orchardServices, Lazy<ITypeImplComposer> typeImplComposer, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover, Lazy<IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog>> requestCommandWorkFlowLogManager)
        {
            Logger = NullLogger.Instance;
            _requestApprovalHandler = requestApprovalHandler;
            _extractDetailManager = extractDetailManager;
            _orchardServices = orchardServices;
            _typeImplComposer = typeImplComposer;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
            _requestCommandWorkFlowLogManager = requestCommandWorkFlowLogManager;
        }

        /// <summary>
        /// This handles processing of a USSD aproval request that has to do with extract
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse ProcessRequest(USSDRequestModel model)
        {
            string[] requestStage = model.Text.Split('|');

            bool checkDiaryAndIncidentDate = _extractDetailManager.Value.CheckExtractDiaryIncidentDetails(requestStage[1]);
            if (requestStage.Length == (int)USSDProcessingStage.OperationType)
            {
                bool parsed = int.TryParse(requestStage[2], out int operationTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                parsed = Enum.IsDefined(typeof(USSDOperationType), operationTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                //If it is a decline request, prompt the user to enter comment
                if (operationTypeId == (int)USSDOperationType.Decline)
                {
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Comment (Minimum of ten characters)\n" };
                }

                //If it is an approve request, check if content detail has already being populated
                bool hasContent = _extractDetailManager.Value.CheckExtractContentDetails(requestStage[1]);
                if (!hasContent)
                {
                    Node node = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == PSSTenantConfigKeys.SMSShortCode.ToString()).FirstOrDefault();
                    if (node != null && !string.IsNullOrEmpty(node.Value))
                    {
                        throw new NoRecordFoundException($"Please send the Contents of the extract (minimum of 10 and maximum of 1000 characters) via SMS to {node.Value} before you continue with this process.");
                    }

                    throw new NoRecordFoundException($"Please send the Contents of the extract (minimum of 10 and maximum of 1000 characters) via SMS to the provided short code before you continue with this process.");
                }

                //If Diary Number and Incident Date Time have not been populated, present a menu for user to enter Diary Number
                //In the case of the approver is not a first approver, in which Diary Number and Incident Date Time have been populated by first approver, prompt the user to enter DPO Force No
                if (!checkDiaryAndIncidentDate)
                {
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Diary Serial Number\n" };
                }

                //Commented out the certificate draft view confirmaion because we were told to remove it.
                //_requestApprovalHandler.Value.ConfirmAdminHasViewedDraftDocument(requestStage[1], $"0{model.PhoneNumber.Substring(PhoneTrimStartIndex)}");
                return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter AP/Force No of the DPO to provide the service\n" };
            }

            //Stage 4 is equivalent to comment stage in the case of decline request and Enter Diary Number in the case of approve request
            //System checks and process the reject if it's a decline request
            if (requestStage.Length == DiaryAndDeclineCommentStage)
            {
                int.TryParse(requestStage[2], out int operationTypeId);
                if (operationTypeId == (int)USSDOperationType.Decline)
                {
                    string approvalResponse = ProcessApproval(model);
                    ObjectCacheProvider.RemoveCachedObject(_orchardServices.WorkContext.CurrentSite.SiteName, $"USSD-{nameof(POSSAPCachePrefix.FileNumber)}-{requestStage[1]}");
                    return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = approvalResponse };
                }
            }

            //If this is true, it means Diary Number and Incident Date Time have been populated, then prompt the user to enter comment
            if (checkDiaryAndIncidentDate && requestStage.Length == SecondApproverDPOForceNoStage)
            {
                try
                {
                    PoliceOfficerVM officerVM = _typeImplComposer.Value.GetPoliceOfficer(requestStage[3]);
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Dear {officerVM.Name}");
                    sb.Append("\n");
                    sb.Append("Please enter comment (Minimum of ten characters) to proceed\n");
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = sb.ToString() };
                }
                catch (Exception)
                {
                    throw;
                }
            }

            //If this is true, it means Diary Number and Incident Date Time have been populated, then just go ahead to approve the request
            if (checkDiaryAndIncidentDate)
            {
                string approvalResponse = ProcessApproval(model, true);
                ObjectCacheProvider.RemoveCachedObject(_orchardServices.WorkContext.CurrentSite.SiteName, $"USSD-{nameof(POSSAPCachePrefix.FileNumber)}-{requestStage[1]}");
                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = approvalResponse };
            }

            switch (requestStage.Length)
            {
                case DiaryAndDeclineCommentStage:
                    if (string.IsNullOrEmpty(requestStage[3]))
                    {
                        throw new DirtyFormDataException("Diary Serial Number field cannot be empty.");
                    }
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Incident Date and Time e.g 17/10/2021 17:35\n" };

                case IncidentDateAndTimeStage:
                    if (string.IsNullOrEmpty(requestStage[4]))
                    {
                        throw new DirtyFormDataException("Incident Date and Time is required. Must be in format dd/mm/yyyy HH:mm.");
                    }
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = ParseIncidentDate(requestStage[4]) };

                case CrossReferenceStage:
                    string crossReferenceNumber = requestStage[5];
                    if (!string.IsNullOrEmpty(crossReferenceNumber))
                    {
                        if (crossReferenceNumber.Trim().Length < 1 || crossReferenceNumber.Trim().Length > 10)
                        {
                            return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Cross Reference value must be between 1 and 10 characters\n" };
                        }
                    }
                    string phoneNumber = model.PhoneNumber;
                    //Commented out the certificate draft view confirmaion because we were told to remove it.
                    //_requestApprovalHandler.Value.ConfirmAdminHasViewedDraftDocument(requestStage[1], $"0{model.PhoneNumber.Substring(PhoneTrimStartIndex)}");
                    return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = "Enter Comment (Minimum of ten characters)\n" };
            }

            if(requestStage.Length > CrossReferenceStage)
            {
                string approvalResponse = ProcessApproval(model);
                ObjectCacheProvider.RemoveCachedObject(_orchardServices.WorkContext.CurrentSite.SiteName, $"USSD-{nameof(POSSAPCachePrefix.FileNumber)}-{requestStage[1]}");
                return new USSDAPIResponse { IsFinalStage = true, StatusCode = HttpStatusCode.OK, ResponseObject = approvalResponse };
            }
            throw new DirtyFormDataException("Invalid request, please try again.");
        }

        /// <summary>
        /// Process the request approval
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string ProcessApproval(USSDRequestModel model, bool isDiaryAndIncidentDateUpdated = false)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                string[] requestStage = model.Text.Split('|');
                bool parsed = int.TryParse(requestStage[2], out int operationTypeId);
                if (!parsed)
                {
                    throw new DirtyFormDataException("Invalid input, please try again.");
                }

                string comment = requestStage[requestStage.Length - 1];
                RequestApprovalResponse approvalResponse = null;
                if (operationTypeId == (int)USSDOperationType.Decline)
                {
                    approvalResponse = _requestApprovalHandler.Value.ProcessRequestRejection(requestStage[1], ref errors, comment, model.PhoneNumber);
                    return $"{approvalResponse.NotificationMessage}\n";
                }

                string content = _extractDetailManager.Value.GetExtractContentDetails(requestStage[1]);
                if (string.IsNullOrEmpty(content))
                {
                    Node node = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName).Node.Where(x => x.Key == PSSTenantConfigKeys.SMSShortCode.ToString()).FirstOrDefault();
                    if (node != null && !string.IsNullOrEmpty(node.Value))
                    {
                        throw new NoRecordFoundException($"Please send the Contents of the extract (minimum of 10 and maximum of 1000 characters) via SMS to {node.Value} before you continue with this process.");
                    }

                    throw new NoRecordFoundException($"Please send the Contents of the extract (minimum of 10 and maximum of 1000 characters) via SMS to the provided short code before you continue with this process.");
                }

                ExtractRequestDetailsVM detailsVM = null;
                if (isDiaryAndIncidentDateUpdated)
                {
                    PoliceOfficerVM officerVM = _typeImplComposer.Value.GetPoliceOfficer(requestStage[3]);
                    detailsVM = new ExtractRequestDetailsVM
                    {
                        Comment = comment,
                        SelectedDPO = new List<ProposedEscortOffficerVM> { new ProposedEscortOffficerVM { PoliceOfficerLogId = officerVM.PoliceOfficerLogId } }
                    };
                }
                else
                {
                    string[] incidentDateTime = requestStage[4].Split(' ');
                    detailsVM = new ExtractRequestDetailsVM
                    {
                        Comment = comment,
                        Content = content,
                        DiarySerialNumber = requestStage[3],
                        IncidentDate = incidentDateTime[0],
                        IncidentTime = incidentDateTime[1],
                        CrossReferencing = requestStage[5]
                    };
                }
                approvalResponse = _requestApprovalHandler.Value.ProcessRequestApproval(requestStage[1], ref errors, detailsVM, model.PhoneNumber);
                return $"{approvalResponse.NotificationMessage}\n";
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This confirms that the incident date and time entered is valid date and time. It also check that the date is not in the future 
        /// </summary>
        /// <param name="incidentDateTime"></param>
        /// <returns>string</returns>
        private string ParseIncidentDate(string incidentDateTime)
        {
            try
            {
              DateTime incidentDate =  DateTime.ParseExact(incidentDateTime.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                if (incidentDate > DateTime.Now)
                {
                    throw new DirtyFormDataException($"Incident date cannot be in the future. value {incidentDate.ToString("dd/MM/yyyy HH:mm")}.");
                }
                return "Enter Cross Reference (Optional)\n";
            }
            catch (Exception)
            {
                throw new DirtyFormDataException("Invalid Incident Date and Time. It must be in format dd/mm/yyyy HH:mm.");
            }
        }

        /// <summary>
        /// Validate that the approver can approve for the request command
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <param name="phoneNumber"></param>
        /// <param name="fileNumber"></param>
        public void ValidateApproverCommand(int flowDefinitionLevelId, string phoneNumber, string fileNumber)
        {
            if(_serviceRequestFlowApprover.Value.GetCommandIdForApproverOfDefinitionLevel(flowDefinitionLevelId, phoneNumber) != _requestCommandWorkFlowLogManager.Value.GetRequestCommandId(fileNumber, flowDefinitionLevelId))
            {
                throw new UserNotAuthorizedForThisActionException("User not authorized to approve this request.");
            }
        }
    }
}