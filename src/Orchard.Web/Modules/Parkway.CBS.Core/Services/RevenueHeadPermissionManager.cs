using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.Services
{
    public class RevenueHeadPermissionManager : BaseManager<RevenueHeadPermission>, IRevenueHeadPermissionManager<RevenueHeadPermission>
    {
        private readonly IRepository<RevenueHeadPermission> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public RevenueHeadPermissionManager(IRepository<RevenueHeadPermission> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;

        }

        /// <summary>
        /// Get revenue head permissions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RevenueHeadPermissionVM> GetRevenueHeadPermissions()
        {
            return _transactionManager.GetSession().Query<RevenueHeadPermission>().Select(x => new RevenueHeadPermissionVM {
                Id = x.Id,
                Description = x.Description
            }).ToFuture();

        }
    }
}