using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DataFilters.Requests.Contracts;
using Parkway.CBS.Police.Core.DataFilters.Requests.SearchFilters;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts;
using Parkway.CBS.Police.Core.PSSServiceType.Rejection.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Net;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class RequestApprovalHandler : IRequestApprovalHandler
    {
        private readonly Lazy<IPoliceRankingManager<PoliceRanking>> _policeRankingManager;

        private readonly IEnumerable<Lazy<IPSSServiceTypeApprovalImpl>> _serviceTypeApprovalImpl;

        private readonly Lazy<IPSSRequestManager<PSSRequest>> _requestManager;

        private readonly Lazy<IRejectionComposition> _rejectionComposition;

        private readonly Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> _approvalAccesRoleManager;

        private readonly IPoliceRequestFilter _requestFilter;

        private readonly Lazy<IInvoiceFilter> _invoiceFilter;

        private readonly Lazy<IPSServiceManager<PSService>> _serviceManager;

        private readonly Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> _serviceRequestFlowApprover;


        public ILogger Logger { get; set; }

        public RequestApprovalHandler(IEnumerable<Lazy<IPSSServiceTypeApprovalImpl>> serviceTypeApprovalImpl, Lazy<IPoliceRankingManager<PoliceRanking>> policeRankingManager, Lazy<IPSSRequestManager<PSSRequest>> requestManager, Lazy<IRejectionComposition> rejectionComposition, Lazy<IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>> approvalAccesRoleManager, IPoliceRequestFilter requestFilter, Lazy<IInvoiceFilter> invoiceFilter, Lazy<IPSServiceManager<PSService>> serviceManager, Lazy<IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>> serviceRequestFlowApprover)
        {
            _serviceTypeApprovalImpl = serviceTypeApprovalImpl;
            _policeRankingManager = policeRankingManager;
            Logger = NullLogger.Instance;
            _requestManager = requestManager;
            _rejectionComposition = rejectionComposition;
            _approvalAccesRoleManager = approvalAccesRoleManager;
            _requestFilter = requestFilter;
            _serviceManager = serviceManager;
            _invoiceFilter = invoiceFilter;
            _serviceRequestFlowApprover = serviceRequestFlowApprover;
        }



        /// <summary>
        /// Get police rank details using the rank id
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPoliceRank(int rankId)
        {
            try
            {
                var response = _policeRankingManager.Value.GetPoliceRank(rankId);
                return new APIResponse { Error = false, StatusCode = HttpStatusCode.OK, ResponseObject = response };
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex}");
                return new APIResponse { Error = true, StatusCode = HttpStatusCode.BadRequest, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }



        /// <summary>
        /// Get the Extract and Escort view details using request id and service type id,
        /// It returns dynamic object
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="serviceTypeId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetailsForView(long requestId)
        {
            int serviceType = _requestManager.Value.GetServiceType(requestId);

            foreach (var impl in _serviceTypeApprovalImpl)
            {
                if ((PSSServiceTypeDefinition)serviceType == impl.Value.GetServiceTypeDefinition)
                {
                    return impl.Value.GetServiceRequestViewDetails(requestId);
                }
            }
            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id " + serviceType);
        }


        /// <summary>
        /// Get the view details for approval using request id and service type id,
        /// It returns dynamic object
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="adminUserId"></param>
        /// <returns>PSSRequestDetailsVM</returns>
        public PSSRequestDetailsVM GetServiceRequestViewDetailsForApproval(long requestId, int adminUserId)
        {
            int serviceType = _requestManager.Value.GetServiceType(requestId);

            foreach (var impl in _serviceTypeApprovalImpl)
            {
                if ((PSSServiceTypeDefinition)serviceType == impl.Value.GetServiceTypeDefinition)
                {
                    return impl.Value.GetServiceRequestViewDetailsForApproval(requestId, adminUserId);
                }
            }
            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + serviceType);
        }



        /// <summary>
        /// Process request approval details
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>RequestApprovalResponse</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException">Throw this exception if the user is not an assigned user</exception>
        public RequestApprovalResponse ProcessRequestApproval(long requestId, ref List<ErrorModel> errors, dynamic userInput)
        {
            GenericRequestDetails requestDetails = _requestManager.Value.GetRequestDetails(requestId);
            if (requestDetails == null) { throw new NoRecordFoundException("No record found for request " + requestId); }

            //check admin user can approve this request
            bool adminCanApproveRequest = _serviceRequestFlowApprover.Value.UserIsValidApproverForDefinitionLevel(userInput.ApproverId, requestDetails.FlowDefinitionLevelId);
            if (!adminCanApproveRequest)
            {
                throw new UserNotAuthorizedForThisActionException();
            }

            requestDetails.ApproverComment = userInput.Comment;
            if (string.IsNullOrEmpty(requestDetails.ApproverComment))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment field is required", FieldName = "Comment" });
                throw new DirtyFormDataException("Comment field is empty for apporval");
            }

            if (requestDetails.ApproverComment.Trim().Length < 10)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment requires at least 10 characters", FieldName = "Comment" });
                throw new DirtyFormDataException("Comment requires at least 10 characters");
            }

            foreach (var impl in _serviceTypeApprovalImpl)
            {
                if (impl.Value.GetServiceTypeDefinition == (PSSServiceTypeDefinition)requestDetails.ServiceTypeId)
                {
                    return impl.Value.ValidatedAndProcessRequestApproval(requestDetails, ref errors, userInput);
                }
            }

            throw new NoBillingTypeSpecifiedException("Could not find service type implementation. Type Id" + requestId);
        }



        /// <summary>
        /// Do process for request rejection
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errors"></param>
        /// <param name="comment"></param>
        /// <param name="id"></param>
        /// <returns>RequestApprovalResponse</returns>
        public RequestApprovalResponse ProcessRequestRejection(long requestId, ref List<ErrorModel> errors, string comment, int adminUserId)
        {
            GenericRequestDetails requestDetails = _requestManager.Value.GetRequestDetails(requestId);
            if (requestDetails == null) { throw new NoRecordFoundException("No record found for request " + requestId); }

            //check admin user can approve this request
            bool adminCanApproveRequest = _serviceRequestFlowApprover.Value.UserIsValidApproverForDefinitionLevel(adminUserId, requestDetails.FlowDefinitionLevelId);
            if (!adminCanApproveRequest)
            {
                throw new UserNotAuthorizedForThisActionException();
            }

            requestDetails.ApproverComment = comment;
            if (string.IsNullOrEmpty(requestDetails.ApproverComment))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment field is required", FieldName = "Comment" });
                throw new DirtyFormDataException("Comment field is empty for apporval");
            }

            if (requestDetails.ApproverComment.Trim().Length < 10)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment requires atleast 10 characters", FieldName = "Comment" });
                throw new DirtyFormDataException("Comment requires atleast 10 characters");
            }

            return _rejectionComposition.Value.ProcessRequestRejection(requestDetails, adminUserId);
        }
    }
}