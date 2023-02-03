using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IApprovalAccessRoleUserManager<ApprovalAccessRoleUser> : IDependency, IBaseManager<ApprovalAccessRoleUser>
    {
        /// <summary>
        /// Get Approval Access Role User Id using UserPart Record Id <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IEnumerable<ApprovalAccessRoleUserDTO> GetApprovalAccessRoleUserId(int userId);

        /// <summary>
        /// Here we check if the user has been constrained to any command
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>bool</returns>
        bool UserHasAcessTypeRole(int userId);

        /// <summary>
        /// Get Access Role User Id using UserPart Record Id <paramref name="userId"/> and Admin User Type Id <paramref name="accessType"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accessType"></param>
        /// <returns>int</returns>
        int GetAccessRoleUserId(int userId, AdminUserType accessType = AdminUserType.Viewer);

    }
}
