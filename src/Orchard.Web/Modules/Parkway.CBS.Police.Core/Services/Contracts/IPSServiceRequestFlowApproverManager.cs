using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSServiceRequestFlowApproverManager<PSServiceRequestFlowApprover> : IDependency, IBaseManager<PSServiceRequestFlowApprover>
    {
        /// <summary>
        /// Here we check if the user is part of admin that can approve request
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        bool UserHasApproverRole(int userId);

        /// <summary>
        /// Check if a user is a valid approver for a specified approval definition level
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>bool</returns>
        bool UserIsValidApproverForDefinitionLevel(int userId, int definitionLevelId);

        /// <summary>
        /// Check if a user is a valid approver for a specified approval definition level
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>bool</returns>
        bool UserIsValidApproverForDefinitionLevel(string phoneNumber, int definitionLevelId);

        /// <summary>
        /// Checks if the user is already assigned to a flow definition level
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        bool IsAlreadyAssignedToFlowDefinitionLevel(int userId, int flowDefinitionLevelId);

        /// <summary>
        /// Get list of Ids (<see cref="PSServiceRequestFlowDefinitionLevel.Id"/>) by <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see cref="List{int}"/></returns>
        List<int> GetFlowDefintionLevelIdByUserId(int userId);

        /// <summary>
        /// Get list of Ids (<see cref="PSServiceRequestFlowDefinition.Id"/>) by <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns><see cref="List{int}"/></returns>
        List<int> GetDistinctDefinitionIdByUserId(int userId);

        /// <summary>
        /// Get the request approvers email and phone numbers
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="definitionLevelId"></param>
        /// <returns>IEnumerable<NotificationInfoVM></returns>
        List<NotificationInfoVM> GetRequestApproversInfo(long requestId, int definitionLevelId);

        /// <summary>
        /// Get command id for approver of flow definition level with the specified id
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns></returns>
        int GetCommandIdForApproverOfDefinitionLevel(int flowDefinitionLevelId);

        /// <summary>
        /// Gets service request flow approver DTO containing the PSS Admin User along with the CommandVM for that admin user and the flow definition level with the specified id
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <returns>PSServiceRequestFlowApproverDTO</returns>
        PSServiceRequestFlowApproverDTO GetServiceRequestFlowApproverForDefinitionLevelWithId(int flowDefinitionLevelId);

        /// <summary>
        /// Check if a user is a valid approver
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns>bool</returns>
        bool UserIsValidApprover(string phoneNumber);

        /// <summary>
        /// Get command id for approver of flow definition level with the specified id and phone number
        /// </summary>
        /// <param name="flowDefinitionLevelId"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>int</returns>
        int GetCommandIdForApproverOfDefinitionLevel(int flowDefinitionLevelId, string phoneNumber);
    }
}
