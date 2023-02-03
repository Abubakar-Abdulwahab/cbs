using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.ClientRepository.Repositories
{
    public class SettlementEngineDetailsDAOManager : Repository<SettlementEngineDetails>, ISettlementEngineDetailsDAOManager
    {
        public SettlementEngineDetailsDAOManager(IUoW uow) : base(uow)
        { }

    }
}
