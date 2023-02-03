using NHibernate.Linq;
using Orchard.Data;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition
{
    public class SquadronLeaderEscortViewComposition : IEscortViewComposition
    {
        private ITransactionManager _transactionManager;
        public ILogger Logger { get; set; }
        public int StageIdentifier => throw new System.NotImplementedException();

        public SquadronLeaderEscortViewComposition()
        {
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Validates selected police officers
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public bool DoValidation(EscortPartialVM item, EscortRequestDetailsVM objUserInput, ref List<ErrorModel> errors)
        {
            objUserInput.OfficersSelection = (objUserInput.OfficersSelection == null) ? new List<ProposedEscortOffficerVM> { } : objUserInput.OfficersSelection;
            objUserInput.RemovedOfficersSelection = (objUserInput.RemovedOfficersSelection == null) ? new List<ProposedEscortOffficerVM> { } : objUserInput.RemovedOfficersSelection;

            var session = _transactionManager.GetSession();

            EscortDetailsDTO escort = session.Query<PSSEscortDetails>().Where(x => x.Request == new PSSRequest { Id = objUserInput.RequestId }).Select(esd => new EscortDetailsDTO
            {
                LGAId = esd.LGA.Id,
                StateId = esd.State.Id,
                PSSEscortServiceCategoryId = esd.CategoryType.Id,
                Id = esd.Id,
            }).SingleOrDefault();
            int adminCommandId = session.Query<PSSAdminUsers>().Where(x => x.User == new Orchard.Users.Models.UserPartRecord { Id = objUserInput.ApproverId }).Select(x => x.Command.Id).SingleOrDefault();


            PoliceOfficerLogVM officerLog = null;
            foreach (var officer in objUserInput.OfficersSelection)
            {
                //here we validate if the police officer exists and that the selected police officer belongs to the same command as the approving officer(squadron leader aka fourth approver)
                officerLog = session.Query<PolicerOfficerLog>().Where(x => ((x.Id == officer.PoliceOfficerLogId)))
                    .Select(x => new PoliceOfficerLogVM { Id = x.Id, IdentificationNumber = x.IdentificationNumber, CommandId = x.Command.Id, RankId = x.Rank.Id }).SingleOrDefault();

                if (officerLog == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = CBS.Core.Lang.ErrorLang.record404().ToString(), FieldName = "OfficersSelection" });
                    return true;
                }

                if(officerLog.CommandId != adminCommandId)
                {
                    errors.Add(new ErrorModel { ErrorMessage = Lang.PoliceErrorLang.ToLocalizeString("Command mismatch, officer assigned does not belong to the same command as the logged in user.").Text, FieldName = "OfficersSelection" });
                    return true;
                }

                //here we validate if the police officer is in an active deployment
                if (session.Query<PoliceOfficerDeploymentLog>().Count(x => x.PoliceOfficerLog.IdentificationNumber == officerLog.IdentificationNumber && x.IsActive) > 0)
                {
                    errors.Add(new ErrorModel { ErrorMessage = Lang.PoliceErrorLang.police_officer_is_in_active_deployment.ToString(), FieldName = "OfficersSelection" });
                    return true;
                }

                officer.OfficerCommandId = officerLog.CommandId;
                officer.OfficerRankId = officerLog.RankId;
                officer.EscortRankRate = GetRateSheetId(officer.OfficerRankId, escort.PSSEscortServiceCategoryId, escort.StateId, escort.LGAId, item.CommandTypeId);
                if (officer.EscortRankRate <= 0) { throw new DirtyFormDataException("No set rate for this rank with ID " + officer.OfficerRankId + " and officer " + officer.OfficerName); }
            }


            foreach (var removal in objUserInput.RemovedOfficersSelection)
            {
                officerLog = session.Query<PolicerOfficerLog>().Where(x => x.Id == removal.PoliceOfficerLogId && x.Command.Id == adminCommandId).Select(x => new PoliceOfficerLogVM { Id = x.Id, IdentificationNumber = x.IdentificationNumber, CommandId = x.Command.Id, RankId = x.Rank.Id }).SingleOrDefault();
                if (officerLog == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = CBS.Core.Lang.ErrorLang.record404().ToString(), FieldName = "OfficersSelection" });
                    return true;
                }

                removal.OfficerRankId = officerLog.RankId;
                removal.EscortRankRate = GetRateSheetId(removal.OfficerRankId, escort.PSSEscortServiceCategoryId, escort.StateId, escort.LGAId, item.CommandTypeId);
            }

            return false;
        }


        public dynamic SetPartialData(EscortPartialVM partialComp)
        {
            var session = _transactionManager.GetSession();
            int adminCommandId = session.Query<PSSAdminUsers>().Where(x => x.User == new Orchard.Users.Models.UserPartRecord { Id = partialComp.UserId }).Select(x => x.Command.Id).SingleOrDefault();
            long escortAllocationGroupId = session.Query<EscortSquadAllocationGroup>().Where(x => x.Request == new PSSRequest { Id = partialComp.RequestId }).Select(x => x.Id).SingleOrDefault();
            long formationAllocationId = session.Query<EscortFormationAllocation>()
                .Where(x => ((x.Group == new EscortSquadAllocationGroup { Id = escortAllocationGroupId }) && (x.Command == new Command { Id = adminCommandId }) && (!x.IsDeleted)))
                .Select(x => x.Id).SingleOrDefault();

            if (formationAllocationId == 0) { throw new UserNotAuthorizedForThisActionException { }; }
            return new SquadronLeaderApprovalVM
            {
                ProposedEscortOffficers = _transactionManager.GetSession().Query<EscortFormationOfficer>().Where(x => ((x.FormationAllocation.Id == formationAllocationId) && (x.Group.Id == escortAllocationGroupId) && (!x.IsDeleted)))
                .Select(x => new ProposedEscortOffficerVM { PoliceOfficerLogId = x.PoliceOfficerLog.Id, OfficerRankId = x.PoliceOfficerLog.Rank.Id, OfficerRankName = x.PoliceOfficerLog.Rank.RankName, OfficerCommandName = x.PoliceOfficerLog.Command.Name, OfficerIdentificationNumber = x.PoliceOfficerLog.IdentificationNumber, OfficerIPPISNumber = x.PoliceOfficerLog.IPPISNumber, OfficerName = x.PoliceOfficerLog.Name, OfficerAccountNumber = x.PoliceOfficerLog.AccountNumber, OfficerRankCode = x.PoliceOfficerLog.Rank.ExternalDataCode })
                .ToList(),
                NumberOfOfficersRequested = _transactionManager.GetSession().Query<EscortFormationAllocation>().Where(x => ((x.Group == new EscortSquadAllocationGroup { Id = escortAllocationGroupId }) && (x.Command == new Command { Id = adminCommandId })))
                .Select(x => x.NumberOfOfficers)
                .SingleOrDefault(),
                FormationAllocationId = formationAllocationId,
                AllocationGroupId = escortAllocationGroupId,
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
        /// Saves model records to EscortFormationOfficer
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public void SaveRecords(EscortPartialVM item, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors)
        {
            var session = _transactionManager.GetSession();
            int adminCommandId = session.Query<PSSAdminUsers>().Where(x => (x.User.Id == objUserInput.ApproverId)).Select(x => x.Command.Id).SingleOrDefault();
            long groupId = session.Query<EscortSquadAllocationGroup>().Where(x => (x.Request.Id == objUserInput.RequestId)).Select(x => x.Id).SingleOrDefault();
            long formationAllocationId = session.Query<EscortFormationAllocation>().Where(x => (x.Group.Id == groupId) && (x.Command.Id == adminCommandId) && (!x.IsDeleted)).Select(x => x.Id).SingleOrDefault();

            if (formationAllocationId == 0) { throw new UserNotAuthorizedForThisActionException { }; }

            var reference = string.Empty;
            string escortFormationOfficerSelectionTrackingTableName = "Parkway_CBS_Police_Core_" + typeof(EscortFormationOfficerSelectionTracking).Name;
            string escortFormationOfficerSelectionTrackingQueryText = string.Empty;

            //Number of ticks + PSS Admin Command Id + Formation Allocation Id + Implementation Class HashCode value - Reference Format
            reference = string.Format("ESC-{0}-CMD-{1}-DETS-{2}-IMPL-{3}", System.DateTime.Now.Ticks, adminCommandId, formationAllocationId, item.ImplementationClass.GetHashCode());
            foreach (var addition in objUserInput.OfficersSelection)
            {
                escortFormationOfficerSelectionTrackingQueryText = $"INSERT INTO {escortFormationOfficerSelectionTrackingTableName}(FormationAllocation_Id, Group_Id, PoliceOfficerLog_Id, IsDeleted, Reference, CreatedAtUtc, UpdatedAtUtc, EscortRankRate) VALUES({formationAllocationId}, {groupId}, {addition.PoliceOfficerLogId}, 0, '{reference}', GETDATE(), GETDATE(),{addition.EscortRankRate})";

                session.CreateSQLQuery(escortFormationOfficerSelectionTrackingQueryText).ExecuteUpdate();
            }

            foreach (var removal in objUserInput.RemovedOfficersSelection)
            {
                escortFormationOfficerSelectionTrackingQueryText = $"INSERT INTO {escortFormationOfficerSelectionTrackingTableName}(FormationAllocation_Id, Group_Id, PoliceOfficerLog_Id, IsDeleted, Reference, CreatedAtUtc, UpdatedAtUtc, EscortRankRate) VALUES({formationAllocationId}, {groupId}, {removal.PoliceOfficerLogId}, 1, '{reference}', GETDATE(), GETDATE(), {removal.EscortRankRate})";

                session.CreateSQLQuery(escortFormationOfficerSelectionTrackingQueryText).ExecuteUpdate();
            }

            //merge query comes in here, this updates the EscortFormationOfficer table with the EscortFormationOfficerSelectionTracking table
            string escortFormationOfficerSyncQuery = $"MERGE Parkway_CBS_Police_Core_EscortFormationOfficer AS Target USING Parkway_CBS_Police_Core_EscortFormationOfficerSelectionTracking AS Source ON Source.FormationAllocation_Id = Target.FormationAllocation_Id AND Source.PoliceOfficerLog_Id = Target.PoliceOfficerLog_Id WHEN MATCHED AND Source.Reference = '{reference}' THEN UPDATE SET Target.IsDeleted = Source.IsDeleted, Target.UpdatedAtUtc = GETDATE() WHEN NOT MATCHED BY Target AND Source.Reference = '{reference}' THEN INSERT(FormationAllocation_Id, Group_Id, PoliceOfficerLog_Id, IsDeleted, CreatedAtUtc, UpdatedAtUtc, EscortRankRate) VALUES(Source.FormationAllocation_Id, Source.Group_Id, Source.PoliceOfficerLog_Id, 0, GETDATE(), GETDATE(), Source.EscortRankRate);";

            session.CreateSQLQuery(escortFormationOfficerSyncQuery).ExecuteUpdate();
            SaveApprovalLog(objUserInput);
            return;
        }


        public RequestApprovalResponse OnSubmit(int approverId, long requestId, int commandTypeId)
        {
            //get the number of assignments
            var session = _transactionManager.GetSession();
            int adminCommandId = session.Query<PSSAdminUsers>().Where(x => (x.User.Id == approverId)).Select(x => x.Command.Id).SingleOrDefault();
            long groupId = session.Query<EscortSquadAllocationGroup>().Where(x => (x.Request.Id == requestId)).Select(x => x.Id).SingleOrDefault();
            var alloc = session.Query<EscortFormationAllocation>().Where(x => (x.Group.Id == groupId) && (x.Command.Id == adminCommandId) && (!x.IsDeleted));
            alloc.FirstOrDefault().NumberAssignedByCommander = alloc.FirstOrDefault().SquadronOfficers.Count(x => !x.IsDeleted);

            PSSRequest request = _transactionManager.GetSession().Query<PSSRequest>().Where(r => r.Id == requestId).FirstOrDefault();

            UpdateRequestCommandWorkflowLog(requestId, adminCommandId, request.FlowDefinitionLevel.Id, false);


            return new RequestApprovalResponse
            {
                ServiceType = request.Service.ServiceType.ToString(),
                FileNumber = request.FileRefNumber,
                CustomerName = request.CBSUser.Name,
                NotificationMessage = string.Format("{0} request with File Number {1} for {2} submitted successfully. {3}", (PSSServiceTypeDefinition)request.Service.ServiceType, request.FileRefNumber, request.CBSUser.Name, "This application has been submitted."),
                ResponseFromPartial = true,
            };
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

                string additionalComment = string.Empty;

                if (objUserInput.OfficersSelection.Any() && objUserInput.RemovedOfficersSelection.Any())
                {
                    //Added and removed officers
                    additionalComment = $"The request for officers has been assigned to {string.Join(", ", objUserInput.OfficersSelection.Select(x => $"{x.OfficerName} ({x.OfficerRankName})"))} and  {string.Join(", ", objUserInput.RemovedOfficersSelection.Select(x => $"{x.OfficerName} ({x.OfficerRankName})"))} have been unassigned from the request.";
                }
                else if (objUserInput.OfficersSelection.Any() && !objUserInput.RemovedOfficersSelection.Any())
                {
                    //Added officers
                    additionalComment = $"The request for officers has been assigned to {string.Join(", ", objUserInput.OfficersSelection.Select(x => $"{x.OfficerName} ({x.OfficerRankName})"))}.";
                }
                else if (!objUserInput.OfficersSelection.Any() && objUserInput.RemovedOfficersSelection.Any())
                {
                    //Removed officers
                    additionalComment = $"{string.Join(", ", objUserInput.RemovedOfficersSelection.Select(x => $"{x.OfficerName} ({x.OfficerRankName})"))} has been unassigned from the request.";
                }

                objUserInput.Comment = $"{adminUser.Fullname} (Squadron Leader {adminUser.Command.Name}): {objUserInput.Comment.TrimEnd('.')}. {additionalComment}";

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
            catch (System.Exception exception)
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


        /// <summary>
        /// Gets rate of officer with specified rank for the specified state and lga taking into consideration the selected sub tax category
        /// </summary>
        /// <param name="officerRankId"></param>
        /// <param name="pssEscortServiceCategoryId"></param>
        /// <param name="stateId"></param>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        private decimal GetRateSheetId(long officerRankId, int pssEscortServiceCategoryId, int stateId, int lgaId, int commandTypeId)
        {
            return _transactionManager.GetSession().Query<EscortAmountChartSheet>()
          .Where(er => (er.Rank == new PoliceRanking { Id = officerRankId }) && (er.PSSEscortServiceCategory == new PSSEscortServiceCategory { Id = pssEscortServiceCategoryId })
          && (er.State == new CBS.Core.Models.StateModel { Id = stateId }) && (er.LGA == new CBS.Core.Models.LGA { Id = lgaId }) && (er.CommandType == new CommandType { Id = commandTypeId }))
          .Select(er => er.Rate).FirstOrDefault();
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


        /// <summary>
        /// Routes the request to Squadron Leader level
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToThisEscortStage(EscortPartialVM item, EscortRequestDetailsVM objUserInput)
        {
            throw new UserNotAuthorizedForThisActionException();
        }
    }
}