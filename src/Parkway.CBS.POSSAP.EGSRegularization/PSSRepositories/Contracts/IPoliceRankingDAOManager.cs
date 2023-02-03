using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPoliceRankingDAOManager : IRepository<PoliceRanking>
    {
        PoliceRankingVM GetPoliceRank(string code);
    }
}
