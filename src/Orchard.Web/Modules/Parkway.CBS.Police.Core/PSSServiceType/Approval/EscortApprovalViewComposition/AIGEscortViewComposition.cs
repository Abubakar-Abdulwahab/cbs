using Orchard.Data;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts;
using Parkway.CBS.Core.Models;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.Logging;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition
{
    public class AIGEscortViewComposition : IEscortViewComposition
    {
        private ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }

        public int StageIdentifier => throw new NotImplementedException();


        public AIGEscortViewComposition()
        {
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// for AIG we need to get the list of formations per state
        /// and the list of formations the officer has assigned
        /// </summary>
        /// <param name="partialComp"></param>
        /// <returns>dynamic</returns>
        public dynamic SetPartialData(EscortPartialVM partialComp)
        {
            try
            {
                List<StateModel> states = _transactionManager.GetSession().Query<StateModel>().Where(s => s.IsActive)
                    .Select(s => new StateModel { LGAs = s.LGAs.Select(l => new LGA { Name = l.Name, Id = l.Id }).ToList(), Name = s.Name, ShortName = s.ShortName, Id = s.Id }).ToList();
                //we need to know the list of formation allocations tied to this level
                //first we need to get the level of this officer
                //get group allocations
                EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                        .Where(x => ((x.Request.Id == partialComp.RequestId))).SingleOrDefault();

                var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                    .Where(x => ((x.AdminUser.User.Id == partialComp.UserId) && (x.CommandType.Id == partialComp.CommandTypeId) && (x.IsActive))).First();
                //now we have the user, we need to get the level
                //from the def level I am able to get the squad alloc
                IEnumerable<EscortSquadAllocation> sallocs = _transactionManager.GetSession().Query<EscortSquadAllocation>().Where(sa => ((sa.CommandType.Id == partialComp.CommandTypeId) && (!sa.IsDeleted) && (sa.Command == thisProcessFlow.AdminUser.Command) && (sa.AllocationGroup == new EscortSquadAllocationGroup { Id = allocGrp.Id })));
                //check the main allocation to see if this is part of the base units is amongst what was allocated

                if (!sallocs.Any()) { throw new UserNotAuthorizedForThisActionException(); }

                List<EscortFormationAllocationDTO> formAllocs = sallocs.SelectMany(sa => sa.Formations.Where(s => !s.IsDeleted).Select(frmalloc => new EscortFormationAllocationDTO
                {
                    Id = frmalloc.Id,
                    StateName = frmalloc.State.Name,
                    StateId = frmalloc.State.Id,
                    LGAName = frmalloc.LGA.Name,
                    FormationName = frmalloc.Command.Name,
                    NumberOfOfficers = frmalloc.NumberOfOfficers,
                    NumberOfOfficersAssignedByCommander = frmalloc.NumberAssignedByCommander,
                    FormationId = frmalloc.Command.Id,
                    AllocationGroupId = frmalloc.Group.Id
                })).ToList();

                return new AIGApprovalVM { States = states, FormationsAllocated = formAllocs, NumberOfOfficersRequested = sallocs.ElementAt(0).NumberOfOfficers, RequestId = partialComp.RequestId, CanApprove = CanApprove(new List<EscortPartialVM> { partialComp }, partialComp.RequestId, new EscortRequestDetailsVM { EscortInfo = new EscortRequestVM { SelectedCommandType = partialComp.CommandTypeId } }, partialComp.UserId) };
            }
            catch (Exception)
            { throw; }
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
        /// Validates selected formations
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public bool DoValidation(EscortPartialVM item, EscortRequestDetailsVM objUserInput, ref List<ErrorModel> errors)
        {
            ///
            EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                       .Where(x => ((x.Request.Id == objUserInput.RequestId))).SingleOrDefault();

            var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                .Where(x => ((x.AdminUser.User.Id == objUserInput.ApproverId) && (x.CommandType.Id == item.CommandTypeId) && (x.IsActive))).First();
            //now we have the user, we need to get the level
            //from the def level I am able to get the squad alloc
            IEnumerable<EscortSquadAllocation> sallocs = _transactionManager.GetSession().Query<EscortSquadAllocation>().Where(sa => ((sa.CommandType.Id == item.CommandTypeId) && (!sa.IsDeleted) && (sa.Command == thisProcessFlow.AdminUser.Command) && (sa.AllocationGroup == new EscortSquadAllocationGroup { Id = allocGrp.Id })));
            //check the main allocation to see if this is part of the base units is amongst what was allocated

            if (!sallocs.Any()) { throw new UserNotAuthorizedForThisActionException(); }
            ///


            objUserInput.FormationsSelection = (objUserInput.FormationsSelection == null) ? new List<AIGFormationVM> { } : objUserInput.FormationsSelection;
            objUserInput.RemovedFormations = (objUserInput.RemovedFormations == null) ? new List<AIGFormationVM> { } : objUserInput.RemovedFormations;
            //here we validate if a formation was selected on the admin interface

            var session = _transactionManager.GetSession();
            //get the admin command code which would be used to validate the selected formations to ensure that they are formations under the admin
            string adminCommandCode = session.Query<PSSAdminUsers>().Where(x => x.User == new Orchard.Users.Models.UserPartRecord { Id = objUserInput.ApproverId }).Select(x => x.Command.Code).SingleOrDefault();

            HelperModels.CommandVM vm = null;
            //here we validate that the selected formation is a next level formation of the admin command
            //if (objUserInput.FormationsSelection != null)
            foreach (var formation in objUserInput.FormationsSelection)
            {
                vm = session.Query<Command>().Where(x => x.Id == formation.FormationId && x.Code.Like($"%{adminCommandCode}-%")).Select(x =>
                new HelperModels.CommandVM
                {
                    Id = x.Id,
                    StateId = x.State.Id,
                    LGAId = x.LGA.Id
                }).SingleOrDefault();
                if (vm == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Formation selected is not valid", FieldName = "FormationsSelection" });
                    return true;
                }
                formation.LGAId = vm.LGAId;
                formation.StateId = vm.StateId;

                if (formation.NumberofOfficers == 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Formation selected requires number of officers to be specified", FieldName = "FormationsSelection" });
                    return true;
                }
            }

            if (objUserInput.RemovedFormations != null)
            {
                foreach (var removal in objUserInput.RemovedFormations)
                {
                    vm = session.Query<Command>().Where(x => x.Id == removal.FormationId && x.Code.Like($"%{adminCommandCode}-%")).Select(x =>
                    new HelperModels.CommandVM
                    {
                        Id = x.Id,
                        StateId = x.State.Id,
                        LGAId = x.LGA.Id
                    }).SingleOrDefault();
                    if (vm == null)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Formation selected is not valid", FieldName = "FormationsSelection" });
                        return true;
                    }
                    removal.LGAId = vm.LGAId;
                    removal.StateId = vm.StateId;
                }
            }

            return false;
        }


        /// <summary>
        /// Saves model records to EscortFormationAllocation
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public void SaveRecords(EscortPartialVM item, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors)
        {
            ///
            EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                       .Where(x => ((x.Request.Id == objUserInput.RequestId))).SingleOrDefault();

            var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                .Where(x => ((x.AdminUser.User.Id == objUserInput.ApproverId) && (x.CommandType.Id == item.CommandTypeId) && (x.IsActive))).First();
            //now we have the user, we need to get the level
            //from the def level I am able to get the squad alloc
            IEnumerable<EscortSquadAllocation> sallocs = _transactionManager.GetSession().Query<EscortSquadAllocation>().Where(sa => ((sa.CommandType.Id == item.CommandTypeId) && (!sa.IsDeleted) && (sa.Command == thisProcessFlow.AdminUser.Command) && (sa.AllocationGroup == new EscortSquadAllocationGroup { Id = allocGrp.Id })));
            //check the main allocation to see if this is part of the base units is amongst what was allocated

            if (!sallocs.Any()) { throw new UserNotAuthorizedForThisActionException(); }
            ///

            var session = _transactionManager.GetSession();

            PSSAdminUsersVM pssAdminUser = session.Query<PSSAdminUsers>().Where(x => x.User == new Orchard.Users.Models.UserPartRecord { Id = objUserInput.ApproverId }).Select(x => new PSSAdminUsersVM
            {
                Id = x.Id,
                Command = new HelperModels.CommandVM { Id = x.Command.Id }
            }).SingleOrDefault();

            long escortSquadAllocationGroupId = allocGrp.Id;

            long escortSquadAllocationId = sallocs.ElementAt(0).Id;

            if (escortSquadAllocationGroupId == 0 || escortSquadAllocationId == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Request has not been assigned to your command", FieldName = "FormationsSelection" });
                return;
            }

            string statusDescription = "PendingAdminApproval";

            //if (objUserInput.AllocationGroupId != 0)
            {
                //Edit logic, this is executed if the currently logged in admin belongs to a command that is listed as part of the allocations for the EscortSquadAllocationGroup mapped to the current request id
                string reference = string.Format("ESC-{0}-ALLOC-{1}-IMPL-{2}", DateTime.Now.Ticks, escortSquadAllocationGroupId, item.ImplementationClass.GetHashCode());

                string formationAllocationSelectionTrackingTableName = "Parkway_CBS_Police_Core_" + typeof(EscortFormationAllocationSelectionTracking).Name;
                string squadAllocationSelectionTrackingQueryText = string.Empty;

                foreach (var addition in objUserInput.FormationsSelection)
                {
                    squadAllocationSelectionTrackingQueryText = $"INSERT INTO {formationAllocationSelectionTrackingTableName}(Command_Id, NumberOfOfficers, IsDeleted, State_Id, LGA_Id, Group_Id, AllocatedByAdminUser_Id, EscortSquadAllocation_Id, Reference, CreatedAtUtc, UpdatedAtUtc) VALUES({addition.FormationId}, {addition.NumberofOfficers}, 0, {addition.StateId}, {addition.LGAId}, {escortSquadAllocationGroupId}, {pssAdminUser.Id},{escortSquadAllocationId}, '{reference}', GETDATE(), GETDATE())";

                    session.CreateSQLQuery(squadAllocationSelectionTrackingQueryText).ExecuteUpdate();
                }

                foreach (var removal in objUserInput.RemovedFormations)
                {
                    squadAllocationSelectionTrackingQueryText = $"INSERT INTO {formationAllocationSelectionTrackingTableName}(Command_Id, NumberOfOfficers, IsDeleted, State_Id, LGA_Id, Group_Id, AllocatedByAdminUser_Id, EscortSquadAllocation_Id, Reference, CreatedAtUtc, UpdatedAtUtc) VALUES({removal.FormationId}, {removal.NumberofOfficers}, 1, {removal.StateId}, {removal.LGAId}, {escortSquadAllocationGroupId},{pssAdminUser.Id},{escortSquadAllocationId}, '{reference}', GETDATE(), GETDATE())";

                    session.CreateSQLQuery(squadAllocationSelectionTrackingQueryText).ExecuteUpdate();
                }

                //merge query comes in here, this updates the EscortFormationAllocation table with the EscortFormationAllocationSelectionTracking table
                string escortSquadAllocationSyncQuery = $"MERGE Parkway_CBS_Police_Core_EscortFormationAllocation AS Target USING Parkway_CBS_Police_Core_EscortFormationAllocationSelectionTracking AS Source ON Source.Group_Id = Target.Group_Id AND Source.Command_Id = Target.Command_Id " +
                    $"WHEN MATCHED AND Source.Reference = '{reference}' THEN UPDATE SET Target.IsDeleted = Source.IsDeleted, Target.NumberOfOfficers = Source.NumberOfOfficers, Target.UpdatedAtUtc = GETDATE() WHEN NOT MATCHED BY Target AND Source.Reference = '{reference}' " +
                    $"THEN INSERT(AllocatedByAdminUser_Id, Group_Id,EscortSquadAllocation_Id, State_Id, LGA_Id, Command_Id, NumberOfOfficers, NumberAssignedByCommander,StatusDescription,Fulfilled, CreatedAtUtc, UpdatedAtUtc, IsDeleted) VALUES(Source.AllocatedByAdminUser_Id, Source.Group_Id,Source.EscortSquadAllocation_Id,Source.State_Id, Source.LGA_Id, Source.Command_Id, Source.NumberOfOfficers, 0, '{statusDescription}', 0,  GETDATE(), GETDATE(), 0);";

                session.CreateSQLQuery(escortSquadAllocationSyncQuery).ExecuteUpdate();

                SaveApprovalLog(objUserInput);
                return;
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

                if (objUserInput.FormationsSelection.Any() && objUserInput.RemovedFormations.Any())
                {
                    //Added and removed formations
                    additionalComment = $"The request for officers has been assigned to {string.Join(", ", objUserInput.FormationsSelection.Select(x => $"{x.FormationName} to provide {x.NumberofOfficers} officers"))} and the {string.Join(", ", objUserInput.RemovedFormations.Select(x => x.FormationName))} has been removed from assigning officers.";
                }
                else if (objUserInput.FormationsSelection.Any() && !objUserInput.RemovedFormations.Any())
                {
                    //Added formations
                    additionalComment = $"The request for officers has been assigned to {string.Join(", ", objUserInput.FormationsSelection.Select(x => $"{x.FormationName} to provide {x.NumberofOfficers} officers"))}.";
                }
                else if (!objUserInput.FormationsSelection.Any() && objUserInput.RemovedFormations.Any())
                {
                    //Removed formations
                    additionalComment = $"The {string.Join(", ", objUserInput.RemovedFormations.Select(x => x.FormationName))} has been removed from assigning officers.";
                }

                objUserInput.Comment = $"{adminName} (AIG): {objUserInput.Comment.TrimEnd('.')}. {additionalComment}";

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


        /// <summary>
        /// On submit is used to subnit the request to the
        /// formations that are to assign the officers
        /// </summary>
        /// <param name="approverId"></param>
        /// <param name="requestId"></param>
        /// <param name="commandTypeId"></param>
        /// <returns></returns>
        public RequestApprovalResponse OnSubmit(int adminUserId, long requestId, int commandTypeId)
        {
            try
            {
                int workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == requestId).Select(x => x.FlowDefinitionLevel.Id).FirstOrDefault();

                var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                    .Where(x => ((x.AdminUser.User.Id == adminUserId) && (x.CommandType.Id == commandTypeId) && (x.IsActive)))
                    .Select(x => new EscortProcessFlowVM { LevelId = x.Level.Id, CommandId = x.AdminUser.Command.Id, LevelGroupIdentifier = x.Level.LevelGroupIdentifier })
                    .First();

                //get the level where their parent is me
                //this would indicate the level after me, I can have many levels so far they inherit from me
                var childLevels = _transactionManager.GetSession().Query<EscortProcessStageDefinition>()
                    .Where(x => ((x.ParentDefinition.Id == thisProcessFlow.LevelId) && (x.CommandType.Id == commandTypeId) && (x.IsActive))).ToList();

                ///
                EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                       .Where(x => ((x.Request.Id == requestId))).SingleOrDefault();
                //now we have the user, we need to get the level
                //from the def level I am able to get the squad alloc
                IEnumerable<EscortSquadAllocation> sallocs = _transactionManager.GetSession().Query<EscortSquadAllocation>().Where(sa => ((sa.CommandType.Id == commandTypeId) && (!sa.IsDeleted) && (sa.Command.Id == thisProcessFlow.CommandId) && (sa.AllocationGroup == new EscortSquadAllocationGroup { Id = allocGrp.Id })));
                //check the main allocation to see if this is part of the base units is amongst what was allocated

                if (!sallocs.Any()) { throw new UserNotAuthorizedForThisActionException(); }
                ///
                //for the officers that have been assigned set the isdeleted flag to true
                //do an update
                string updateToDeleted = $"UPDATE [dbo].[Parkway_CBS_Police_Core_EscortFormationOfficer] SET IsDeleted = 1 FROM [dbo].[Parkway_CBS_Police_Core_EscortFormationOfficer] efo INNER JOIN [dbo].[Parkway_CBS_Police_Core_EscortFormationAllocation] formation ON formation.IsDeleted = 1 AND efo.FormationAllocation_Id = formation.Id";
                _transactionManager.GetSession().CreateSQLQuery(updateToDeleted).ExecuteUpdate();
                //now that we have set is deleted to true on sqaud officers table
                //we need to set the ones that are active to the approver list
                //now we need to get the next level users
                IEnumerable<EscortProcessFlow> nextLevelapprovers = childLevels.Select(c => c.AssignedFlow).SelectMany(v => v);
                //we have gotten the next level users
                IEnumerable<PSSAdminUsers> usersToNotify = nextLevelapprovers.Select(ad => ad.AdminUser);
                //now we have the list of users on this level, we need to filter by selected sqaud formation
                //IEnumerable<EscortFormationOfficer> activeSqaudAllocs = nextLevelapprovers.AssignedFormationOfficers.Where(x => !x.IsDeleted);
                //Parkway_CBS_Police_Core_EscortFormationAllocation
                IEnumerable<EscortFormationAllocation> assignedFormations = sallocs.ElementAt(0).Formations.Where(f => !f.IsDeleted);
                IEnumerable<Command> assignedCommands = assignedFormations.Select(x => x.Command);

                //once we know the commands that have been assigned we need to filter out from the next level approvers
                foreach (var item in usersToNotify)
                {
                    if (assignedCommands.Any(x => item.Command.Code == x.Code))
                    {
                        //if part of the assigned commands let's check if it has been added to the approvers list
                        if (_transactionManager.GetSession().Query<EscortFormationOfficer>().Count(x => (x.FormationAllocation.Command.Code == item.Command.Code) && (x.Group.Request.Id == requestId)) == 0)
                        {
                            if (_transactionManager.GetSession().Query<RequestCommandWorkFlowLog>().Count(x => (x.Request.Id == requestId) && (x.Command.Id == item.Command.Id) && (x.DefinitionLevel.Id == workFlowDefLevel)) > 0)
                            {
                                UpdateRequestCommandWorkflowLog(requestId, item.Command.Id, workFlowDefLevel, true);
                            }
                            else 
                            { 
                                AddRequestCommandWorkflowLog(requestId, item.Command.Id, workFlowDefLevel);
                                //Update the request phase for the AIG to ongoing
                                UpdateRequestCommandWorkflowLog(requestId, thisProcessFlow.CommandId, workFlowDefLevel, true);
                            }
                        }
                    }
                    else
                    {
                        UpdateRequestCommandWorkflowLog(requestId, item.Command.Id, workFlowDefLevel, false);
                    }
                }
                //now that we have treated the users to view the app we need to decide on what the next flow is
                int adminLevel = thisProcessFlow.LevelGroupIdentifier;
                int presentProcessLevel = allocGrp.RequestLevel.LevelGroupIdentifier;
                if (adminLevel == presentProcessLevel)
                {
                    //move to squad commander level
                    if (assignedFormations.Count() > 0)
                    {
                        allocGrp.RequestLevel = new EscortProcessStageDefinition { Id = nextLevelapprovers.First().Level.Id };
                        allocGrp.StatusDescription = "Moved to Formation units.";
                        allocGrp.Comment = "Moved to Formation units.";
                    }
                }
                string messages = "";

                PSSRequest request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == requestId).FirstOrDefault();
                return new RequestApprovalResponse
                {
                    ServiceType = request.Service.ServiceType.ToString(),
                    FileNumber = request.FileRefNumber,
                    CustomerName = request.CBSUser.Name,
                    NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", (PSSServiceTypeDefinition)request.Service.ServiceType, request.FileRefNumber, request.CBSUser.Name, "This application has been submitted." + messages.Trim().TrimEnd(',')),
                    ResponseFromPartial = true,
                };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public CanApproveEscortVM CanApprove(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int adminUserId)
        {
            //first we need to check if the escort formation officer allocation has approved their request
            int workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == requestId).Select(x => x.FlowDefinitionLevel.Id).FirstOrDefault();

            var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                .Where(x => ((x.AdminUser.User.Id == adminUserId) && (x.CommandType.Id == escort.EscortInfo.SelectedCommandType) && (x.IsActive))).First();

            ///
            EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                   .Where(x => ((x.Request.Id == requestId))).SingleOrDefault();
            //now we have the user, we need to get the level
            //from the def level I am able to get the squad alloc
            IEnumerable<EscortSquadAllocation> sallocs = _transactionManager.GetSession().Query<EscortSquadAllocation>().Where(sa => ((sa.CommandType.Id == escort.EscortInfo.SelectedCommandType) && (!sa.IsDeleted) && (sa.Command == thisProcessFlow.AdminUser.Command) && (sa.AllocationGroup == new EscortSquadAllocationGroup { Id = allocGrp.Id })));
            //check the main allocation to see if this is part of the base units is amongst what was allocated

            if (!sallocs.Any()) { throw new UserNotAuthorizedForThisActionException(); }
            //first we need to know if we have any formations for this squad
            int formationCount = sallocs.ElementAt(0).Formations.Where(f => !f.IsDeleted).Count();
            if (formationCount < 1) { return new CanApproveEscortVM { Message = "No assigned formations found", CanApprove = false }; }
            //here we have the squad allocation
            IEnumerable<KVHolder> formationNotFulfilled = sallocs.ElementAt(0).Formations.Where(f => ((!f.Fulfilled) && (!f.IsDeleted))).Select(s => new KVHolder { Name = s.Command.Name, NumberOfOfficers = s.NumberOfOfficers });
            if (formationNotFulfilled.Count() > 0)
            {
                //if this allocation has pending allocations
                //we need to check that it has a least on allocation that has officers assigned to it
                //so we check if the allocation has been fulfilled
                IEnumerable<KVHolder> formationFulfilled = sallocs.ElementAt(0).Formations.Where(f => ((f.Fulfilled) && (!f.IsDeleted))).Select(s => new KVHolder { Name = s.Command.Name, NumberOfOfficers = s.NumberOfOfficers });
                if (formationFulfilled.Count() > 0)
                {
                    return new CanApproveEscortVM { Message = "Formation(s): " + string.Join(", ", formationFulfilled.Select(f => (f.Name + " with " + f.NumberOfOfficers.ToString() + "officer(s) ")).ToArray()) + " have assigned officers. Formations " + string.Join(", ", formationNotFulfilled.Select(f => (f.Name + " with " + f.NumberOfOfficers.ToString() + " officer(s)")).ToArray()) + " have not fulfilled their allocation. Would you like to proceed with the assignment?", CanApprove = true };
                }
            }
            return new CanApproveEscortVM { CanApprove = true };
        }


        public EscortApprovalMessage Approval(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int adminUserId)
        {
            //first we need to check if the escort formation officer allocation has approved their request
            int workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == requestId).Select(x => x.FlowDefinitionLevel.Id).FirstOrDefault();

            var thisProcessFlow = _transactionManager.GetSession().Query<EscortProcessFlow>()
                .Where(x => ((x.AdminUser.User.Id == adminUserId) && (x.CommandType.Id == escort.EscortInfo.SelectedCommandType) && (x.IsActive))).First();

            var escortDetailsId = _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.Id == requestId).Select(x => x.Id).SingleOrDefault();

            ///
            EscortSquadAllocationGroup allocGrp = _transactionManager.GetSession().Query<EscortSquadAllocationGroup>()
                   .Where(x => ((x.Request.Id == requestId))).SingleOrDefault();
            //now we have the user, we need to get the level
            //from the def level I am able to get the squad alloc
            IEnumerable<EscortSquadAllocation> sallocs = _transactionManager.GetSession().Query<EscortSquadAllocation>().Where(sa => ((sa.CommandType.Id == escort.EscortInfo.SelectedCommandType) && (!sa.IsDeleted) && (sa.Command == thisProcessFlow.AdminUser.Command) && (sa.AllocationGroup == new EscortSquadAllocationGroup { Id = allocGrp.Id })));
            //check the main allocation to see if this is part of the base units is amongst what was allocated

            if (!sallocs.Any()) { throw new UserNotAuthorizedForThisActionException(); }

            //here we have the squad allocation
            int formationNotFulfilled = sallocs.ElementAt(0).Formations.Count(f => ((!f.Fulfilled) && (!f.IsDeleted)));
            //if we have formations that have not fulfilled their  
            CanApproveEscortVM result = CanApprove(partials, requestId, escort, adminUserId);

            if (!result.CanApprove) { throw new UserNotAuthorizedForThisActionException { }; }

            //set the fulfilled flag for all formations of this AIG that have not been deleted to true
            foreach (var formation in sallocs.ElementAt(0).Formations.Where(x => !x.IsDeleted))
            {
                formation.Fulfilled = true;
            }

            sallocs.ElementAt(0).Fulfilled = true;
            sallocs.ElementAt(0).StatusDescription = "Officer assignment have been approved";
            //mow we are ignoring any unfulfilled allocation by the formations under this AIG
            //we now need to look at the bigger request,
            //we check if the DIG has assigned any allocation to other AIGs
            IEnumerable<EscortSquadAllocation> allocationsFromGroupThaAreUnfulfilled = allocGrp.Allocations.Where(s => ((!s.IsDeleted) && (!s.Fulfilled) && (s.Command != thisProcessFlow.AdminUser.Command)));

            //if we have unfulfilled allocation we have to pause the application here
            if (allocationsFromGroupThaAreUnfulfilled.Count() > 0)
            {
                return new EscortApprovalMessage { Message = "Pending allocations from DIG for " + string.Join(",", allocationsFromGroupThaAreUnfulfilled.Select(a => a.Command.Name)) };
            }

            //if we have no allocation pending on the DIG level
            //we need to check for this sqaud formation level
            //that is check for other formations that the AIG has assigned to
            var otherFormationAssignedToThisAIG = sallocs.ElementAt(0).Formations.Where(f => ((!f.IsDeleted) && (!f.Fulfilled)));
            //if we have formations that have not been fulfilled
            //we need to break from this process
            if (otherFormationAssignedToThisAIG.Count() > 0)
            {
                return new EscortApprovalMessage { Message = "Pending allocation/officer assignments " + string.Join(", ", otherFormationAssignedToThisAIG.Select(f => (f.Command.Name))) };
            }
            //now that we have not other level process pending we proceed to invoice generation


            //now that we have the allocation for the group we need to search for 
            //now we have ascertained that we can apporve, we need to know whether to move on with this application of wait
            var digofficeAllocs = sallocs.ElementAt(0).AllocationGroup.Allocations.Where(f => !f.IsDeleted);
            //get the number of untreated allocations
            var didOfficeWithoutFormatiions = digofficeAllocs.Where(f => (!f.Formations.Any() && !f.IsDeleted));
            if (didOfficeWithoutFormatiions.Any())
            {
                //here we have allocation that haven't assigned any formation
                return new EscortApprovalMessage { Message = string.Join(",", didOfficeWithoutFormatiions.Select(i => (i.Command.Name + " with " + i.NumberOfOfficers + " number of officers has not fulfilled their allocation."))) };
            }

            IEnumerable<EscortSquadAllocation> digOfficerWithFormations = digofficeAllocs.Where(f => f.Formations.Any() && !f.IsDeleted);
            IEnumerable<EscortFormationAllocation> digOfficeFormationsNoAlloc = digOfficerWithFormations.SelectMany(x => x.Formations);
            //get formation that don't have an officer assigned
            var noOfficerAssigned = digOfficeFormationsNoAlloc.Where(x => (!x.SquadronOfficers.Any() && !x.IsDeleted));
            if (noOfficerAssigned.Any())
            {
                return new EscortApprovalMessage { Message = string.Join(",", noOfficerAssigned.Select(i => (i.Command.Name + " with " + i.NumberOfOfficers + " number of officers has not fulfilled their allocation."))) };
            }
            //now we have officers that are assigned
            var officersAssigned = digOfficeFormationsNoAlloc.Count(x => (x.SquadronOfficers.Any() && !x.IsDeleted));
            SetAssignedOfficersValueToTrue(escortDetailsId);
            //var officersAssigned  = digOfficerWithFormations
            var digoffice = sallocs.ElementAt(0).AllocationGroup.Allocations.Where(alloc => alloc.Formations.Any(f => ((!f.Fulfilled) && (!f.IsDeleted)))).Select(k => new KVHolder { Name = k.Command.Name, NumberOfOfficers = k.NumberOfOfficers });

            if (digoffice.Count() > 0)
            {
                return new EscortApprovalMessage { Message = string.Join(",", digoffice.Select(i => (i.Name + " with " + i.NumberOfOfficers + " number of officers have not fulfilled their allocation."))) };
            }

            //if the dig officer has no pending approvals, let proceed with the next level
            //update group, squad, formation and officers
            allocGrp.UpdatedAtUtc = DateTime.Now.ToLocalTime();
            allocGrp.StatusDescription = thisProcessFlow.AdminUser.Fullname + " approved at " + DateTime.Now.ToLocalTime().ToString("dd/MM/YYYY");
            allocGrp.Fulfilled = true;

            sallocs.ElementAt(0).UpdatedAtUtc = DateTime.Now.ToLocalTime();
            sallocs.ElementAt(0).StatusDescription = thisProcessFlow.AdminUser.Fullname + " approved at " + DateTime.Now.ToLocalTime().ToString("dd/MM/YYYY");
            sallocs.ElementAt(0).Fulfilled = true;

            //string updateQuery = $"UPDATE [dbo].[Parkway_CBS_Police_Core_EscortSquadAllocation] SET Fulfilled = 1, StatusDescription = '{thisProcessFlow.AdminUser.Fullname} approved at {DateTime.Now.ToLocalTime().ToString("dd/MM/YYYY")}', UpdatedAtUtc = {DateTime.Now.ToLocalTime()} FROM [dbo].[Parkway_CBS_Police_Core_EscortFormationOfficer] efo INNER JOIN [dbo].[Parkway_CBS_Police_Core_EscortFormationAllocation] formation ON formation.IsDeleted = 1";

            //_transactionManager.GetSession().CreateSQLQuery(updateQuery).ExecuteUpdate();
            UpdateRequestCommandWorkflowLog(requestId, thisProcessFlow.AdminUser.Command.Id, workFlowDefLevel, false);
            return new EscortApprovalMessage { CanApproveRequest = true };
        }


        private void SetAssignedOfficersValueToTrue(long escortDetailsId)
        {
            string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSEscortDetails).Name;
            string assignedOfficerFlag = nameof(PSSEscortDetails.OfficersHaveBeenAssigned);
            string updatedAtName = nameof(PSSEscortDetails.UpdatedAtUtc);
            string escortIdName = nameof(PSSEscortDetails.Id);

            var queryText = $"UPDATE esd SET esd.{assignedOfficerFlag} = :boolval, esd.{updatedAtName} = :updateDate FROM {tableName} esd WHERE {escortIdName} = :escortDetailsId";
            var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
            query.SetParameter("boolval", true);
            query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
            query.SetParameter("escortDetailsId", escortDetailsId);

            query.ExecuteUpdate();
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
        /// Routes the request to AIG level
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToThisEscortStage(EscortPartialVM item, EscortRequestDetailsVM objUserInput)
        {
            //This is where we perform all the actions that would've otherwise been performed by the IG and DIG before getting to this level
            //Get tactical squad for this stage
            CommandVM tacticalSquadForRouteStage = _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => (x.Level.Id == objUserInput.SelectedRequestStage) && (x.IsActive)).Select(x => new CommandVM { Id = x.AdminUser.Command.Id, Name = x.AdminUser.Command.Name }).FirstOrDefault();

            //-Save Records for IG and DIG
            PSSAdminUsersVM adminUser = _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User == new Orchard.Users.Models.UserPartRecord { Id = objUserInput.ApproverId }).Select(x => new PSSAdminUsersVM { Id = x.Id, Command = new CommandVM { Id = x.Command.Id } }).SingleOrDefault();
            EscortDetailsDTO escortDetails = _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.Id == objUserInput.RequestId).Select(x => new EscortDetailsDTO { CommandTypeId = x.CommandType.Id, NumberOfOfficers = x.NumberOfOfficers }).SingleOrDefault();

            //Get current workflow definition level
            PSServiceRequestFlowDefinitionLevelDTO workFlowDefLevel = _transactionManager.GetSession().Query<PSSRequest>()
                        .Where(r => r.Id == objUserInput.RequestId).Select(x => new PSServiceRequestFlowDefinitionLevelDTO { Id = x.FlowDefinitionLevel.Id, DefinitionId = x.FlowDefinitionLevel.Definition.Id, Position = x.FlowDefinitionLevel.Position }).FirstOrDefault();

            //Get the next approval workflow definition level after the current workflow definition level
            int nextWorkflowDefinitionLevelId = _transactionManager.GetSession().Query<PSServiceRequestFlowDefinitionLevel>().Where(x => (x.Definition.Id == workFlowDefLevel.DefinitionId) && (x.WorkFlowActionValue == (int)RequestDirection.Approval) && x.Position > workFlowDefLevel.Position).OrderBy(x => x.Position).First().Id;

            SetPreviousRequestCommandWorkflowLogsToInactive(objUserInput.RequestId);

            //Add the next level approver to the request command log
            AddRequestCommandWorkflowLog(objUserInput.RequestId, tacticalSquadForRouteStage.Id, nextWorkflowDefinitionLevelId);

            //Ensures the request is still visible for this admin
            UpdateRequestCommandWorkflowLog(objUserInput.RequestId, adminUser.Command.Id, workFlowDefLevel.Id, true);

            string statusDescription = "Pending Admin Approval";

            PSSRequestVM request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == objUserInput.RequestId).Select(x => new PSSRequestVM { ServiceId = x.Service.Id, ServiceName = x.Service.Name, CustomerName = x.CBSUser.Name, FileRefNumber = x.FileRefNumber }).FirstOrDefault();

            //create the escort squad allocation group
            EscortSquadAllocationGroup allocModel = new EscortSquadAllocationGroup { AdminUser = new PSSAdminUsers { Id = adminUser.Id }, Comment = "POSSAP Secretariat has routed the request", Request = new PSSRequest { Id = objUserInput.RequestId }, RequestLevel = new EscortProcessStageDefinition { Id = objUserInput.SelectedRequestStage }, Service = new PSService { Id = request.ServiceId }, StatusDescription = "POSSAP Secretariat has routed the request" };
            _transactionManager.GetSession().Save(allocModel);

            objUserInput.AllocationGroupId = allocModel.Id;

            //Create an entry in the escort squad allocation table for this tactical squad
            string squadAllocationTableName = "Parkway_CBS_Police_Core_" + typeof(EscortSquadAllocation).Name;
            string squadAllocationQueryText = $"INSERT INTO {squadAllocationTableName}(Command_Id, NumberOfOfficers, IsDeleted, AllocationGroup_Id, Fulfilled, StatusDescription, CommandType_Id, CreatedAtUtc, UpdatedAtUtc) VALUES({tacticalSquadForRouteStage.Id}, {escortDetails.NumberOfOfficers}, 0, {objUserInput.AllocationGroupId}, 0, '{statusDescription}', {escortDetails.CommandTypeId}, GETDATE(), GETDATE())";

            _transactionManager.GetSession().CreateSQLQuery(squadAllocationQueryText).ExecuteUpdate();

            //move to this flow definition level
            UpdatePSSRequest(objUserInput.RequestId, nextWorkflowDefinitionLevelId);
            SavePoliceServiceRequest(objUserInput.RequestId, workFlowDefLevel.Id, nextWorkflowDefinitionLevelId);

            return new SecretariatRoutingApprovalResponse
            {
                NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", request.ServiceName, request.FileRefNumber, request.CustomerName, "This application has been moved to the AIG " + tacticalSquadForRouteStage.Name.Trim().TrimEnd(',')),
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
    }

    public class KVHolder
    {
        public string Name { get; set; }

        public int NumberOfOfficers { get; set; }
    }
}