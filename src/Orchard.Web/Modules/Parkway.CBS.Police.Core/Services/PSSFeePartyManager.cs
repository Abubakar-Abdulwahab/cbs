using NHibernate.Linq;
using Orchard;
using Orchard.Data;
using Orchard.Users.Models;
using Parkway.CBS.Core.Services;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Police.Core.Services
{
    public class PSSFeePartyManager : BaseManager<PSSFeeParty>, IPSSFeePartyManager<PSSFeeParty>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;
        public PSSFeePartyManager(IRepository<PSSFeeParty> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
        }

        /// <summary>
        /// Gets all active parties
        /// </summary>
        /// <returns></returns>
        public List<PSSFeePartyVM> GetAllActiveFeeParties()
        {
            return _transactionManager.GetSession().Query<PSSFeeParty>().Where(fp => fp.IsActive)
               .Select(cm => new PSSFeePartyVM { Id = cm.Id, Name = cm.Name }).ToList();
        }


        /// <summary>
        /// Checks if the fee party exists using the <paramref name="feePartyId"/>
        /// </summary>
        /// <param name="feePartyId"></param>
        /// <returns></returns>
        public bool CheckIfFeePartyExistById(int feePartyId)
        {
            return _transactionManager.GetSession().Query<PSSFeeParty>().Count(sac => sac.Id == feePartyId) > 0;
        }
    }
}