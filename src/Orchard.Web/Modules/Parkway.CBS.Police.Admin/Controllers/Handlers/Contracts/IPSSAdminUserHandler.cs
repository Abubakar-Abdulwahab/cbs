using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSAdminUserHandler : IDependency
    {
        /// <summary>
        /// Get edit user VM
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>AdminUserCreationVM</returns>
        AdminUserCreationVM GetEditUserVM(int adminUserId);

        /// <summary>
        /// Get create user VM
        /// </summary>
        /// <param name="commandCategoryId"></param>
        /// <returns>AdminUserCreationVM</returns>
        AdminUserCreationVM GetCreateUserVM(int commandCategoryId = 0);

        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canCreateAdminUser"></param>
        void CheckForPermission(Permission canCreateAdminUser);

        /// <summary>
        /// Create admin user
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>bool</returns>
        bool CreateAdminUser(ref List<ErrorModel> errors, AdminUserCreationVM userInput);

        /// <summary>
        /// Edit admin user
        /// </summary>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <returns>bool</returns>
        bool EditAdminUser(ref List<ErrorModel> errors, AdminUserCreationVM userInput);

        /// <summary>
        /// Gets police officer with specified service number from HR system
        /// </summary>
        /// <param name="serviceNumber"></param>
        /// <returns></returns>
        APIResponse GetPoliceOfficerDetails(string serviceNumber);

        /// <summary>
        /// Gets flow definition for service type with specified id
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        APIResponse GetFlowDefinitionForServiceType(int serviceTypeId);

        /// <summary>
        /// Gets approval flow definition levels for flow definition with specified id
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        APIResponse GetApprovalFlowDefinitionLevelsForDefinitionWithId(int definitionId);

        /// <summary>
        /// Populates the vm for post back
        /// </summary>
        /// <param name="userInput"></param>
        void PopulateAdminUserModelForPostback(AdminUserCreationVM userInput);
    }
}
