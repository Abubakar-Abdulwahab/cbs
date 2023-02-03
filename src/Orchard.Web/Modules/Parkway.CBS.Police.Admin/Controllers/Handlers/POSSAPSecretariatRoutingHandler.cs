using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers
{
    public class POSSAPSecretariatRoutingHandler : IPOSSAPSecretariatRoutingHandler
    {
        private readonly IHandlerComposition _handlerComposition;
        private readonly IOrchardServices _orchardServices;
        private readonly IEscortProcessStageDefinitionManager<EscortProcessStageDefinition> _escortProcessStageDefinitionManager;
        private readonly IEscortRolePartialManager<EscortRolePartial> _escortRolePartialManager;
        private readonly IPSSAdminUsersManager<PSSAdminUsers> _pssAdminUsersManager;
        private readonly ISecretariatRoutingLevelManager<SecretariatRoutingLevel> _secretariatRoutingLevelManager;
        private readonly IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> _flowDefinitionLevelManager;
        private readonly IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> _flowApproverManager;
        private readonly IPSSRequestManager<PSSRequest> _pssRequestManager;
        private readonly IPoliceServiceRequestManager<PoliceServiceRequest> _policeServiceRequestManager;
        private readonly IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog> _requestCommandWorkFlowLogManager;
        public ILogger Logger { get; set; }
        public POSSAPSecretariatRoutingHandler(IEscortProcessStageDefinitionManager<EscortProcessStageDefinition> escortProcessStageDefinitionManager, IEscortRolePartialManager<EscortRolePartial> escortRolePartialManager, IOrchardServices orchardServices, IPSSAdminUsersManager<PSSAdminUsers> pssAdminUsersManager, ISecretariatRoutingLevelManager<SecretariatRoutingLevel> secretariatRoutingLevelManager, IPSServiceRequestFlowDefinitionLevelManager<PSServiceRequestFlowDefinitionLevel> flowDefinitionLevelManager, IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> flowApproverManager, IPSSRequestManager<PSSRequest> pssRequestManager, IPoliceServiceRequestManager<PoliceServiceRequest> policeServiceRequestManager, IRequestCommandWorkFlowLogManager<RequestCommandWorkFlowLog> requestCommandWorkFlowLogManager, IHandlerComposition handlerComposition)
        {
            _escortProcessStageDefinitionManager = escortProcessStageDefinitionManager;
            _escortRolePartialManager = escortRolePartialManager;
            _orchardServices = orchardServices;
            _pssAdminUsersManager = pssAdminUsersManager;
            _secretariatRoutingLevelManager = secretariatRoutingLevelManager;
            _flowDefinitionLevelManager = flowDefinitionLevelManager;
            _flowApproverManager = flowApproverManager;
            _pssRequestManager = pssRequestManager;
            _policeServiceRequestManager = policeServiceRequestManager;
            _requestCommandWorkFlowLogManager = requestCommandWorkFlowLogManager;
            _handlerComposition = handlerComposition;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            _handlerComposition.IsAuthorized(permission);
        }


        /// <summary>
        /// Routes the request to the escort stage for the selected escort process stage definition
        /// </summary>
        /// <param name="requestDetailsVM"></param>
        /// <param name="errors"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToEscortStage(EscortRequestDetailsVM requestDetailsVM, ref List<ErrorModel> errors)
        {
            try
            {
                //check if this request exists
                if (_pssRequestManager.Count(x => x.Id == requestDetailsVM.RequestId) == 0)
                {
                    Logger.Error($"No request found with the id of {requestDetailsVM.RequestId}");
                    throw new NoRecordFoundException();
                }

                //check if this is an actual stage
                if (_escortProcessStageDefinitionManager.Count(x => x.Id == requestDetailsVM.SelectedRequestStage) == 0)
                {
                    Logger.Error($"No escort process stage definition found with the specified id of {requestDetailsVM.SelectedRequestStage}");
                    errors.Add(new ErrorModel { FieldName = nameof(EscortRequestDetailsVM.SelectedRequestStage), ErrorMessage = "Selected request stage not valid" });
                    throw new DirtyFormDataException();
                }

                //validate comment
                if(string.IsNullOrEmpty(requestDetailsVM.Comment) || string.IsNullOrEmpty(requestDetailsVM.Comment.Trim()) || requestDetailsVM.Comment.Trim().Length < 10)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Comment requires at least 10 characters", FieldName = nameof(EscortRequestDetailsVM.Comment) });
                    throw new DirtyFormDataException("Comment requires at least 10 characters");
                }

                List<EscortPartialVM> partials = _escortRolePartialManager.GetPartials(requestDetailsVM.ApproverId).ToList();
                //Get escort role partial for this admin to perform logic for routing the request to the escort stage
                IEscortViewComposition partialCompImpl = ((IEscortViewComposition)Activator.CreateInstance(partials[0].ImplementationClass.Split(',')[0], partials[0].ImplementationClass.Split(',')[1]).Unwrap());
                partialCompImpl.SetTransactionManagerForDBQueries(_orchardServices.TransactionManager);
                return partialCompImpl.RouteToThisEscortStage(partials[0], requestDetailsVM);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _secretariatRoutingLevelManager.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Routes the request to the selected character certificate flow definition level
        /// </summary>
        /// <param name="requestDetailsVM"></param>
        /// <param name="errors"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToCharacterCertificateStage(CharacterCertificateRequestDetailsVM requestDetailsVM, ref List<ErrorModel> errors)
        {
            try
            {
                //check if this request exists
                if (_pssRequestManager.Count(x => x.Id == requestDetailsVM.RequestId) == 0)
                {
                    Logger.Error($"No pss request found with the specified id of {requestDetailsVM.RequestId}");
                    throw new NoRecordFoundException();
                }
                //check if this is an actual flow definition level
                if (_flowDefinitionLevelManager.Count(x => x.Id == requestDetailsVM.SelectedRequestStage) == 0)
                {
                    Logger.Error($"No character certificate flow definition level found with the specified id of {requestDetailsVM.SelectedRequestStage}");
                    errors.Add(new ErrorModel { FieldName = nameof(CharacterCertificateRequestDetailsVM.SelectedRequestStage), ErrorMessage = "Selected request stage is not valid" });
                    throw new DirtyFormDataException();
                }
                //validate comment
                if (string.IsNullOrEmpty(requestDetailsVM.Comment) || string.IsNullOrEmpty(requestDetailsVM.Comment.Trim()) || requestDetailsVM.Comment.Trim().Length < 10)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Comment requires at least 10 characters", FieldName = nameof(CharacterCertificateRequestDetailsVM.Comment) });
                    throw new DirtyFormDataException("Comment requires at least 10 characters");
                }

                //This is where we perform all the actions that need to be done to get to this level
                //Get approver for this stage
                PSServiceRequestFlowApproverDTO requestFlowApproverDTO = _flowApproverManager.GetServiceRequestFlowApproverForDefinitionLevelWithId(requestDetailsVM.SelectedRequestStage);
                if (requestFlowApproverDTO == null){ Logger.Error($"Could not find service request flow approver record for definition level with id {requestDetailsVM.SelectedRequestStage}");
                    throw new NoRecordFoundException(); }

                //Get request details including the current flow definition level
                PSSRequestVM request = _pssRequestManager.GetPSSRequestServiceDetails(requestDetailsVM.RequestId);

                //create entry in secretariat routing level to hold the stage the request has been routed to
                _secretariatRoutingLevelManager.SaveSecretariatRoutingLevel(requestDetailsVM.RequestId, requestDetailsVM.SelectedRequestStage, _pssAdminUsersManager.GetAdminUserId(requestDetailsVM.ApproverId), typeof(PSServiceRequestFlowDefinitionLevel).Name);

                _requestCommandWorkFlowLogManager.SetPreviousRequestCommandWorkflowLogsToInactive(requestDetailsVM.RequestId);

                //Add the next level approver to the request command log
                _requestCommandWorkFlowLogManager.AddRequestCommandWorkflowLog(requestDetailsVM.RequestId, requestFlowApproverDTO.PSSAdminUser.Command.Id, requestFlowApproverDTO.FlowDefinitionLevel.Id);

                //move to this flow definition level
                _pssRequestManager.UpdateRequestFlowId(requestDetailsVM.RequestId, requestFlowApproverDTO.FlowDefinitionLevel.Id, Core.Models.Enums.PSSRequestStatus.PendingApproval);
                _policeServiceRequestManager.SavePoliceServiceRequest(requestDetailsVM.RequestId, request.FlowDefinitionLevelId, requestFlowApproverDTO.FlowDefinitionLevel.Id);

                return new SecretariatRoutingApprovalResponse
                {
                    NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", request.ServiceName, request.FileRefNumber, request.CustomerName, "This application has been moved to the selected approval stage"),
                };

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _secretariatRoutingLevelManager.RollBackAllTransactions();
                throw;
            }
        }
    }
}