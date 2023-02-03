using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services
{
    public class PoliceRankingManager : BaseManager<PoliceRanking>, IPoliceRankingManager<PoliceRanking>
    {
        private readonly IRepository<PoliceRanking> _repository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }
        private readonly ITransactionManager _transactionManager;

        public PoliceRankingManager(IRepository<PoliceRanking> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _repository = repository;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _user = user;
            _transactionManager = orchardServices.TransactionManager;
        }

        /// <summary>
        /// Get list of police rank
        /// </summary>
        /// <returns></returns>
        public List<PoliceRankingVM> GetPoliceRanks()
        {
            return _transactionManager.GetSession().Query<PoliceRanking>().OrderByDescending(x=>x.Id)
               .Select(rk => new PoliceRankingVM { Id = rk.Id, RankName = rk.RankName, RankLevel = rk.RankLevel }).ToList();
        }


        /// <summary>
        /// Get police rank details using rank id
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns>PoliceRankingVM</returns>
        public PoliceRankingVM GetPoliceRank(long rankId)
        {
            return _transactionManager.GetSession().Query<PoliceRanking>().Where(x => x.Id == rankId)
               .Select(rk => new PoliceRankingVM { Id = rk.Id, RankName = rk.RankName }).FirstOrDefault();
        }


        /// <summary>
        /// Get police rank details using rank code
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns>PoliceRankingVM</returns>
        public PoliceRankingVM GetPoliceRank(string rankCode)
        {
            return _transactionManager.GetSession().Query<PoliceRanking>().Where(x => x.ExternalDataCode == rankCode)
               .Select(rk => new PoliceRankingVM { Id = rk.Id, RankName = rk.RankName, ExternalDataCode = rk.ExternalDataCode }).SingleOrDefault();
        }

    }
}