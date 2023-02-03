using NHibernate.Linq;
using System.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System;
using Orchard.Logging;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services
{
    public class ActivityPermissionManager : BaseManager<ActivityPermission>, IActivityPermissionManager<ActivityPermission>
    {
        private readonly IRepository<ActivityPermission> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ActivityPermissionManager(IRepository<ActivityPermission> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Gets the permission preference
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="permissionName"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>Dictionary{int, bool}</returns>
        public Dictionary<int, bool> GetPermissionPreference(CBSPermissionName permissionName, int revenueHeadId, int mdaId)
        {
            try
            {
                return _transactionManager.GetSession().Query<ActivityPermission>()
                                                .Where(a =>
                                                ((a.CBSPermission.Name == permissionName.GetEnumName()) && (a.CBSPermission.IsActive))
                                                &&
                                                ((((a.ActivityId == revenueHeadId) && (a.ActivityType == (int)ActivityType.RevenueHead))) ||
                                                 ((a.ActivityId == mdaId) && (a.ActivityType == (int)ActivityType.MDA)))
                                                    && (!a.IsDeleted))
                                                .ToDictionary(a => a.ActivityType, a => a.Value);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
            
        }
    }
}