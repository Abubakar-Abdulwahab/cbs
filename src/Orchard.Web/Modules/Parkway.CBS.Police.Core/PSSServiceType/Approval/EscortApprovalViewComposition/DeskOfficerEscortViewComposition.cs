using Orchard.Data;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models;
using NHibernate.Linq;
using System.Linq;
using System;
using Parkway.CBS.Police.Core.DTO;
using Orchard.Logging;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition
{
    public class DeskOfficerEscortViewComposition : IEscortViewComposition
    {
        public int StageIdentifier => 1;
        private ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public DeskOfficerEscortViewComposition()
        {
            Logger = NullLogger.Instance;
        }

        public dynamic SetPartialData(EscortPartialVM partialComp)
        {
            return new DeskOfficerApprovalVM { ControllerName = "PSSEscortApproval", ApprovalButtonName = "Submit" };
        }

        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        public void SetTransactionManagerForDBQueries(ITransactionManager transactionManager) { _transactionManager = transactionManager; }


        /// <summary>
        /// Do validation on partial comment
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public bool DoValidation(EscortPartialVM item, EscortRequestDetailsVM objUserInput, ref List<ErrorModel> errors)
        {
            if (string.IsNullOrEmpty(objUserInput.Comment))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment field is required", FieldName = "Comment" });
                return true;
            }

            if (objUserInput.Comment.Trim().Length < 10)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Comment requires at least 10 characters", FieldName = "Comment" });
                return true;
            }
            return false;
        }


        /// <summary>
        /// Saves model records
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public void SaveRecords(EscortPartialVM item, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors)
        {
            SaveApprovalLog(objUserInput);
        }


        /// <summary>
        /// Here we done the level shifting and approval assignments
        /// </summary>
        public RequestApprovalResponse OnSubmit(int adminUserId, Int64 requestId, int commandTypeId)
        {
            try
            {
                int workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == requestId).Select(x => x.FlowDefinitionLevel.Id).FirstOrDefault();
                //when the desk officer approves
                //we need to add the next level officer to the approval scheme
                //so that the apporval shows on their end of the approval report
                //here we need to get the officer for the next role
                //the next role here will be gotten from the process stage flow
                //here we have a predefined value of 1 as the level value for desk officers
                var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                    .Where(x => ((x.AdminUser.User.Id == adminUserId) && (x.CommandType.Id == commandTypeId) && (x.IsActive)))
                    .Select(x => new EscortProcessFlowVM { LevelId = x.Level.Id })
                    .First();

                //get the level where their parent is me
                //this would indicate the level after me, I can have man level so far they inherit from me
                var childLevels = _transactionManager.GetSession().Query<EscortProcessStageDefinition>()
                    .Where(x => ((x.ParentDefinition.Id == thisProcessFlow.LevelId) && (x.CommandType.Id == commandTypeId) && (x.IsActive))).ToList();

                IEnumerable<EscortProcessFlow> nextLevelapprovers = childLevels.Select(c => c.AssignedFlow).SelectMany(v => v);

                SetPreviousRequestCommandWorkflowLogsToInactive(requestId);

                foreach (var item in nextLevelapprovers)
                {
                    if (_transactionManager.GetSession().Query<RequestCommandWorkFlowLog>().Count(x => (x.Request.Id == requestId) && (x.Command.Id == item.AdminUser.Command.Id) && (x.DefinitionLevel.Id == workFlowDefLevel)) > 0)
                    {
                        UpdateRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, workFlowDefLevel, true);
                    }
                    else { AddRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, workFlowDefLevel); }
                }

                //need to update the grp allocation to the new level
                var allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                        .Where(x => ((x.Request.Id == requestId))).SingleOrDefault();
                if (allocGrp == null)
                {
                    var serviceId = _transactionManager.GetSession().Query<PSSRequest>()
                    .Where(r => r.Id == requestId).Select(x => x.Service.Id).FirstOrDefault();

                    EscortSquadAllocationGroup allocModel = new EscortSquadAllocationGroup { AdminUser = new PSSAdminUsers { Id = adminUserId }, Comment = "Desk Officer has approved", Request = new PSSRequest { Id = requestId }, RequestLevel = nextLevelapprovers.First().Level, Service = new PSService { Id = serviceId }, StatusDescription = "Desk Officer has approved" };
                    _transactionManager.GetSession().Save(allocModel);
                }
                else
                {
                    allocGrp.RequestLevel = nextLevelapprovers.First().Level;
                }

                //check if there is a next level after this
                PSSRequest request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == requestId).FirstOrDefault();
                return new RequestApprovalResponse
                {
                    ServiceType = request.Service.ServiceType.ToString(),
                    FileNumber = request.FileRefNumber,
                    CustomerName = request.CBSUser.Name,
                    NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", (PSSServiceTypeDefinition)request.Service.ServiceType, request.FileRefNumber, request.CBSUser.Name, "This application has been moved to the DIG for officer allocation"),
                    ResponseFromPartial = true,
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Creates approval log
        /// </summary>
        /// <param name="objUserInput"></param>
        private void SaveApprovalLog(EscortRequestDetailsVM objUserInput)
        {
            try
            {
                string adminName = _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.Id == objUserInput.ApproverId).Select(x => x.Fullname).SingleOrDefault();

                int workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == objUserInput.RequestId).Select(x => x.FlowDefinitionLevel.Id).FirstOrDefault();

                objUserInput.Comment = $"{adminName} (Desk Officer): {objUserInput.Comment}";

                string requestApprovalLogTableName = "Parkway_CBS_Police_Core_" + typeof(PSSRequestApprovalLog).Name;
                string requestApprovalLogQuery = $"INSERT INTO {requestApprovalLogTableName} (Request_Id, Status, FlowDefinitionLevel_Id, AddedByAdminUser_Id, Comment, CreatedAtUtc, UpdatedAtUtc) VALUES(:requestId, :requestStatus, :workFlowDefLevel, :approverId, :comment, GETDATE(), GETDATE());";

                var query = _transactionManager.GetSession().CreateSQLQuery(requestApprovalLogQuery);
                query.SetParameter("requestId", objUserInput.RequestId);
                query.SetParameter("requestStatus", (int)PSSRequestStatus.PendingApproval);
                query.SetParameter("workFlowDefLevel", workFlowDefLevel);
                query.SetParameter("approverId", objUserInput.ApproverId);
                query.SetParameter("comment", objUserInput.Comment);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        EscortApprovalMessage IEscortViewComposition.Approval(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int userPartId)
        {
            throw new UserNotAuthorizedForThisActionException();
        }


        public CanApproveEscortVM CanApprove(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int adminUserId)
        {
            throw new UserNotAuthorizedForThisActionException();
        }


        private void UpdateRequestCommandWorkflowLog(long requestId, int commandId, int flowDefinitionLevelId, bool isActive)
        {
            string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestCommandWorkFlowLog).Name;
            string queryString = $"UPDATE {tableName} SET {nameof(RequestCommandWorkFlowLog.IsActive)} = :isActive, {nameof(RequestCommandWorkFlowLog.UpdatedAtUtc)} = GETDATE(), {nameof(RequestCommandWorkFlowLog.RequestPhaseId)} = :requestPhaseId, {nameof(RequestCommandWorkFlowLog.RequestPhaseName)} = :requestPhaseName WHERE {nameof(RequestCommandWorkFlowLog.Request)}_Id = :requestId AND {nameof(RequestCommandWorkFlowLog.DefinitionLevel)}_Id = :definitionLevelId AND {nameof(RequestCommandWorkFlowLog.Command)}_Id = :commandId";
            var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
            query.SetParameter("requestId", requestId);
            query.SetParameter("definitionLevelId", flowDefinitionLevelId);
            query.SetParameter("commandId", commandId);
            query.SetParameter("isActive", isActive);
            query.SetParameter("requestPhaseId", (int)RequestPhase.Ongoing);
            query.SetParameter("requestPhaseName", nameof(RequestPhase.Ongoing));
            query.ExecuteUpdate();
        }


        private void AddRequestCommandWorkflowLog(long requestId, int commandId, int flowDefinitionLevelId)
        {
            string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestCommandWorkFlowLog).Name;
            string queryString = $"INSERT INTO {tableName}({nameof(RequestCommandWorkFlowLog.Request)}_Id, {nameof(RequestCommandWorkFlowLog.Command)}_Id, {nameof(RequestCommandWorkFlowLog.DefinitionLevel)}_Id, {nameof(RequestCommandWorkFlowLog.IsActive)}, {nameof(RequestCommandWorkFlowLog.CreatedAtUtc)}, {nameof(RequestCommandWorkFlowLog.UpdatedAtUtc)}, {nameof(RequestCommandWorkFlowLog.RequestPhaseId)}, {nameof(RequestCommandWorkFlowLog.RequestPhaseName)}) VALUES(:requestId, :commandId, :definitionLevelId, :isActive, GETDATE(), GETDATE(), :requestPhaseId, :requestPhaseName)";
            var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
            query.SetParameter("requestId", requestId);
            query.SetParameter("definitionLevelId", flowDefinitionLevelId);
            query.SetParameter("commandId", commandId);
            query.SetParameter("isActive", true);
            query.SetParameter("requestPhaseId", (int)RequestPhase.New);
            query.SetParameter("requestPhaseName", nameof(RequestPhase.New));
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Set all work flow logs to inactive
        /// </summary>
        /// <param name="requestId"></param>
        private void SetPreviousRequestCommandWorkflowLogsToInactive(long requestId)
        {
            string tableName = "Parkway_CBS_Police_Core_" + typeof(RequestCommandWorkFlowLog).Name;
            string queryString = $"UPDATE {tableName} SET {nameof(RequestCommandWorkFlowLog.IsActive)} = :isActive, {nameof(RequestCommandWorkFlowLog.UpdatedAtUtc)} = GETDATE(), {nameof(RequestCommandWorkFlowLog.RequestPhaseId)} = :requestPhaseId, {nameof(RequestCommandWorkFlowLog.RequestPhaseName)} = :requestPhaseName WHERE {nameof(RequestCommandWorkFlowLog.Request)}_Id = :requestId";
            var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
            query.SetParameter("requestId", requestId);
            query.SetParameter("isActive", false);
            query.SetParameter("requestPhaseId", (int)RequestPhase.Ongoing);
            query.SetParameter("requestPhaseName", nameof(RequestPhase.Ongoing));
            query.ExecuteUpdate();
        }


        /// <summary>
        /// Routes the request to IGP Desk Officer level
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToThisEscortStage(EscortPartialVM item, EscortRequestDetailsVM objUserInput)
        {
            //This is where we perform all the actions that need to be done to get to this level
            //Get IG command for this stage
            CommandVM commandForRouteStage = _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => (x.Level.Id == objUserInput.SelectedRequestStage) && (x.IsActive)).Select(x => new CommandVM { Id = x.AdminUser.Command.Id, Name = x.AdminUser.Command.Name }).FirstOrDefault();

            //Get current workflow definition level
            PSServiceRequestFlowDefinitionLevelDTO workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == objUserInput.RequestId).Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.FlowDefinitionLevel.Id, DefinitionId = x.FlowDefinitionLevel.Definition.Id, Position = x.FlowDefinitionLevel.Position }).FirstOrDefault();

            //Get the next approval workflow definition level after the current workflow definition level
            int nextWorkflowDefinitionLevelId = _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>().Where(x => (x.Definition.Id == workFlowDefLevel.DefinitionId) && (x.WorkFlowActionValue == (int)RequestDirection.Approval) && x.Position > workFlowDefLevel.Position).OrderBy(x => x.Position).First().Id;

            SetPreviousRequestCommandWorkflowLogsToInactive(objUserInput.RequestId);

            //Add the next level approver to the request command log
            AddRequestCommandWorkflowLog(objUserInput.RequestId, commandForRouteStage.Id, nextWorkflowDefinitionLevelId);

            PSSRequestVM request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == objUserInput.RequestId).Select(x => new PSSRequestVM { ServiceId = x.Service.Id, ServiceName = x.Service.Name, CustomerName = x.CBSUser.Name, FileRefNumber = x.FileRefNumber }).FirstOrDefault();

            //move to this flow definition level
            UpdatePSSRequest(objUserInput.RequestId, nextWorkflowDefinitionLevelId);
            SavePoliceServiceRequest(objUserInput.RequestId, workFlowDefLevel.Id, nextWorkflowDefinitionLevelId);

            return new SecretariatRoutingApprovalResponse
            {
                NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", request.ServiceName, request.FileRefNumber, request.CustomerName, "This application has been moved to " + commandForRouteStage.Name.Trim().TrimEnd(',')),
            };
        }


        private void UpdatePSSRequest(long requestId, int flowDefinitionLevelId)
        {
            string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSRequest).Name;
            string queryString = $"UPDATE {tableName} SET {nameof(PSSRequest.FlowDefinitionLevel)}_Id = :flowDefinitionLevelId, {nameof(PSSRequest.UpdatedAtUtc)} = GETDATE() WHERE {nameof(PSSRequest.Id)} = :requestId";
            var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
            query.SetParameter("requestId", requestId);
            query.SetParameter("flowDefinitionLevelId", flowDefinitionLevelId);
            query.ExecuteUpdate();
        }


        private void SavePoliceServiceRequest(long requestId, int flowDefinitionLevelId, int nextFlowDefinitionLevelId)
        {
            if (_transactionManager.GetSession().Query<PoliceServiceRequest>().Count(x => (x.FlowDefinitionLevel.Id == nextFlowDefinitionLevelId) && (x.Request.Id == requestId)) > 0) { return; }
            PSServiceRequestInvoiceValidationDTO requestDetails = _transactionManager.GetSession().Query<PoliceServiceRequest>()
                .Where(x => (x.Request.Id == requestId) && (x.FlowDefinitionLevel.Id == flowDefinitionLevelId))
                .Select(x => new PSServiceRequestInvoiceValidationDTO { RevenueHeadId = x.RevenueHead.Id, InvoiceId = x.Invoice.Id, Request = x.Request, ServiceId = x.Service.Id, ServiceRequestStatus = x.Status })
                .SingleOrDefault();

            string tableName = "Parkway_CBS_Police_Core_" + typeof(PoliceServiceRequest).Name;
            string queryString = $"INSERT INTO {tableName}({nameof(PoliceServiceRequest.RevenueHead)}_Id, {nameof(PoliceServiceRequest.Invoice)}_Id, {nameof(PoliceServiceRequest.Request)}_Id, {nameof(PoliceServiceRequest.Service)}_Id, {nameof(PoliceServiceRequest.Status)}, {nameof(PoliceServiceRequest.CreatedAtUtc)}, {nameof(PoliceServiceRequest.UpdatedAtUtc)}, {nameof(PoliceServiceRequest.FlowDefinitionLevel)}_Id) VALUES(:revenueHeadId, :invoiceId, :requestId, :serviceId, :status, GETDATE(), GETDATE(), :flowDefinitionLevelId)";

            var query = _transactionManager.GetSession().CreateSQLQuery(queryString);
            query.SetParameter("revenueHeadId", requestDetails.RevenueHeadId);
            query.SetParameter("invoiceId", requestDetails.InvoiceId);
            query.SetParameter("requestId", requestId);
            query.SetParameter("serviceId", requestDetails.ServiceId);
            query.SetParameter("status", requestDetails.ServiceRequestStatus);
            query.SetParameter("flowDefinitionLevelId", nextFlowDefinitionLevelId);
            query.ExecuteUpdate();
        }

    }
}