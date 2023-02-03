using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class MDARevenueAccessRestrictionsStagingManager : BaseManager<MDARevenueAccessRestrictionsStaging>, IMDARevenueAccessRestrictionsStagingManager<MDARevenueAccessRestrictionsStaging>
    {
        private readonly IRepository<MDARevenueAccessRestrictionsStaging> _accessRestrictionsStagingRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public MDARevenueAccessRestrictionsStagingManager(IRepository<MDARevenueAccessRestrictionsStaging> accessRestrictionsStagingRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(accessRestrictionsStagingRepository, user, orchardServices)
        {
            _accessRestrictionsStagingRepository = accessRestrictionsStagingRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }
    }
}