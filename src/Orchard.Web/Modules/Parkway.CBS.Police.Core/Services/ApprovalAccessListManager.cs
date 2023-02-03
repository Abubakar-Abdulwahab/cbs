using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class ApprovalAccessListManager : BaseManager<ApprovalAccessList>, IApprovalAccessListManager<ApprovalAccessList>
    {
        private readonly IRepository<ApprovalAccessList> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ApprovalAccessListManager(IRepository<ApprovalAccessList> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Checks if the user already has approval access
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool CheckIfCommandExistForUser(int commandId, int userId)
        {
            return _transactionManager.GetSession().Query<ApprovalAccessList>().Count(x => x.Command.Id == commandId && x.ApprovalAccessRoleUser.Id == userId) > 0;
        }

        /// <summary>
        /// Returns a list of CommandId and service type for ApprovalAccessRoleUser using <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<ApprovalAccessListVM> GetApprovalAccessListByUserId(int userId)
        {
            return _transactionManager.GetSession().Query<ApprovalAccessList>().Where(x => x.ApprovalAccessRoleUser.User.Id == userId && !x.IsDeleted).Select(x => new ApprovalAccessListVM  { CommandId = x.Command.Id, ServiceId = x.Service.Id, AccessTypeId = x.ApprovalAccessRoleUser.AccessType }).ToList();
        }

        /// <summary>
        /// Returns all the services the user with the specified id has access to
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<int> GetSelectedServicesFromAccessListByuserId(int userId)
        {
            return _transactionManager.GetSession().Query<ApprovalAccessList>().Where(x => x.ApprovalAccessRoleUser.User.Id == userId && !x.IsDeleted).Select(x => x.Service.Id ).Distinct().ToList();
        }


        /// <summary>
        /// Deletes the specified service for specified access role user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="serviceId"></param>
        public void DeleteServiceInAccessList(int userId, int serviceId)
        {
            try
            {
                string tableName = "Parkway_CBS_Police_Core_" + typeof(ApprovalAccessList).Name;
                string tableNameTwo = "Parkway_CBS_Police_Core_" + typeof(ApprovalAccessRoleUser).Name;

                var queryText = $"UPDATE T1 SET T1.{nameof(ApprovalAccessList.IsDeleted)} = :isDeleted, T1.{nameof(ApprovalAccessList.UpdatedAtUtc)} = :updateDate FROM {tableName} T1 INNER JOIN {tableNameTwo} AS T2 ON T1.{nameof(ApprovalAccessList.ApprovalAccessRoleUser)}_Id = T2.{nameof(ApprovalAccessRoleUser.Id)}  WHERE T2.{nameof(ApprovalAccessRoleUser.User)}_Id = :userId AND T1.{nameof(ApprovalAccessList.Service)}_Id = :serviceId";
                var query = _transactionManager.GetSession().CreateSQLQuery(queryText);
                query.SetParameter("updateDate", DateTime.Now.ToLocalTime());
                query.SetParameter("userId", userId);
                query.SetParameter("serviceId", serviceId);
                query.SetParameter("isDeleted", true);

                query.ExecuteUpdate();
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }
    }
}