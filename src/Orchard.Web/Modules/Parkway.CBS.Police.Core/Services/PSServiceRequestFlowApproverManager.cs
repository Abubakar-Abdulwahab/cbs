using NHibernate.Linq;
using NHibernate.Transform;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSServiceRequestFlowApproverManager : BaseManager<PSServiceRequestFlowApprover>, IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover>
    {
        private readonly IRepository<PSServiceRequestFlowApprover> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }

        public PSServiceRequestFlowApproverManager(IRepository<PSServiceRequestFlowApprover> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Here we check if the user is part of admin that can approve request
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        public bool UserHasApproverRole(int userId)
        {
            if (userId == 0) { return false; }

            return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(r => r.AssignedApprover == new UserPartRecord { Id = userId } && !r.IsDeleted).Select(v => v.Id).ToList().Count > 0;
        }


        /// <summary>
        /// Checks if the user is already assigned to a flow definition level
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        public bool IsAlreadyAssignedToFlowDefinitionLevel(int userId, int flowDefinitionLevelId)
        {
            return (_transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Count(r => (r.AssignedApprover == new UserPartRecord { Id = userId }) && (!r.IsDeleted) && (r.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = flowDefinitionLevelId})) > 0);
        }


        /// <summary>
        /// Get list of Ids (<see cref="PSServiceRequestFlowDefinition.Id"/>) by <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see cref="List{int}"/></returns>
        public List<int> GetDistinctDefinitionIdByUserId(int userId)
        {
            if (userId == 0) { return new List<int>(); }

            return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(r => r.AssignedApprover == new UserPartRecord { Id = userId } && !r.IsDeleted).GroupBy(x => x.FlowDefinitionLevel.Definition.Id ).Select(v => v.Key).ToList();
        }

        /// <summary>
        /// Get list of Ids (<see cref="PSServiceRequestFlowDefinitionLevel.Id"/>) by <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see cref="List{int}"/></returns>
        public List<int> GetFlowDefintionLevelIdByUserId(int userId)
        {
            if (userId == 0) { return new List<int>(); }

            return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(r => r.AssignedApprover == new UserPartRecord { Id = userId } && !r.IsDeleted).Select(v => v.FlowDefinitionLevel.Id).ToList();
        }

        /// <summary>
        /// Check if a user is a valid approver for a specified approval definition level
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>bool</returns>
        public bool UserIsValidApproverForDefinitionLevel(int userId, int definitionLevelId)
        {
            if (userId == 0) { return false; }
            return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(r => r.AssignedApprover == new UserPartRecord { Id = userId } && r.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = definitionLevelId } && !r.IsDeleted).Count() > 0;
        }

        /// <summary>
        /// Check if a user is a valid approver for a specified approval definition level
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>bool</returns>
        public bool UserIsValidApproverForDefinitionLevel(string phoneNumber, int definitionLevelId)
        {
            return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(r => r.PSSAdminUser.PhoneNumber == phoneNumber && r.FlowDefinitionLevel == new PSServiceRequestFlowDefinitionLevel { Id = definitionLevelId } && !r.IsDeleted).Count() > 0;
        }


        /// <summary>
        /// Get the request approvers email and phone numbers
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>IEnumerable<NotificationInfoVM></returns>
        public List<NotificationInfoVM> GetRequestApproversInfo(long requestId, int definitionLevelId)
        {
            try
            {
                var queryString = $"Select PAU.PhoneNumber, PAU.Email, PAU.Fullname as Name FROM Parkway_CBS_Police_Core_PSServiceRequestFlowApprover SRFA " +
                    $"INNER JOIN Parkway_CBS_Police_Core_PSSAdminUsers PAU ON PAU.Id = SRFA.PSSAdminUser_Id " +
                    $"INNER JOIN Parkway_CBS_Police_Core_ApprovalAccessRoleUser AARU ON AARU.User_Id = SRFA.AssignedApprover_Id " +
                    $"INNER JOIN Parkway_CBS_Police_Core_ApprovalAccessList AAL ON AAL.ApprovalAccessRoleUser_Id = AARU.Id WHERE exists (SELECT* FROM Parkway_CBS_Police_Core_RequestCommand RC " +
                    $"WHERE RC.Command_Id = AAL.Command_Id AND RC.Request_Id = {requestId}) AND SRFA.FlowDefinitionLevel_Id = {definitionLevelId}";

                return _transactionManager.GetSession().CreateSQLQuery(queryString).SetResultTransformer(Transformers.AliasToBean<NotificationInfoVM>()).List<NotificationInfoVM>().ToList();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, $"Exception getting the approvers details for request Id {requestId} and flowdefinition Id {definitionLevelId}");
                throw;
            }
        }


        /// <summary>
        /// Get command id for approver of flow definition level with the specified id
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        public int GetCommandIdForApproverOfDefinitionLevel(int flowDefinitionLevelId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(x => x.FlowDefinitionLevel.Id == flowDefinitionLevelId).Select(x => x.PSSAdminUser.Command.Id).FirstOrDefault();
            }catch(System.Exception exception)
            {
                Logger.Error(exception, $"Exception getting the approver command id for flow definition level with id {flowDefinitionLevelId}");
                throw;
            }
        }


        /// <summary>
        /// Gets service request flow approver DTO containing the PSS Admin User along with the CommandVM for that admin user and the flow definition level with the specified id
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        public PSServiceRequestFlowApproverDTO GetServiceRequestFlowApproverForDefinitionLevelWithId(int flowDefinitionLevelId)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(x => (x.FlowDefinitionLevel.Id == flowDefinitionLevelId) && (!x.IsDeleted)).Select(x => new PSServiceRequestFlowApproverDTO { FlowDefinitionLevel = new PSServiceRequestFlowDefinitionLevelDTO { Id = x.FlowDefinitionLevel.Id }, PSSAdminUser = new PSSAdminUsersVM { Id = x.PSSAdminUser.Id, Command = new CommandVM { Id = x.PSSAdminUser.Command.Id, Name = x.PSSAdminUser.Command.Name } } }).FirstOrDefault();
            }
            catch(System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Check if a user is a valid approver
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>bool</returns>
        public bool UserIsValidApprover(string phoneNumber)
        {
            return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(r => r.PSSAdminUser.PhoneNumber == phoneNumber && !r.IsDeleted).Count() > 0;
        }

        /// <summary>
        /// Get command id for approver of flow definition level with the specified id and phone number
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public int GetCommandIdForApproverOfDefinitionLevel(int flowDefinitionLevelId, string phoneNumber)
        {
            try
            {
                return _transactionManager.GetSession().Query<PSServiceRequestFlowApprover>().Where(x => x.FlowDefinitionLevel.Id == flowDefinitionLevelId && x.PSSAdminUser.PhoneNumber == phoneNumber).Select(x => x.PSSAdminUser.Command.Id).FirstOrDefault();
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, $"Exception getting the approver command id for flow definition level with id {flowDefinitionLevelId} and phone number {phoneNumber}");
                throw;
            }
        }


    }
}