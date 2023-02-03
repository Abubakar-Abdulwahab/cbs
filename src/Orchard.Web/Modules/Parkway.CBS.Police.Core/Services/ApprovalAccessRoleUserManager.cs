using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class ApprovalAccessRoleUserManager : BaseManager<ApprovalAccessRoleUser>, IApprovalAccessRoleUserManager<ApprovalAccessRoleUser>
    {

        private readonly IRepository<ApprovalAccessRoleUser> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public ApprovalAccessRoleUserManager(IRepository<ApprovalAccessRoleUser> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UserHasAcessTypeRole(int userId)
        {
            if (userId == 0) { return false; }

            var result = _transactionManager.GetSession().Query<ApprovalAccessRoleUser>().Where(r => (r.User == new UserPartRecord { Id = userId }) && (r.IsActive)).Select(v => v.Id).Take(1).ToList().FirstOrDefault();

            if (result != 0) { return true; }
            return false;
        }

        /// <summary>
        /// Get Approval Access Role User Id using UserPart Record Id <paramref name="userId"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IEnumerable<ApprovalAccessRoleUserDTO> GetApprovalAccessRoleUserId(int userId)
        {
            return _transactionManager.GetSession().Query<ApprovalAccessRoleUser>().Where(r => r.User == new UserPartRecord { Id = userId }).Select(x => new ApprovalAccessRoleUserDTO { Id = x.Id, AccessType = x.AccessType }); 
        }

        /// <summary>
        /// Get Access Role User Id using UserPart Record Id <paramref name="userId"/> and Admin User Type Id <paramref name="accessType"/>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accessType"></param>
        /// <returns>int</returns>
        public int GetAccessRoleUserId(int userId, AdminUserType accessType = AdminUserType.Viewer)
        {
            return _transactionManager.GetSession().Query<ApprovalAccessRoleUser>().Where(r => r.User == new UserPartRecord { Id = userId } && r.AccessType == (int)accessType).Select(x => x.Id).FirstOrDefault();
        }
    }
}