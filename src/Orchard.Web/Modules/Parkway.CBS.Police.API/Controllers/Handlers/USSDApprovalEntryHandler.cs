using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Exceptions;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class USSDApprovalEntryHandler : IUSSDRequestTypeHandler
    {
        public ILogger Logger { get; set; }

        public USSDRequestType GetRequestType => USSDRequestType.Approval;

        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly IEnumerable<Lazy<IPSSServiceTypeUSSDApprovalImpl>> _pssServiceTypeUSSDApprovalImpl;
        private readonly IEnumerable<Lazy<IPSSServiceTypeUSSDApprovalValidatorImpl>> _pssServiceTypeUSSDApprovalValidatorImpl;
        private readonly Lazy<ITypeImplComposer> _typeImplComposer;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;

        public USSDApprovalEntryHandler(Lazy<IPSSRequestManager<PSSRequest>> requestManager, IEnumerable<Lazy<IPSSServiceTypeUSSDApprovalImpl>> pssServiceTypeUSSDApprovalImpl, Lazy<ITypeImplComposer> typeImplComposer, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover, IEnumerable<Lazy<IPSSServiceTypeUSSDApprovalValidatorImpl>> pssServiceTypeUSSDApprovalValidatorImpl)
        {
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _pssServiceTypeUSSDApprovalImpl = pssServiceTypeUSSDApprovalImpl;
            _typeImplComposer = typeImplComposer;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
            _pssServiceTypeUSSDApprovalValidatorImpl = pssServiceTypeUSSDApprovalValidatorImpl;
        }

        /// <summary>
        /// Start USSD approval request processing
        /// </summary>
        /// <param name="model"></param>
        /// <returns>USSDAPIResponse</returns>
        public USSDAPIResponse StartRequest(USSDRequestModel model)
        {
            try
            {
                string[] requestStage = model.Text.Split('|');
                switch (requestStage.Length)
                {
                    case (int)USSDProcessingStage.RequestType:
                        return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = FileNumberMenu() };

                    case (int)USSDProcessingStage.FileNumber:
                        return new USSDAPIResponse { StatusCode = HttpStatusCode.OK, ResponseObject = FileNumberStage(requestStage[1], model.PhoneNumber) };
                }

                //This stage is dynamic
                //Case of extract it can be comment stage or enter diary number stage
                //Case of character certificate it can be comment stage of enter reference number stage
                //Case of guard service (escort), it can be comment stage or officer input page
                if (requestStage.Length > (int)USSDProcessingStage.FileNumber)
                {
                    PSSRequestVM requestDet = _typeImplComposer.Value.ConfirmFileNumber(requestStage[1]);
                    if(requestDet == null)
                    {
                        throw new NoRecordFoundException("Request with the specified File Number not valid.");
                    }

                    if (requestDet.Status != PSSRequestStatus.PendingApproval)
                    {
                        throw new PSSRequestNotPendingApprovalException("Request with the specified File Number not pending approval.");
                    }

                    foreach (var impl in _pssServiceTypeUSSDApprovalValidatorImpl)
                    {
                        if ((PSSServiceTypeDefinition)requestDet.ServiceTypeId == impl.Value.GetServiceTypeDefinition)
                        {
                            impl.Value.ValidateApproverCommand(requestDet.FlowDefinitionLevelId, model.PhoneNumber, requestDet.FileRefNumber);
                        }
                    }

                    foreach (var impl in _pssServiceTypeUSSDApprovalImpl)
                    {
                        if ((PSSServiceTypeDefinition)requestDet.ServiceTypeId == impl.Value.GetServiceTypeDefinition)
                        {
                            return impl.Value.ProcessRequest(model);
                        }
                    }
                }
                throw new DirtyFormDataException("Invalid input, please try again.");
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Prompt user to enter file number
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        private string FileNumberMenu()
        {
            return "Enter File Number\n"; ;
        }

        /// <summary>
        /// Prompt user to select action to be performed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private string FileNumberStage(string fileNumber, string phoneNumber)
        {
            try
            {
                PSSRequestVM requestDet = _typeImplComposer.Value.ConfirmFileNumber(fileNumber);
                if(requestDet == null)
                {
                    throw new NoRecordFoundException("Request with the specified File Number not valid.");
                }

                if (requestDet.Status != PSSRequestStatus.PendingApproval)
                {
                    throw new PSSRequestNotPendingApprovalException("Request with the specified File Number not pending approval.");
                }

                bool canApproveRequest = _serviceRequestFlowApprover.Value.UserIsValidApproverForDefinitionLevel(phoneNumber, requestDet.FlowDefinitionLevelId);
                if (!canApproveRequest)
                {
                    throw new UserNotAuthorizedForThisActionException("User not authorized to approve this request.");
                }

                foreach (var impl in _pssServiceTypeUSSDApprovalValidatorImpl)
                {
                    if ((PSSServiceTypeDefinition)requestDet.ServiceTypeId == impl.Value.GetServiceTypeDefinition)
                    {
                        impl.Value.ValidateApproverCommand(requestDet.FlowDefinitionLevelId, phoneNumber, requestDet.FileRefNumber);
                    }
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("Please select action below\n");
                sb.Append("\n");
                sb.Append("1.Approve Request\n");
                sb.Append("2.Reject Request\n");
                return sb.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}