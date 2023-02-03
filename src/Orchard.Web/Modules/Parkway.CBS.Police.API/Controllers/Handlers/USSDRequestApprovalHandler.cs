using Orchard;
using Orchard.Logging;
using Parkway.CBS.CacheProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Rejection.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.API.Controllers.Handlers
{
    public class USSDRequestApprovalHandler : IUSSDRequestApprovalHandler
    {
        private readonly IEnumerable<Lazy<IPSSServiceTypeApprovalImpl>> _serviceTypeApprovalImpl;
        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;
        private readonly Lazy<IRejectionComposition> _rejectionComposition;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _flowDefinitionLevelManager;
        private readonly Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> _escortSettingsManager;
        public ILogger Logger { get; set; }
        private readonly IEnumerable<IPSSServiceTypeDocumentPreviewImpl> _pssServiceTypeDocumentPreviewImpl;
        private readonly IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> _pssRequestApprovalDocumentPreviewLogManager;
        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;
        private readonly IOrchardServices _orchardServices;


        public USSDRequestApprovalHandler(IEnumerable<Lazy<IPSSServiceTypeApprovalImpl>> serviceTypeApprovalImpl,Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<IRejectionComposition> rejectionComposition, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> flowDefinitionLevelManager, Lazy<IPSSEscortSettingsManager<PSSEscortSettings>> escortSettingsManager, IEnumerable<IPSSServiceTypeDocumentPreviewImpl> pssServiceTypeDocumentPreviewImpl, IPSSRequestApprovalDocumentPreviewLogManager<PSSRequestApprovalDocumentPreviewLog> pssRequestApprovalDocumentPreviewLogManager, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover, IOrchardServices orchardServices)
        {
            _serviceTypeApprovalImpl = serviceTypeApprovalImpl;
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _rejectionComposition = rejectionComposition;
            _flowDefinitionLevelManager = flowDefinitionLevelManager;
            _escortSettingsManager = escortSettingsManager;
            _pssServiceTypeDocumentPreviewImpl = pssServiceTypeDocumentPreviewImpl;
            _pssRequestApprovalDocumentPreviewLogManager = pssRequestApprovalDocumentPreviewLogManager;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Process request approval details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="sRequestFormDump"></param>
        /// <returns>RequestApprovalResponse</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException">Throw this exception if the user is not an assigned user</exception>
        public RequestApprovalResponse ProcessRequestApproval(string fileNumber, ref List<ErrorModel> errors, dynamic userInput, string phoneNumber)
        {
            GenericRequestDetails requestDetails = _requestManager.Value.GetRequestDetailsByFileNumber(fileNumber);
            if (requestDetails == null) { throw new NoRecordFoundException("No record found for request " + fileNumber); }

            //check admin user can approve this request
            bool adminCanApproveRequest = _serviceRequestFlowApprover.Value.UserIsValidApproverForDefinitionLevel(phoneNumber, requestDetails.FlowDefinitionLevelId);
            if (!adminCanApproveRequest)
            {
                throw new UserNotAuthorizedForThisActionException("User not authorized to perfrom this action.");
            }

            int adminUserId = _flowDefinitionLevelManager.GetAssignedApproverId(phoneNumber, requestDetails.FlowDefinitionLevelId);
            ObjectCacheProvider.TryCache(_orchardServices.WorkContext.CurrentSite.SiteName, $"USSD-Admin-{fileNumber.ToUpper()}", new PSSRequestDetailsVM { ApproverId = adminUserId }, DateTime.Now.AddMinutes(5));

            requestDetails.FlowDefinitionLevelId = requestDetails.FlowDefinitionLevelId;
            requestDetails.ApproverComment = userInput.Comment;
            userInput.ApproverId = adminUserId;
            if (string.IsNullOrEmpty(requestDetails.ApproverComment))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment field is required", FieldName = "Comment" });
                throw new DirtyFormDataException("Comment field is empty for apporval");
            }

            if (requestDetails.ApproverComment.Length < 10)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment requires atleast 10 characters", FieldName = "Comment" });
                throw new DirtyFormDataException("Comment requires atleast 10 characters");
            }

            foreach (var impl in _serviceTypeApprovalImpl)
            {
                if (impl.Value.GetServiceTypeDefinition == (PSSServiceTypeDefinition)requestDetails.ServiceTypeId)
                {
                    return impl.Value.ValidatedAndProcessRequestApproval(requestDetails, ref errors, userInput);
                }
            }

            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. File number" + fileNumber);
        }
        

        /// <summary>
        /// Do process for request rejection
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errors"></param>
        /// <param name="comment"></param>
        /// <param name="id"></param>
        /// <returns>RequestApprovalResponse</returns>
        public RequestApprovalResponse ProcessRequestRejection(string fileNumber, ref List<ErrorModel> errors, string comment, string phoneNumber)
        {
            
            GenericRequestDetails requestDetails = _requestManager.Value.GetRequestDetailsByFileNumber(fileNumber);
            if (requestDetails == null) { throw new NoRecordFoundException("No record found for request " + fileNumber); }

            int adminUserId = _flowDefinitionLevelManager.GetAssignedApproverId(phoneNumber, requestDetails.ServiceRequests.Last().FlowDefinitionLevelId);

            //check admin user can approve this request
            bool adminCanApproveRequest = _serviceRequestFlowApprover.Value.UserIsValidApproverForDefinitionLevel(phoneNumber, requestDetails.FlowDefinitionLevelId);
            if (!adminCanApproveRequest)
            {
                throw new UserNotAuthorizedForThisActionException();
            }

            requestDetails.ApproverComment = comment;
            if (string.IsNullOrEmpty(requestDetails.ApproverComment))
            {
                throw new DirtyFormDataException("Comment field is empty for approval");
            }

            if (requestDetails.ApproverComment.Length < 10)
            {
                throw new DirtyFormDataException("Comment requires atleast 10 characters");
            }

            return _rejectionComposition.Value.ProcessRequestRejection(requestDetails, adminUserId);
        }

        /// <summary>
        /// Check if admin can assign officers
        /// </summary>
        /// <param name="fileNumber"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>bool</returns>
        public bool CanAdminAssignOfficers(string fileNumber, string phoneNumber)
        {
            try
            {
                GenericRequestDetails requestDetails = _requestManager.Value.GetRequestDetailsByFileNumber(fileNumber);
                if (requestDetails == null) { throw new NoRecordFoundException("No record found for request " + fileNumber); }

                int adminUserId = _flowDefinitionLevelManager.GetAssignedApproverId(phoneNumber, requestDetails.ServiceRequests.Last().FlowDefinitionLevelId);

                return _escortSettingsManager.Value.CanAdminAssignOfficers(adminUserId, requestDetails.FlowDefinitionId);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Confirms the admin user has viewed the service specific draft document during approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public void ConfirmAdminHasViewedDraftDocument(string fileRefNumber, string phoneNumber)
        {
            try
            {
                GenericRequestDetails requestDetails = _requestManager.Value.GetRequestDetailsByFileNumber(fileRefNumber);
                if (requestDetails == null) { throw new NoRecordFoundException("No record found for request " + fileRefNumber); }

                int adminUserId = _flowDefinitionLevelManager.GetAssignedApproverId(phoneNumber, requestDetails.ServiceRequests.Last().FlowDefinitionLevelId);

                //check admin user can approve this request
                bool adminCanApproveRequest = _serviceRequestFlowApprover.Value.UserIsValidApproverForDefinitionLevel(phoneNumber, requestDetails.FlowDefinitionLevelId);
                if (!adminCanApproveRequest)
                {
                    throw new UserNotAuthorizedForThisActionException();
                }

                CreateCertificateDocumentVM serviceDraftDocument = null;
                if (string.IsNullOrEmpty(fileRefNumber) || fileRefNumber.Trim().Length == 0) { throw new Exception("File ref number not specified"); }
                var request = _requestManager.Value.GetRequestDetails(fileRefNumber);
                if (request == null) { throw new Exception($"404 no request found for file ref number {fileRefNumber}"); }
                if (_pssRequestApprovalDocumentPreviewLogManager.Count(x => x.Request == new PSSRequest { Id = request.Id } && x.Approver == new Orchard.Users.Models.UserPartRecord { Id = adminUserId } && x.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = request.FlowDefinitionLevelId } && x.Confirmed) < 1)
                {
                    //This means that the approver has not confirmed that he/she has viewed the draft document for this level
                    foreach (var impl in _pssServiceTypeDocumentPreviewImpl)
                    {
                        if (impl.GetServiceTypeDefinition == (PSSServiceTypeDefinition)request.ServiceTypeId)
                        {
                            serviceDraftDocument = impl.CreateDraftServiceDocumentByteFile(fileRefNumber);
                        }
                    }

                    if (serviceDraftDocument == null) { throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + request.ServiceTypeId); }

                    PSSRequestApprovalDocumentPreviewLog documentPreviewLog = new PSSRequestApprovalDocumentPreviewLog
                    {
                        Request = new PSSRequest { Id = request.Id },
                        Approver = new Orchard.Users.Models.UserPartRecord { Id = adminUserId },
                        FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevel { Id = request.FlowDefinitionLevelId },
                        RequestDocumentDraftBlob = Convert.ToBase64String(serviceDraftDocument.DocByte),
                        Confirmed = true,
                    };

                    if (!_pssRequestApprovalDocumentPreviewLogManager.Save(documentPreviewLog)) { throw new CouldNotSaveRecord(); }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _pssRequestApprovalDocumentPreviewLogManager.RollBackAllTransactions();
                throw;
            }
        }

    }
}