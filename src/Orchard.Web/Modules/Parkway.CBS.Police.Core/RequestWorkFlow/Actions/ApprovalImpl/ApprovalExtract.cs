using Orchard.Logging;
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
    public class ApprovalExtract : IServiceApprovalImpl
    {
        public PSSServiceTypeDefinition GetServiceType => PSSServiceTypeDefinition.Extract;
        private readonly ITypeImplComposer _typeImpl;
        private readonly IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> _requestFlowApproverManager;
        public ILogger Logger { get; set; }

        public ApprovalExtract(ITypeImplComposer typeImpl, IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> requestFlowApproverManager)
        {
            Logger = NullLogger.Instance;
            _typeImpl = typeImpl;
            _requestFlowApproverManager = requestFlowApproverManager;
        }

        /// <summary>
        /// Do extra work specific to a particular service
        /// </summary>
        /// <param name="requestDeets"></param>
        /// <param name="nextDefinedLevel"></param>
        public void DoServiceImplementationWorkForApproval(IEnumerable<PSServiceRequestInvoiceValidationDTO> requestDeets, PSServiceRequestFlowDefinitionLevelDTO nextDefinedLevel)
        {
            //update request command workflow log
            _typeImpl.SetPreviousRequestCommandWorkflowLogsToInactive(requestDeets.First().Request.Id);

            //add request command workflow log
            _typeImpl.AddRequestCommandWorkFlowLog(new RequestCommandWorkFlowLog
            {
                Request = new PSSRequest { Id = requestDeets.First().Request.Id },
                Command = new Command { Id = requestDeets.First().CommandId },
                DefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = nextDefinedLevel.Id },
                RequestPhaseId = (int)RequestPhase.New,
                RequestPhaseName = nameof(RequestPhase.New),
                IsActive = true
            });

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

    }
}