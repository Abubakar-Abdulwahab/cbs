using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSAdminUsersManager<PSSAdminUsers> : IDependency, IBaseManager<PSSAdminUsers>
    {
        /// <summary>
        /// Check if a user with the specified email exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        bool CheckUser(string email);

        /// <summary>
        /// Gets command for logged in admin user with specified id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        CommandVM GetCommandForUser(int UserId);

        /// <summary>
        /// Gets admin user id for user part record with specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetAdminUserId(int userId);

        /// <summary>
        /// Gets AdminUserVM for user with specific <paramref name="UserId"/>
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        AdminUserVM GetAdminUser(int UserId);


        /// <summary>
        /// Get admin user details using <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        AdminUserVM GetAdminUser(string username);

        /// <summary>
        /// Gets the Id and UserPartRecord using the <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        AdminUserVM GetAdminUserPartRecordId(string username);

        /// <summary>
        /// Updates certain columns in PSSAdminUsers
        /// </summary>
        /// <param name="adminUsers"></param>
        void UpdateAdminUser(AdminUserVM adminUsers);


        /// <summary>
        /// Updates the <see cref="PSSAdminUsers.LastUpdatedBy"/> using <paramref name="lastUpdatedById"/>
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="lastUpdatedById"></param>
        /// <param name="isActive"></param>
        void UpdateLastUpdatedBy(int userPartRecordId, bool isActive, int lastUpdatedById);


        /// <summary>
        /// Gets admin user with specified user part record id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        PSSAdminUsersVM GetAdminUserWithUserPartRecordId(int userPartRecordId);
    }
}