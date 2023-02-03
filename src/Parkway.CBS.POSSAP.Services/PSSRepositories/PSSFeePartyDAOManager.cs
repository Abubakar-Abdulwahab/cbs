using NHibernate.Linq;
using Parkway.CBS.ClientRepository;
using Parkway.CBS.ClientRepository.Repositories;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories
{
    public class PSSFeePartyDAOManager : Repository<PSSFeeParty>, IPSSFeePartyDAOManager
    {
        public PSSFeePartyDAOManager(IUoW uow) : base(uow)
        { }


        /// <summary>
        /// Get fee parties
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IEnumerable<PSSFeePartyVM> GetFeeParties(int skip, int take)
        {
            return _uow.Session.Query<PSSFeeParty>()
                .Where(x => x.IsActive)
                .Skip(skip)
                .Take(take)
                .Select(x => new PSSFeePartyVM { Id = x.Id, AccountNumber = x.AccountNumber, Name = x.Name });
        }
    }
}
