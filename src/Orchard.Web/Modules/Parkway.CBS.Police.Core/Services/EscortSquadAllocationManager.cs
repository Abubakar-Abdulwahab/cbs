using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services
{
    public class EscortSquadAllocationManager : BaseManager<EscortSquadAllocation>, IEscortSquadAllocationManager<EscortSquadAllocation>
    {
        private readonly IRepository<EscortSquadAllocation> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;


        public EscortSquadAllocationManager(IRepository<EscortSquadAllocation> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
            Logger = NullLogger.Instance;
            _user = user;
        }
    }
}