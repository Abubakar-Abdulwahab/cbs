using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Security;
using Orchard.Users.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Services.Contracts;
using System;

namespace Parkway.CBS.Core.Services
{
    public class UserRolesPartRecordManager : IUserRolesPartRecordManager
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ILogger Logger { get; set; }

        public UserRolesPartRecordManager(IOrchardServices orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Updates the role id for the <paramref name="userRolesPartRecordId"/>
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userRolesPartRecordId"></param>
        public void UpdateRoleTypeId(int roleId, int userRolesPartRecordId)
        {
            try
            {
                string tableName = "Orchard_Roles_UserRolesPartRecord";
                string roleTypeId = nameof(UserRolesPartRecord.Role) + "_id";
                string userRolesPartRecord = nameof(UserRolesPartRecord.Id);


                var queryText = $"UPDATE {tableName} SET {roleTypeId} = :roleTypeId WHERE {userRolesPartRecord} = :userRolesPartRecordId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("userRolesPartRecordId", userRolesPartRecordId);
                query.SetParameter("roleTypeId", roleId);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                _transactionManager.GetSession().Transaction.Rollback();
                Logger.Error(exception, string.Format("Exception updating details for User Roles Part Record with user id {0}, Exception message {1}", userRolesPartRecordId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Sets <see cref="UserPart.RegistrationStatus"/>  to the value in <paramref name="isActive"/>
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="isActive"></param>
        /// <param name="lastUpdatedById"></param>
        /// <exception cref="CBSUserNotFoundException"></exception>
        /// <returns>The user's username </returns>
        public string ToggleIsUserRegistrationStatus(int userPartRecordId, bool isActive, int lastUpdatedById)
        {
            Logger.Information($"Updating {nameof(UserPart.RegistrationStatus)} with value {isActive} by user with Id: {lastUpdatedById}");

            IUser user = _orchardServices.ContentManager.Get<IUser>(userPartRecordId) ?? throw new CBSUserNotFoundException();

            user.As<UserPart>().RegistrationStatus = isActive ? UserStatus.Approved : UserStatus.Pending;

            return user.UserName;
        }

    }
}