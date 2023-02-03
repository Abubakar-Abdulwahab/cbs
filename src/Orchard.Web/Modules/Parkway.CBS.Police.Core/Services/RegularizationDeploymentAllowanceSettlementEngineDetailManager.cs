using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services
{
    public class RegularizationDeploymentAllowanceSettlementEngineDetailManager : BaseManager<RegularizationDeploymentAllowanceSettlementEngineDetail>, IRegularizationDeploymentAllowanceSettlementEngineDetailManager<RegularizationDeploymentAllowanceSettlementEngineDetail>
    {

        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public RegularizationDeploymentAllowanceSettlementEngineDetailManager(IRepository<RegularizationDeploymentAllowanceSettlementEngineDetail> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
        }
    }
}