using NHibernate.Linq;
using Orchard.Data;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition
{
    public class CPEscortViewComposition : IEscortViewComposition
    {
        public int StageIdentifier => 1;
        private ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public CPEscortViewComposition()
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
                PSServiceRequestFlowDefinitionLevelDTO workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == requestId).Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.FlowDefinitionLevel.Id, DefinitionId = x.FlowDefinitionLevel.Definition.Id, Position = x.FlowDefinitionLevel.Position }).FirstOrDefault();

                int nextWorkflowDefinitionLevelId = _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>().Where(x => (x.Definition.Id == workFlowDefLevel.DefinitionId) && (x.WorkFlowActionValue == (int)RequestDirection.Approval) && x.Position > workFlowDefLevel.Position).OrderBy(x => x.Position).First().Id;

                int adminCommand = _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.Id == adminUserId).Select(x => x.Command.Id).SingleOrDefault();
                int numberOfOfficers = _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.Id == requestId).Select(x => x.NumberOfOfficers).SingleOrDefault();
                //when the commissioner of police approves
                //we need to add the next level officer to the approval scheme
                //so that the apporval shows on their end of the approval report
                //here we need to get the officer for the next role
                //the next role here will be gotten from the process stage flow
                var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                    .Where(x => ((x.AdminUser.User.Id == adminUserId) && (x.CommandType.Id == commandTypeId) && (x.IsActive)))
                    .Select(x => new EscortProcessFlowVM { LevelId = x.Level.Id })
                    .First();

                //get the level where their parent is me
                //this would indicate the level after me, I can have many level so far they inherit from me
                var childLevels = _transactionManager.GetSession().Query<EscortProcessStageDefinition>()
                    .Where(x => ((x.ParentDefinition.Id == thisProcessFlow.LevelId) && (x.CommandType.Id == commandTypeId) && (x.IsActive))).ToList();

                IEnumerable<EscortProcessFlow> nextLevelapprovers = childLevels.Select(c => c.AssignedFlow).SelectMany(v => v);

                UpdateRequestCommandWorkflowLog(requestId, adminCommand, workFlowDefLevel.Id, false);

                foreach (var item in nextLevelapprovers)
                {
                    if (CheckIfNextDefinitionLevelIsApprovalAndHasSameCommand(adminCommand, nextWorkflowDefinitionLevelId, item.AdminUser.User.Id))
                    {
                        UpdatePSSRequest(requestId, nextWorkflowDefinitionLevelId);
                        SavePoliceServiceRequest(requestId, workFlowDefLevel.Id, nextWorkflowDefinitionLevelId);
                        if (_transactionManager.GetSession().Query<RequestCommandWorkFlowLog>().Count(x => (x.Request.Id == requestId) && (x.Command.Id == item.AdminUser.Command.Id) && (x.DefinitionLevel.Id == nextWorkflowDefinitionLevelId)) > 0)
                        {
                            UpdateRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, nextWorkflowDefinitionLevelId, true);
                        }
                        else { AddRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, nextWorkflowDefinitionLevelId); }
                    }
                    else
                    {
                        if (_transactionManager.GetSession().Query<RequestCommandWorkFlowLog>().Count(x => (x.Request.Id == requestId) && (x.Command.Id == item.AdminUser.Command.Id) && (x.DefinitionLevel.Id == workFlowDefLevel.Id)) > 0)
                        {
                            UpdateRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, workFlowDefLevel.Id, true);
                        }
                        else { AddRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, workFlowDefLevel.Id); }
                    }
                }

                //need to update the grp allocation to the new level
                var allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                        .Where(x => ((x.Request.Id == requestId))).SingleOrDefault();
                if (allocGrp == null)
                {
                    var serviceId = _transactionManager.GetSession().Query<PSSRequest>()
                    .Where(r => r.Id == requestId).Select(x => x.Service.Id).FirstOrDefault();

                    EscortSquadAllocationGroup allocModel = new EscortSquadAllocationGroup { AdminUser = new PSSAdminUsers { Id = adminUserId }, Comment = "CP has approved", Request = new PSSRequest { Id = requestId }, RequestLevel = nextLevelapprovers.First().Level, Service = new PSService { Id = serviceId }, StatusDescription = "CP has approved" };
                    _transactionManager.GetSession().Save(allocModel);

                    EscortSquadAllocation squadAllocModel = new EscortSquadAllocation { Command = new Command { Id = adminCommand }, NumberOfOfficers = numberOfOfficers, StatusDescription = "CP has approved", AllocationGroup = new EscortSquadAllocationGroup { Id = allocModel.Id }, CommandType = new CommandType { Id = commandTypeId }, Fulfilled = false, IsDeleted = false };
                    _transactionManager.GetSession().Save(squadAllocModel);
                }
                else
                {
                    allocGrp.RequestLevel = nextLevelapprovers.First().Level;
                }

                PSSRequest request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == requestId).FirstOrDefault();
                return new RequestApprovalResponse
                {
                    ServiceType = request.Service.ServiceType.ToString(),
                    FileNumber = request.FileRefNumber,
                    CustomerName = request.CBSUser.Name,
                    NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", (PSSServiceTypeDefinition)request.Service.ServiceType, request.FileRefNumber, request.CBSUser.Name, "This application has been moved to the DCP for officer allocation"),
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
                PSSAdminUsersVM adminUser = _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.Id == objUserInput.ApproverId).Select(x => new PSSAdminUsersVM { Fullname = x.Fullname, Command = new HelperModels.CommandVM { Id = x.Command.Id, Name = x.Command.Name } }).SingleOrDefault();

                int workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == objUserInput.RequestId).Select(x => x.FlowDefinitionLevel.Id).FirstOrDefault();

                objUserInput.Comment = $"{adminUser.Fullname} (CP {adminUser.Command.Name}): {objUserInput.Comment}";

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


        private bool CheckIfNextDefinitionLevelIsApprovalAndHasSameCommand(int commandId, int nextWorkflowDefinitionLevelId, int nextApproverId)
        {
            return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Count(x => x.FlowDefinitionLevel.Id == nextWorkflowDefinitionLevelId && x.AssignedApprover.Id == nextApproverId && x.PSSAdminUser.Command.Id == commandId) > 0;
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


        /// <summary>
        /// Routes the request to CP level
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToThisEscortStage(EscortPartialVM item, EscortRequestDetailsVM objUserInput)
        {
            //This is where we perform all the actions that need to be done to get to this level
            //Get CP command for this stage
            CommandVM commandForRouteStage = _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => x.Level.Id == objUserInput.SelectedRequestStage).Select(x => new CommandVM { Id = x.AdminUser.Command.Id, Name = x.AdminUser.Command.Name }).FirstOrDefault();

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
                NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", request.ServiceName, request.FileRefNumber, request.CustomerName, "This application has been moved to CP " + commandForRouteStage.Name.Trim().TrimEnd(',')),
            };
        }

    }
}