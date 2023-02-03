using Orchard.Logging;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.RequestWorkFlow.Actions.ApprovalImpl.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Parkway.CBS.Police.Core.RequestWorkFlow.Actions.ApprovalImpl
{
    public class ApprovalEscort : IServiceApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.Escort;
        private readonly ITypeImplComposer _typeImpl;
        private readonly IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> _requestFlowApproverManager;
        private readonly IPSSEscortDetailsManager<PSSEscortDetails> _escortDetailsManager;
        private readonly ICoreCommand _coreCommand;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _pSServiceRequestFlowDefinitionLevelManager;
        public ILogger Logger { get; set; }

        public ApprovalEscort(ITypeImplComposer typeImpl, IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> requestFlowApproverManager, IPSSEscortDetailsManager<PSSEscortDetails> escortDetailsManager, ICoreCommand coreCommand, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> pSServiceRequestFlowDefinitionLevelManager)
        {
            Logger = NullLogger.Instance;
            _typeImpl = typeImpl;
            _requestFlowApproverManager = requestFlowApproverManager;
            _escortDetailsManager = escortDetailsManager;
            _coreCommand = coreCommand;
            _pSServiceRequestFlowDefinitionLevelManager = pSServiceRequestFlowDefinitionLevelManager;
        }

        /// <summary>
        /// Do extra work specific to a particular service
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="nextDefinedLevel"></param>
        public void DoServiceImplementationWorkForApproval(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel)
        {
                //EscortDetailsDTO escortDetails = _escortDetailsManager.GetEscortDetails(requestDeets.First().Request.Id);
                //if (escortDetails.CommandTypeId == (int)CommandTypeId.Conventional)
                //{
                //    MoveToCPForConventional(requestDeets, nextDefinedLevel, escortDetails);
                //}
                //else if (escortDetails.CommandTypeId == (int)CommandTypeId.Tactical)
                //{
                //    MoveToIGForTactical(requestDeets, nextDefinedLevel, escortDetails);
                //}

            MoveToNextApproverInFlowDefinition(requestDeets, nextDefinedLevel);
            //Notify the next person assigned to approve the request
            //List<NotificationInfoVM> approvers = _requestFlowApproverManager.GetRequestApproversInfo(requestDeets.First().Request.Id, nextDefinedLevel.Id);
            //SendSMSNotification(requestDeets.First().Request.FileRefNumber, approvers, requestDeets.First().ServiceName);
        }

        /// <summary>
        /// Send SMS notification
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="approvers"></param>
        /// <param name="serviceName"></param>
        private void SendSMSNotification(string fileNumber, List<NotificationInfoVM> approvers, string serviceName)
        {
            foreach (NotificationInfoVM approver in approvers)
            {
                dynamic smsDetails = new ExpandoObject();
                smsDetails.RequestType = serviceName;
                smsDetails.FileNumber = fileNumber;
                smsDetails.Name = approver.Name;
                _typeImpl.SendApproverSMSNotification(smsDetails, new List<string> { approver.PhoneNumber });
            }
        }

        /// <summary>
        /// This creates a request command workflow log record using the state command of the selected origin state if escort service or the service delivery state to allow the request to be directed to the CP of that state command, this only applies to conventional escort & guards requests
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="nextDefinedLevel"></param>
        private void MoveToCPForConventional(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel, EscortDetailsDTO escortDetails)
        {
            int stateCommandId = 0;
            if (escortDetails.OriginStateId > 0) { stateCommandId = _coreCommand.GetStateCommand(escortDetails.OriginStateId).Id; }
            else { stateCommandId = _coreCommand.GetStateCommand(escortDetails.StateId).Id; }

            requestDeets.First().DefinitionLevelId = nextDefinedLevel.Id;
            //set the previous request command workflow entry(which takes place in the approval composition before this method is called) for the next defined level to false since it's being overridden
            _typeImpl.UpdateRequestCommandWorkFlowLog(requestDeets);

            _typeImpl.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog
            {
                Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                Command = new Command { Id = stateCommandId },
                DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                IsActive = true,
                RequestPhaseId = (int)RequestPhase.New,
                RequestPhaseName = nameof(RequestPhase.New)
            });
        }


        private void MoveToIGForTactical(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel, EscortDetailsDTO escortDetails)
        {
            int officeOfIGPCommand = _coreCommand.GetIGPOfficeCommand().Id;

            requestDeets.First().DefinitionLevelId = nextDefinedLevel.Id;
            //set the previous request command workflow entry(which takes place in the approval composition before this method is called) for the next defined level to false since it's being overridden
            _typeImpl.UpdateRequestCommandWorkFlowLog(requestDeets);

            _typeImpl.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog
            {
                Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                Command = new Command { Id = officeOfIGPCommand },
                DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                IsActive = true,
                RequestPhaseId = (int)RequestPhase.New,
                RequestPhaseName = nameof(RequestPhase.New)
            });
        }


        private void MoveToNextApproverInFlowDefinition(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel)
        {
            _typeImpl.SetPreviousRequestCommandWorkflowLogsToInactive(requestDeets.First().Request.Id);

            int nextApproverCommand = _requestFlowApproverManager.GetCommandIdForApproverOfDefinitionLevel(nextDefinedLevel.Id);

            requestDeets.First().DefinitionLevelId = nextDefinedLevel.Id;

            _typeImpl.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog
            {
                Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                Command = new Command { Id = nextApproverCommand },
                DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                IsActive = true,
                RequestPhaseId = (int)RequestPhase.New,
                RequestPhaseName = nameof(RequestPhase.New)
            });
        }

    }
}