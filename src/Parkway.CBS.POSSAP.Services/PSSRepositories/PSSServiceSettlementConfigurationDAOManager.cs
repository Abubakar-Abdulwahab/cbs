using Parkway.CBS.ClientRepository;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSServiceSettlementConfigurationDAOManager : Repository<PSSServiceSettlementConfiguration>, IPSSServiceSettlementConfigurationDAOManager
    {
        public PSSServiceSettlementConfigurationDAOManager(IUoW uow) : base(uow)
        { }

    }
}
