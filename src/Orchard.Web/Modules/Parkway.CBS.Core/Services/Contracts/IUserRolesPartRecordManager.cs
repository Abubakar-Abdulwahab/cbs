using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IUserRolesPartRecordManager : IDependency
    {
        /// <summary>
        /// Updates the role id for the <paramref name="userRolesPartRecordId"/>
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userRolesPartRecordId"></param>
        void UpdateRoleTypeId(int roleId, int userRolesPartRecordId);

        /// <summary>
        /// Sets <see cref="UserPart.RegistrationStatus"/>  to the value in <paramref name="isActive"/>
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="isActive"></param>
        /// <param name="lastUpdatedById"></param>
        /// <exception cref="CBSUserNotFoundException"></exception>
        /// <returns>The user's username </returns>
        string ToggleIsUserRegistrationStatus(int userPartRecordId, bool isActive, int lastUpdatedById);
    }
}
