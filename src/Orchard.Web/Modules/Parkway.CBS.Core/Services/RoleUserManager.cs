using Orchard;
using System;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Logging;
using Orchard.Users.Models;
using System.Linq.Expressions;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.Services
{
    public class RoleUserManager : BaseManager<AccessRoleUser>, IRoleUserManager<AccessRoleUser>
    {
        private readonly IRepository<AccessRoleUser> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public RoleUserManager(IRepository<AccessRoleUser> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Here we check if the user has been constrained to any role
        /// </summary>
        /// <param name="id"></param>
        /// <param name="invoiceAssessmentReport"></param>
        /// <returns>bool</returns>
        public bool UserHasAcessTypeRole(int adminUserId, AccessType accessType)
        {
            if(adminUserId == 0) { return false; }

            var result = _transactionManager.GetSession().Query<AccessRoleUser>().Where(r => (r.User == new UserPartRecord { Id = adminUserId }) && (r.AccessRole.IsActive) && (r.AccessRole.AccessType == (int)accessType)).Take(1).ToList().FirstOrDefault();

            if(result != null) { return true; }
            return false;
        }

    }
}