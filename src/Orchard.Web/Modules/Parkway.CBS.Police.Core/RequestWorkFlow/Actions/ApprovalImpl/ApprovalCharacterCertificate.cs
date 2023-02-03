using Orchard.Logging;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.ApprovalImpl.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Orchard;
using System;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.ApprovalImpl
{
    public class ApprovalCharacterCertificate : IServiceApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.CharacterCertificate;
        private readonly ITypeImplComposer _typeImpl;
        private readonly IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> _requestFlowApproverManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _pSServiceRequestFlowDefinitionLevelManager;
        private readonly ICoreCharacterCertificateService _coreCharacterCertificateService;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _flowDefinitionLevelManager;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;


        public ApprovalCharacterCertificate(IOrchardServices orchardServices, ITypeImplComposer typeImpl, IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> requestFlowApproverManager, ICoreCharacterCertificateService coreCharacterCertificateService, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> pSServiceRequestFlowDefinitionLevelManager, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> flowDefinitionLevelManager)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _typeImpl = typeImpl;
            _requestFlowApproverManager = requestFlowApproverManager;
            _coreCharacterCertificateService = coreCharacterCertificateService;
            _pSServiceRequestFlowDefinitionLevelManager = pSServiceRequestFlowDefinitionLevelManager;
            _flowDefinitionLevelManager = flowDefinitionLevelManager;
        }

        /// <summary>
        /// Do extra work specific to a particular service
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="nextDefinedLevel"></param>
        public void DoServiceImplementationWorkForApproval(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel)
        {
            MoveToNextApproverInFlowDefinition(requestDeets, nextDefinedLevel);

            //Notify the next person assigned to approve the request
            if (nextDefinedLevel.ApprovalButtonName == "Invite For Capture")
            {
                //Notify the applicant to go for capture
                StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.PCCBiometricCaptureDueDay.ToString()).FirstOrDefault();
                if (node != null && !string.IsNullOrEmpty(node.Value))
                {
                    bool result = double.TryParse(node.Value, out double captureDueDay);
                    if (!result)
                    {
                        throw new Exception($"Unable to get PCC biometric capture due date for filenumber {requestDeets.First().Request.FileRefNumber}");
                    }

                    //Update biometric capture info, add a day to the due date to be able to set the time to 11:59:59PM on the deadline date
                    captureDueDay += 1;
                    DateTime captureDueDate = DateTime.Now.AddDays(captureDueDay).Date.AddMilliseconds(-1);
                    _coreCharacterCertificateService.UpdateApplicantBiometricInvitationDetails(requestDeets.First().Request.Id, captureDueDate);

                    string captureDueDateString = captureDueDate.ToString("dd/MM/yyyy");
                    //List<NotificationInfoVM> approvers = _requestFlowApproverManager.GetRequestApproversInfo(requestDeets.First().Request.Id, nextDefinedLevel.Id);
                    //NotifyStateCIDForScheduledBiometricCapture(requestDeets.First(), captureDueDateString, approvers);
                    InviteApplicantForBiometricCapture(requestDeets.First(), captureDueDateString);
                }
            }
            else
            {
                //List<NotificationInfoVM> approvers = _requestFlowApproverManager.GetRequestApproversInfo(requestDeets.First().Request.Id, nextDefinedLevel.Id);
                //SendSMSNotification(requestDeets.First().Request.FileRefNumber, approvers, requestDeets.First().ServiceName);
            }
        }

        /// <summary>
        /// Send SMS notification
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="phoneNumbers"></param>
        /// <param name="serviceName"></param>
        private void SendSMSNotification(string fileNumber, List<NotificationInfoVM> approvers, string serviceName)
        {
            foreach(NotificationInfoVM approver in approvers)
            {
                dynamic smsDetails = new ExpandoObject();
                smsDetails.RequestType = serviceName;
                smsDetails.FileNumber = fileNumber;
                smsDetails.Name = approver.Name;
                _typeImpl.SendApproverSMSNotification(smsDetails, new List<string> { approver.PhoneNumber });
            }
        }

        /// <summary>
        /// Send SMS to an applicant to go for biometric capture
        /// </summary>
        /// <param name="requestDeets"></param>
        private void InviteApplicantForBiometricCapture(PSServiceRequestInvoiceValidationDTO requestDeets, string captureDueDate)
        {
            string message = $"Dear {requestDeets.Recipient}, please proceed to ({requestDeets.CommandName}) for biometric capture in respect of application number {requestDeets.Request.FileRefNumber}. Address: {requestDeets.CommandAddress}. Capture expiry date : {captureDueDate}. Call 018884040 for enquiries.";
            _typeImpl.SendGenericSMSNotification(new List<string> { requestDeets.PhoneNumber }, message);
        }

        /// <summary>
        /// Send SMS to state CID about schedule biometric capture
        /// </summary>
        /// <param name="requestDeets"></param>
        private void NotifyStateCIDForScheduledBiometricCapture(PSServiceRequestInvoiceValidationDTO requestDeets, string captureDueDate, List<NotificationInfoVM> approvers)
        {
            foreach (NotificationInfoVM approver in approvers)
            {
                string message = $"Dear {approver.Name}, please be informed that an applicant with file number {requestDeets.Request.FileRefNumber} has been scheduled for biometric capture between now and {captureDueDate}.";
                _typeImpl.SendGenericSMSNotification(new List<string> { approver.PhoneNumber }, message);
            }
        }

        /// <summary>
        /// Moves the request to the next approver in the flow definition level
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="nextDefinedLevel"></param>
        private void MoveToNextApproverInFlowDefinition(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel)
        {
            _typeImpl.SetPreviousRequestCommandWorkflowLogsToInactive(requestDeets.First().Request.Id);

            //This is to check if the next stage is State CID level, this will help route the request to the appropriate state
            int nextApproverCommand;
            if (nextDefinedLevel.ApprovalButtonName == "Invite For Capture")
            {
                nextApproverCommand = requestDeets.First().CommandId;
            }
            else
            {
                nextApproverCommand = _requestFlowApproverManager.GetCommandIdForApproverOfDefinitionLevel(nextDefinedLevel.Id);
            }
            requestDeets.First().DefinitionLevelId = nextDefinedLevel.Id;

            //We are checking if the next approver is the last approver for this request so that we can set request phase to ongoing in the case of 
            //when the request is going back to the CPCCR for the final approval
            bool isServiceLastApprover = _flowDefinitionLevelManager.CheckIfThisIsLastApprover(nextDefinedLevel.DefinitionId, nextDefinedLevel.Position);
            _typeImpl.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog
            {
                Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                Command = new Command { Id = nextApproverCommand },
                DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                IsActive = true,
                RequestPhaseId = isServiceLastApprover ? (int)RequestPhase.Ongoing : (int)RequestPhase.New,
                RequestPhaseName = isServiceLastApprover ? nameof(RequestPhase.Ongoing) : nameof(RequestPhase.New)
            });
        }
    }
}