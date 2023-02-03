using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts;
using System.Linq;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories
{
    public class PoliceRankingDAOManager : Repository<PoliceRanking>, IPoliceRankingDAOManager
    {
        public PoliceRankingDAOManager(IUoW uow) : base(uow)
        {

        }

        public PoliceRankingVM GetPoliceRank(string rankCode)
        {
            return _uow.Session.Query<PoliceRanking>().Where(x => x.ExternalDataCode == rankCode)
               .Select(rk => new PoliceRankingVM { Id = rk.Id, RankName = rk.RankName, ExternalDataCode = rk.ExternalDataCode }).SingleOrDefault();
        }
    }
}
