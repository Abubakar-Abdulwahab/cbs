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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.EscortApprovalViewComposition
{
    public class SecretariatRoutingEscortViewComposition : IEscortViewComposition
    {
        public int StageIdentifier => 2;
        private ITransactionManager _transactionManager;
        private IEscortViewComposition _inherentImplementation;
        private string _inherentImplementationPartialName;
        private PSSAdminUsersVM _inherentAdminUser;
        public ILogger Logger { get; set; }

        public SecretariatRoutingEscortViewComposition()
        {
            Logger = NullLogger.Instance;
        }


        public dynamic SetPartialData(EscortPartialVM partialComp)
        {
            try
            {
                return new SecretariatEscortRoutingVM { InherentImplementationModel = GetInstanceForInherentImplementation(partialComp.RequestId, partialComp.UserId).SetPartialData(partialComp), InherentImplementationPartialName = _inherentImplementationPartialName };
            }
            catch(NoRecordFoundException)
            {
                return new SecretariatEscortRoutingVM { };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Set the transaction manager to aid database queries
        /// </summary>
        /// <param name="transactionManager"></param>
        public void SetTransactionManagerForDBQueries(ITransactionManager transactionManager) { _transactionManager = transactionManager; }


        /// <summary>
        /// Do validation on partial model
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public bool DoValidation(EscortPartialVM item, EscortRequestDetailsVM objUserInput, ref List<ErrorModel> errors)
        {
            return GetInstanceForInherentImplementation(item.RequestId, item.UserId).DoValidation(item, objUserInput, ref errors);
        }


        /// <summary>
        /// Saves model records
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <param name="errors"></param>
        public void SaveRecords(EscortPartialVM item, EscortRequestDetailsVM objUserInput, EscortDetailsDTO escortDetails, ref List<ErrorModel> errors)
        {
            GetInstanceForInherentImplementation(item.RequestId, item.UserId).SaveRecords(item, objUserInput, escortDetails, ref errors);
        }


        /// <summary>
        /// Here we done the level shifting and approval assignments
        /// </summary>
        public RequestApprovalResponse OnSubmit(int adminUserId, Int64 requestId, int commandTypeId)
        {
            try
            {
                return GetInstanceForInherentImplementation(requestId, adminUserId).OnSubmit(_inherentAdminUser.UserId, requestId, commandTypeId);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        public EscortApprovalMessage Approval(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int adminUserId)
        {
            throw new UserNotAuthorizedForThisActionException();
        }


        public CanApproveEscortVM CanApprove(List<EscortPartialVM> partials, long requestId, EscortRequestDetailsVM escort, int adminUserId)
        {
            throw new UserNotAuthorizedForThisActionException();
        }


        /// <summary>
        /// Routes the request to this level
        /// </summary>
        /// <param name="item"></param>
        /// <param name="objUserInput"></param>
        /// <returns>SecretariatRoutingApprovalResponse</returns>
        public SecretariatRoutingApprovalResponse RouteToThisEscortStage(EscortPartialVM item, EscortRequestDetailsVM objUserInput) 
        {
            //get the current admin user
            int adminUserId = _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.Id == objUserInput.ApproverId).Select(x => x.Id).SingleOrDefault();
            //get the command type for this request
            int commandTypeId = _transactionManager.GetSession().Query<PSSEscortDetails>().Where(x => x.Request.Id == objUserInput.RequestId).Select(x => x.CommandType.Id).SingleOrDefault();
            //get the role of the admin user for the process flow attached to the selected request stage
            int escortStageRole = _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => (x.Level.Id == objUserInput.SelectedRequestStage) && (x.CommandType.Id == commandTypeId) && (x.IsActive)).Select(x => x.AdminUser.RoleType.Id).FirstOrDefault();
            if (escortStageRole == 0) { Logger.Error("No role found for escort process stage definition"); throw new NoRecordFoundException(); }
            //get the partial implementation of the admin user
            IEnumerable<EscortPartialVM> partials = _transactionManager.GetSession().Query<EscortRolePartial>().Where(x => x.Role.Id == escortStageRole).Select(x => new EscortPartialVM { PartialName = x.PartialName, ImplementationClass = x.ImplementationClass });
            //perform logic for routing the request to the escort stage
            return PerformEscortSpecificRouteAction(partials.ToList(), objUserInput, adminUserId);
        }


        /// <summary>
        /// Gets the implementation of the parent escort approver for the stage the request was routed at
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private IEscortViewComposition GetInstanceForInherentImplementation(long requestId, int userId)
        {
            if(_inherentImplementation == null)
            {
                //get the request stage that the secretariat routing took place
                int stageRoutedAt = _transactionManager.GetSession().Query<SecretariatRoutingLevel>().Where(x => (x.Request.Id == requestId) && (x.AdminUser.User.Id == userId) && (x.StageModelName == typeof(EscortProcessStageDefinition).Name)).Select(x => x.StageRoutedTo).SingleOrDefault();
                if (stageRoutedAt == 0) { Logger.Error("Request has not been routed to any stage"); throw new NoRecordFoundException(); }

                EscortProcessStageDefinition stage = _transactionManager.GetSession().Query<EscortProcessStageDefinition>().Where(x => (x.IsActive) && (x.Id == stageRoutedAt)).FirstOrDefault();
                if (stage == null) { Logger.Error($"No escort process stage definition found for selected stage {stageRoutedAt}"); throw new NoRecordFoundException(); }
                if (stage.ParentDefinition == null) { Logger.Error($"No parent escort process stage definition for stage with id of {stageRoutedAt}"); throw new NoRecordFoundException(); }
                int parentStageId = stage.ParentDefinition.Id;

                _inherentAdminUser = _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => (x.Level.Id == parentStageId) && (x.IsActive)).Select(x => new PSSAdminUsersVM { RoleTypeId = x.AdminUser.RoleType.Id, Id = x.AdminUser.Id, UserId = x.AdminUser.User.Id }).FirstOrDefault();

                EscortPartialVM inherentRolePartial = _transactionManager.GetSession().Query<EscortRolePartial>().Where(x => (x.IsActive) && (x.Role.Id == _inherentAdminUser.RoleTypeId)).Select(x => new EscortPartialVM { PartialName = x.PartialName, ImplementationClass = x.ImplementationClass }).SingleOrDefault();
                _inherentImplementationPartialName = inherentRolePartial.PartialName;
                _inherentImplementation = ((IEscortViewComposition)Activator.CreateInstance(inherentRolePartial.ImplementationClass.Split(',')[0], inherentRolePartial.ImplementationClass.Split(',')[1]).Unwrap());
                if (_inherentImplementation is AIGEscortViewComposition || _inherentImplementation is SquadronLeaderEscortViewComposition || _inherentImplementation is DCPEscortViewComposition || _inherentImplementation is DPOEscortViewComposition) { Logger.Error("No implementation for the given escort view composition"); throw new Exception(); }
                _inherentImplementation.SetTransactionManagerForDBQueries(_transactionManager);
            }
            return _inherentImplementation;
        }


        /// <summary>
        /// Creates an instance of the selected escort stage view composition and calls it's implementation to route to the stage
        /// </summary>
        /// <param name="partials"></param>
        /// <param name="requestDetailsVM"></param>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
        private SecretariatRoutingApprovalResponse PerformEscortSpecificRouteAction(List<EscortPartialVM> partials, EscortRequestDetailsVM requestDetailsVM, int adminUserId)
        {
            IEscortViewComposition partialCompImpl = ((IEscortViewComposition)Activator.CreateInstance(partials[0].ImplementationClass.Split(',')[0], partials[0].ImplementationClass.Split(',')[1]).Unwrap());
            partialCompImpl.SetTransactionManagerForDBQueries(_transactionManager);
            SecretariatRoutingApprovalResponse response = partialCompImpl.RouteToThisEscortStage(partials[0], requestDetailsVM);
            //create entry in secretariat routing level to hold the stage the request has been routed
            string secretariatRoutingLevelTableName = "Parkway_CBS_Police_Core_" + typeof(SecretariatRoutingLevel).Name;
            string secretariatRoutingLevelQueryText = $"INSERT INTO {secretariatRoutingLevelTableName}(Request_Id, StageRoutedTo, StageModelName, AdminUser_Id, CreatedAtUtc, UpdatedAtUtc) VALUES({requestDetailsVM.RequestId}, {requestDetailsVM.SelectedRequestStage}, '{typeof(EscortProcessStageDefinition).Name}', {adminUserId}, GETDATE(), GETDATE())";
            _transactionManager.GetSession().CreateSQLQuery(secretariatRoutingLevelQueryText).ExecuteUpdate();
            SaveApprovalLog(requestDetailsVM);
            return response;
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

                string commandName = _transactionManager.GetSession().Query<EscortProcessFlow>().Where(x => x.Level.Id == objUserInput.SelectedRequestStage).Select(x => x.AdminUser.Command.Name).FirstOrDefault();

                string additionalComment = $"The request has been routed to {commandName}.";

                objUserInput.Comment = $"{adminName} (POSSAP Secretariat): {objUserInput.Comment.TrimEnd('.')}. {additionalComment}";

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
    }
}