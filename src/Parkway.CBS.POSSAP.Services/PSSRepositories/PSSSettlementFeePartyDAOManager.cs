using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSSettlementFeePartyDAOManager : Repository<PSSSettlementFeeParty>, IPSSSettlementFeePartyDAOManager
    {
        public PSSSettlementFeePartyDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Get active settlement adapters
        /// </summary>
        /// <param name="settlementId"></param>
        /// <returns>List<string></returns>
        public List<string> GetActiveSettlementAdapters(int settlementId)
        {
            return _uow.Session.Query<PSSSettlementFeeParty>()
                .Where(x => x.IsActive && x.Settlement.Id == settlementId && x.HasAdditionalSplits)
                .Select(x => x.AdditionalSplitValue).ToList();
        }
    }
}
