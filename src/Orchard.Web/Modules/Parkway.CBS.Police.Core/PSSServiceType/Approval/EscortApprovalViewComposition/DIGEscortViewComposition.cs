using Orchard.Data;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts;
using NHibernate.Linq;
using Parkway.CBS.Police.Core.Models;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using Orchard.Logging;
using System;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition
{
    public class DIGEscortViewComposition : IEscortViewComposition
    {
        private ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public DIGEscortViewComposition()
        {
            Logger = NullLogger.Instance;
        }

        public int StageIdentifier => 2;

        /// <summary>
        /// Validates selected tactical squads
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public bool DoValidation(EscortPartialVM item, EscortRequestDetailsVM objUserInput, ref List<ErrorModel> errors)
        {
            //here we validate if a tactical squad was selected on the admin interface
            if (objUserInput.TacticalSquadsSelection == null || objUserInput.TacticalSquadsSelection.Count == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = "You must select at least one tactical squad", FieldName = "TacticalSquadsSelection" });
                return true;
            }

            //here we validate that the selected tactical squad is valid and if it is a tactical command
            var session = _transactionManager.GetSession();
            int commandType = session.Query<PSSEscortDetails>().Where(x => x.Request == new PSSRequest { Id = item.RequestId }).SingleOrDefault().CommandType.Id;

            foreach (var squad in objUserInput.TacticalSquadsSelection)
            {
                if (session.Query<Command>().Count(x => x.Id == squad.TacticalSquadId && x.CommandType == new CommandType { Id = commandType }) == 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Tactical squad selected is not valid", FieldName = "TacticalSquadsSelection" });
                    return true;
                }

                if (squad.NumberofOfficers == 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Tactical squads selected requires number of officers to be specified", FieldName = "TacticalSquadsSelection" });
                    return true;
                }
            }

            EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                        .Where(x => ((x.Request.Id == objUserInput.RequestId))).SingleOrDefault();
            // validation that happens during edit
            {

                if (objUserInput.RemovedTacticalSquads != null)
                {
                    foreach (var removedSquad in objUserInput.RemovedTacticalSquads)
                    {
                        if (session.Query<Command>().Count(x => x.Id == removedSquad.TacticalSquadId && x.CommandType == new CommandType { Id = commandType }) == 0)
                        {
                            errors.Add(new ErrorModel { ErrorMessage = "Tactical squad selected is not valid", FieldName = "TacticalSquadsSelection" });
                            return true;
                        }
                    }
                }

            }

            return false;
        }

        public dynamic SetPartialData(EscortPartialVM partialComp)
        {
            int commandTypeId = _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request == new PSSRequest { Id = partialComp.RequestId }).SingleOrDefault().CommandType.Id;
            return new DIGApprovalVM
            {
                TacticalSquads = _transactionManager.GetSession().Query<Command>().Where(x => x.IsActive && x.CommandType == new CommandType { Id = commandTypeId }).Select(x => new HelperModels.CommandVM { Id = x.Id, Name = x.Name }),
                AssignedTacticalSquads = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>().Where(x => x.Request == new PSSRequest { Id = partialComp.RequestId }).SingleOrDefault()?.Allocations.Where(all => !all.IsDeleted).Select(x => new EscortSquadAllocationVM { Id = x.Id, Command = new HelperModels.CommandVM { Id = x.Command.Id, Name = x.Command.Name }, NumberOfOfficers = x.NumberOfOfficers, EscortSquadAllocationGroupId = x.AllocationGroup.Id })
            };
        }

        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        public void SetTransactionManagerForDBQueries(ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
        }

        /// <summary>
        /// Saves model records to EscortSquadAllocation & EscortSquadAllocationGroup
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public void SaveRecords(EscortPartialVM item, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors)
        {
            try
            {
                var session = _transactionManager.GetSession();
                int pssAdminUserId = session.Query<PSSAdminUsers>().Where(x => x.User == new Orchard.Users.Models.UserPartRecord { Id = objUserInput.ApproverId }).SingleOrDefault().Id;
                int commandTypeId = session.Query<PSSEscortDetails>().Where(x => x.Request == new PSSRequest { Id = item.RequestId }).SingleOrDefault().CommandType.Id;
                string statusDescription = "PendingAdminApproval";

                var reference = string.Empty;
                objUserInput.AllocationGroupId = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                            .Where(x => ((x.Request.Id == objUserInput.RequestId))).SingleOrDefault().Id;

                //Number of ticks + PSS Admin Id + Allocation Group Id + Implementation Class HashCode value - Reference Format
                reference = string.Format("ESC-{0}-ADMN-{1}-ALLOC-{2}-IMPL-{3}", System.DateTime.Now.Ticks, pssAdminUserId, objUserInput.AllocationGroupId, item.ImplementationClass.GetHashCode());
                string squadAllocationSelectionTrackingTableName = "Parkway_CBS_Police_Core_" + typeof(EscortSquadAllocationSelectionTracking).Name;
                string squadAllocationSelectionTrackingQueryText = string.Empty;
                foreach (var addition in objUserInput.TacticalSquadsSelection)
                {
                    squadAllocationSelectionTrackingQueryText = $"INSERT INTO {squadAllocationSelectionTrackingTableName}(Command_Id, NumberOfOfficers, IsDeleted, Reference, AllocationGroup_Id, CreatedAtUtc, UpdatedAtUtc) VALUES({addition.TacticalSquadId}, {addition.NumberofOfficers}, 0, '{reference}', {objUserInput.AllocationGroupId}, GETDATE(), GETDATE())";

                    session.CreateSQLQuery(squadAllocationSelectionTrackingQueryText).ExecuteUpdate();
                }

                objUserInput.RemovedTacticalSquads = (objUserInput.RemovedTacticalSquads == null) ? new List<DIGTacticalSquadVM> { } : objUserInput.RemovedTacticalSquads;

                foreach (var removal in objUserInput.RemovedTacticalSquads)
                {
                    squadAllocationSelectionTrackingQueryText = $"INSERT INTO {squadAllocationSelectionTrackingTableName}(Command_Id, NumberOfOfficers, IsDeleted, Reference, AllocationGroup_Id, CreatedAtUtc, UpdatedAtUtc) VALUES({removal.TacticalSquadId}, {removal.NumberofOfficers}, 1, '{reference}', {objUserInput.AllocationGroupId}, GETDATE(), GETDATE())";

                    session.CreateSQLQuery(squadAllocationSelectionTrackingQueryText).ExecuteUpdate();
                }

                //merge query comes in here, this updates the EscortSquadAllocation table with the EscortSquadAllocationSelectionTracking table
                string escortSquadAllocationSyncQuery = $"MERGE Parkway_CBS_Police_Core_EscortSquadAllocation AS Target USING Parkway_CBS_Police_Core_EscortSquadAllocationSelectionTracking AS Source ON Source.AllocationGroup_Id = Target.AllocationGroup_Id AND Source.Command_Id = Target.Command_Id WHEN MATCHED AND Source.Reference = '{reference}' THEN UPDATE SET Target.IsDeleted = Source.IsDeleted, Target.NumberOfOfficers = Source.NumberOfOfficers, Target.UpdatedAtUtc = GETDATE() WHEN NOT MATCHED BY Target AND Source.Reference = '{reference}' THEN INSERT(Command_Id, NumberOfOfficers, StatusDescription, Fulfilled, AllocationGroup_Id, CommandType_Id, CreatedAtUtc, UpdatedAtUtc, IsDeleted) VALUES(Source.Command_Id, Source.NumberOfOfficers, '{statusDescription}', 0, Source.AllocationGroup_Id, {commandTypeId}, GETDATE(), GETDATE(), 0);";

                session.CreateSQLQuery(escortSquadAllocationSyncQuery).ExecuteUpdate();

                SaveApprovalLog(objUserInput);
                return;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }



        public RequestApprovalResponse OnSubmit(int adminUserId, long requestId, int commandTypeId)
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
                    .Select(x=> new EscortProcessFlowVM { LevelId = x.Level.Id, CommandId = x.AdminUser.Command.Id })
                    .First();

                //get the level where their parent is me
                //this would indicate the level after me, I can have many levels so far they inherit from me
                var childLevels = _transactionManager.GetSession().Query<EscortProcessStageDefinition>()
                    .Where(x => ((x.ParentDefinition.Id == thisProcessFlow.LevelId) && (x.CommandType.Id == commandTypeId) && (x.IsActive))).ToList();

                //
                //get group allocations
                EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                        .Where(x => ((x.Request.Id == requestId))).SingleOrDefault();

                //get the allocations for this level
                //IEnumerable<EscortSquadAllocation> sqaudAllocs = allocGrp.Allocations.Where(x => x.IsDeleted);
                //foreach (var item in sqaudAllocs)
                //{
                //    item.IsDeleted = true;
                //}
                IEnumerable<EscortFormationAllocation> squadFormations = allocGrp.Allocations.Where(x => x.IsDeleted).Select(xr => xr.Formations).SelectMany(s => s);
                foreach (var item in squadFormations)
                {
                    item.IsDeleted = true;
                    UpdateRequestCommandWorkflowLog(requestId, item.Command.Id, workFlowDefLevel, false);
                }
                IEnumerable<EscortFormationOfficer> squadronOfficers = squadFormations.SelectMany(x => x.SquadronOfficers);
                foreach (var item in squadronOfficers)
                {
                    item.IsDeleted = true;
                }
                //now we need to get the next level users
                IEnumerable<EscortProcessFlow> nextLevelapprovers = childLevels.Select(c => c.AssignedFlow).SelectMany(v => v);
                //we have gotten the next level users
                IEnumerable<PSSAdminUsers> usersToNotify = nextLevelapprovers.Select(ad => ad.AdminUser);
                //now we have the list of users on this level, we need to filter by selected sqaud formation
                IEnumerable<EscortSquadAllocation> activeSqaudAllocs = allocGrp.Allocations.Where(x => !x.IsDeleted);
                //we need to loop through each and get the commands directly under them
                //List<Command> formationsUnderAciveSquad = new List<Command> { };
                //foreach (var item in activeSqaudAllocs)
                //{
                //    formationsUnderAciveSquad.AddRange(_transactionManager.GetSession().Query<Command>().Where(x => (x.Code.Contains(item.Command.Code + "-"))).ToList());
                //}

                //now we have all the commands that are within the active next levelers
                string messages = "";

                foreach (var item in nextLevelapprovers)
                {
                    if (activeSqaudAllocs.Any(asq => asq.Command == item.AdminUser.Command))
                    {
                        messages += item.AdminUser.Command.Name + ", ";
                        if (_transactionManager.GetSession().Query<RequestCommandWorkFlowLog>().Count(x => (x.Request.Id == requestId) && (x.Command.Id == item.AdminUser.Command.Id) && (x.DefinitionLevel.Id == workFlowDefLevel)) > 0)
                        {
                            UpdateRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, workFlowDefLevel, true);
                        }
                        else 
                        { 
                            AddRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, workFlowDefLevel);
                            //Update the request phase for the DIG to ongoing
                            UpdateRequestCommandWorkflowLog(requestId, thisProcessFlow.CommandId, workFlowDefLevel, true);
                        }
                    }
                    else
                    {
                        UpdateRequestCommandWorkflowLog(requestId, item.AdminUser.Command.Id, workFlowDefLevel, false);
                    }
                }

                //UpdateRequestCommandWorkflowLog(requestId, thisProcessFlow.AdminUser.Command.Id, workFlowDefLevel, false);

                //need to update the grp allocation to the new level
                allocGrp.RequestLevel = new EscortProcessStageDefinition { Id = nextLevelapprovers.First().Level.LevelGroupIdentifier };
                allocGrp.StatusDescription = "Squad units have been assigned and notifications sent out to " + messages.Trim().TrimEnd(',');
                allocGrp.Comment = "Squad units have been assigned and notifications sent out.";

                //check if there is a next level after this
                PSSRequest request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == requestId).FirstOrDefault();
                return new RequestApprovalResponse
                {
                    ServiceType = request.Service.ServiceType.ToString(),
                    FileNumber = request.FileRefNumber,
                    CustomerName = request.CBSUser.Name,
                    NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", (PSSServiceTypeDefinition)request.Service.ServiceType, request.FileRefNumber, request.CBSUser.Name, "This application has been moved to the AIG " + messages.Trim().TrimEnd(',')),
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

                string additionalComment = string.Empty;

                if (objUserInput.TacticalSquadsSelection.Any() && objUserInput.RemovedTacticalSquads.Any())
                {
                    //Added and removed tactical squads
                    additionalComment = $"The request for officers has been assigned to {string.Join(", ", objUserInput.TacticalSquadsSelection.Select(x => $"{x.SquadName} to provide {x.NumberofOfficers} officers"))} and the {string.Join(", ", objUserInput.RemovedTacticalSquads.Select(x => x.SquadName))} has been removed from assigning officers.";
                }
                else if (objUserInput.TacticalSquadsSelection.Any() && !objUserInput.RemovedTacticalSquads.Any())
                {
                    //Added tactical squads
                    additionalComment = $"The request for officers has been assigned to {string.Join(", ", objUserInput.TacticalSquadsSelection.Select(x => $"{x.SquadName} to provide {x.NumberofOfficers} officers"))}.";
                }
                else if (!objUserInput.TacticalSquadsSelection.Any() && objUserInput.RemovedTacticalSquads.Any())
                {
                    //Removed tactical squads
                    additionalComment = $"The {string.Join(", ", objUserInput.RemovedTacticalSquads.Select(x => x.SquadName))} has been removed from assigning officers.";
                }

                objUserInput.Comment = $"{adminName} (DIG): {objUserInput.Comment.TrimEnd('.')}. {additionalComment}";

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

        public EscortApprovalMessage Approval(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int userPartId)
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
        /// Routes the request to DIG level
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToThisEscortStage(EscortPartialVM item, EscortRequestDetailsVM objUserInput)
        {
            //This is where we perform all the actions that would've otherwise been performed by the IG before getting to this level
            //Get DIG command for this stage
            CommandVM commandForRouteStage = _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => (x.Level.Id == objUserInput.SelectedRequestStage) && (x.IsActive)).Select(x => new CommandVM { Id = x.AdminUser.Command.Id, Name = x.AdminUser.Command.Name }).FirstOrDefault();

            //Get the current admin user
            PSSAdminUsersVM adminUser = _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User == new Orchard.Users.Models.UserPartRecord { Id = objUserInput.ApproverId }).Select(x => new PSSAdminUsersVM { Id = x.Id, Command = new CommandVM { Id = x.Command.Id } }).SingleOrDefault();

            //Get the command type id of the request
            int commandTypeId = _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.Id == objUserInput.RequestId).Select(x => x.CommandType.Id).SingleOrDefault();

            //Get current workflow definition level
            PSServiceRequestFlowDefinitionLevelDTO workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == objUserInput.RequestId).Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.FlowDefinitionLevel.Id, DefinitionId = x.FlowDefinitionLevel.Definition.Id, Position = x.FlowDefinitionLevel.Position }).FirstOrDefault();

            //Get the next approval workflow definition level after the current workflow definition level
            int nextWorkflowDefinitionLevelId = _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>().Where(x => (x.Definition.Id == workFlowDefLevel.DefinitionId) && (x.WorkFlowActionValue == (int)RequestDirection.Approval) && x.Position > workFlowDefLevel.Position).OrderBy(x => x.Position).First().Id;

            SetPreviousRequestCommandWorkflowLogsToInactive(objUserInput.RequestId);

            //Add the next level approver to the request command log
            AddRequestCommandWorkflowLog(objUserInput.RequestId, commandForRouteStage.Id, nextWorkflowDefinitionLevelId);

            PSSRequestVM request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == objUserInput.RequestId).Select(x => new PSSRequestVM { ServiceId = x.Service.Id, ServiceName = x.Service.Name, CustomerName = x.CBSUser.Name, FileRefNumber = x.FileRefNumber }).FirstOrDefault();

            //create the escort squad allocation group
            EscortSquadAllocationGroup allocModel = new EscortSquadAllocationGroup { AdminUser = new PSSAdminUsers { Id = adminUser.Id }, Comment = "POSSAP Secretariat has routed the request", Request = new PSSRequest { Id = objUserInput.RequestId }, RequestLevel = new EscortProcessStageDefinition { Id = objUserInput.SelectedRequestStage }, Service = new PSService { Id = request.ServiceId }, StatusDescription = "POSSAP Secretariat has routed the request" };
            _transactionManager.GetSession().Save(allocModel);

            //move to this flow definition level
            UpdatePSSRequest(objUserInput.RequestId, nextWorkflowDefinitionLevelId);
            SavePoliceServiceRequest(objUserInput.RequestId, workFlowDefLevel.Id, nextWorkflowDefinitionLevelId);

            return new SecretariatRoutingApprovalResponse
            {
                NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", request.ServiceName, request.FileRefNumber, request.CustomerName, "This application has been moved to " + commandForRouteStage.Name.Trim().TrimEnd(',')),
            };
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