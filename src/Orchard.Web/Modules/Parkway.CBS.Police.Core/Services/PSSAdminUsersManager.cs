using NHibernate.Linq;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSAdminUsersManager : BaseManager<PSSAdminUsers>, IPSSAdminUsersManager<PSSAdminUsers>
    {
        private readonly IRepository<PSSAdminUsers> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSAdminUsersManager(IRepository<PSSAdminUsers> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }

        /// <summary>
        /// Check if a user with the specified email exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns>bool</returns>
        public bool CheckUser(string email)
        {
            return _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.Email == email).Count() > 0;
        }

        /// <summary>
        /// Gets command for logged in admin user with specified id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public CommandVM GetCommandForUser(int UserId)
        {
            return _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User == new UserPartRecord { Id = UserId }).Select(x => new CommandVM { Code = x.Command.Code }).SingleOrDefault();
        }

        /// <summary>
        /// Gets admin user id for user part record with specified id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetAdminUserId(int userId)
        {
            return _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.Id == userId).Select(x => x.Id).SingleOrDefault();
        }

        /// <summary>
        /// Gets AdminUserVM for user with specific <paramref name="UserId"/>
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public AdminUserVM GetAdminUser(int UserId)
        {
            return _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.Id == UserId).Select(x => new AdminUserVM { Id = x.Id, CommandCategoryId = x.CommandCategory.Id, UserPartRecordId = x.User.Id, Fullname = x.Fullname, PhoneNumber = x.PhoneNumber, CommandId = x.Command.Id, Email = x.Email, RoleTypeId = x.RoleType.Id, Username = x.User.UserName }).SingleOrDefault();
        }

        /// <summary>
        /// Get admin user details using <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AdminUserVM GetAdminUser(string username)
        {
            return _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.UserName == username).Select(x => new AdminUserVM { CommandCategoryId = x.CommandCategory.Id, UserPartRecordId = x.User.Id, Fullname = x.Fullname, PhoneNumber = x.PhoneNumber, CommandId = x.Command.Id, CommandCategoryName = x.CommandCategory.Name, CommandName = x.Command.Name, Email = x.Email, RoleTypeId = x.RoleType.Id, Username = x.User.UserName, Id = x.Id }).SingleOrDefault();
        }


        /// <summary>
        /// Updates certain columns in PSSAdminUsers
        /// </summary>
        /// <param name="adminUsers"></param>
        public void UpdateAdminUser(AdminUserVM adminUsers)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSAdminUsers).Name;
                string phoneNumber = nameof(PSSAdminUsers.PhoneNumber);
                string fullName = nameof(PSSAdminUsers.Fullname);
                string roleTypeId = nameof(PSSAdminUsers.RoleType) + "_Id";
                string userId = nameof(PSSAdminUsers.User) + "_Id";
                string commandId = nameof(PSSAdminUsers.Command) + "_Id";
                string commandCategoryId = nameof(PSSAdminUsers.CommandCategory) + "_Id";
                string updatedAtName = nameof(PSSAdminUsers.UpdatedAtUtc);
                string lastUpdatedByName = nameof(PSSAdminUsers.LastUpdatedBy) + "_Id";

                var queryText = $"UPDATE {tableName} SET {roleTypeId} = :roleTypeId, {updatedAtName} = :updatedAtUtc,{lastUpdatedByName} = :lastUpdated , {fullName} = :fullName , {phoneNumber} = :phoneNumber , {commandId} = :commandId, {commandCategoryId} = :commandCategoryId    WHERE {userId} = :userId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updatedAtUtc", DateTime.Now.ToLocalTime());
                query.SetParameter("fullName", adminUsers.Fullname);
                query.SetParameter("userId", adminUsers.UserPartRecordId);
                query.SetParameter("phoneNumber", adminUsers.PhoneNumber);
                query.SetParameter("commandCategoryId", adminUsers.CommandCategoryId);
                query.SetParameter("commandId", adminUsers.CommandId);
                query.SetParameter("roleTypeId", adminUsers.RoleTypeId);
                query.SetParameter("lastUpdated", adminUsers.LastUpdatedById);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for admin user with user id {0}, Exception message {1}", adminUsers.UserPartRecordId, exception.Message));
                throw;
            }
        }

        /// <summary>
        /// Updates the <see cref="PSSAdminUsers.LastUpdatedBy"/> using <paramref name="lastUpdatedById"/>
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <param name="lastUpdatedById"></param>
        public void UpdateLastUpdatedBy(int userPartRecordId, bool isActive, int lastUpdatedById)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(PSSAdminUsers).Name;
                string userPartRecordName = nameof(PSSAdminUsers.User) + "_Id";
                string lastUpdatedByName = nameof(PSSAdminUsers.LastUpdatedBy) + "_Id";
                string updatedAtName = nameof(PSSAdminUsers.UpdatedAtUtc);
                string isActiveName = nameof(PSSAdminUsers.IsActive);

                var queryText = $"UPDATE {tableName} SET {updatedAtName} = :updatedAtUtc, {isActiveName} = :isActive, {lastUpdatedByName} = :lastUpdated WHERE {userPartRecordName} = :userId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updatedAtUtc", DateTime.Now.ToLocalTime());
                query.SetParameter("userId", userPartRecordId);
                query.SetParameter("lastUpdated", lastUpdatedById);
                query.SetParameter("isActive", isActive);

                query.ExecuteUpdate();
            }
            catch (Exception exception)
            {
                RollBackAllTransactions();
                Logger.Error(exception, string.Format("Exception updating details for admin user with user id {0}, Exception message {1}", userPartRecordId, exception.Message));
                throw;
            }

        }

        /// <summary>
        /// Gets the Id and UserPartRecord using the <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public AdminUserVM GetAdminUserPartRecordId(string username)
        {
            return _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.UserName == username).Select(x => new AdminUserVM { UserPartRecordId = x.User.Id, Id = x.Id }).SingleOrDefault();
        }


        /// <summary>
        /// Gets admin user with specified user part record id
        /// </summary>
        /// <param name="userPartRecordId"></param>
        /// <returns></returns>
        public PSSAdminUsersVM GetAdminUserWithUserPartRecordId(int userPartRecordId)
        {
            return _transactionManager.GetSession().Query<PSSAdminUsers>().Where(x => x.User.Id == userPartRecordId && x.IsActive).Select(x => new PSSAdminUsersVM { Id = x.Id, Fullname = x.Fullname, Email = x.Email, PhoneNumber = x.PhoneNumber }).SingleOrDefault();
        }
    }
}