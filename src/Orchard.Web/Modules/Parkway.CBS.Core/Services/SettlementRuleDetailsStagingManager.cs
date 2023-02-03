using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class SettlementRuleDetailsStagingManager : BaseManager<SettlementRuleDetailsStaging>, ISettlementRuleDetailsStagingManager<SettlementRuleDetailsStaging>
    {

        private readonly IRepository<SettlementRuleDetailsStaging> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public SettlementRuleDetailsStagingManager(IRepository<SettlementRuleDetailsStaging> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }
    }
}