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
    public class PSSFeePartyAdapterConfigurationManager : BaseManager<PSSFeePartyAdapterConfiguration>, IPSSFeePartyAdapterConfigurationManager<PSSFeePartyAdapterConfiguration>
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public PSSFeePartyAdapterConfigurationManager(IRepository<PSSFeePartyAdapterConfiguration> repository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(repository, user, orchardServices)
        {
            _orchardServices = orchardServices;
            _transactionManager = _orchardServices.TransactionManager;
        }

        /// <summary>
        /// Gets all active adapter configurations
        /// </summary>
        /// <returns></returns>
        public List<FeePartyAdapterConfigurationVM> GetActiveSettlementAdapterConfigurations()
        {
            return _transactionManager.GetSession().Query<PSSFeePartyAdapterConfiguration>().Where(sac => sac.IsActive)
              .Select(sac => new FeePartyAdapterConfigurationVM { Id = sac.Id, Name = sac.Name }).ToList();
        }

        /// <summary>
        /// Get the Adapter by <paramref name="adapterId"/> 
        /// </summary>
        /// <param name="adapterId"></param>
        /// <returns></returns>
        public FeePartyAdapterConfigurationVM GetSettlementAdapterConfigurationsByAdapterId(int adapterId)
        {
            return _transactionManager.GetSession().Query<PSSFeePartyAdapterConfiguration>().Where(sac => sac.Id == adapterId)
              .Select(sac => new FeePartyAdapterConfigurationVM { Id = sac.Id, Name = sac.Name }).Single();
        }

        /// <summary>
        /// Checks if the adapter configuration exists using the <paramref name="adapterId"/>
        /// </summary>
        /// <param name="adapterId"></param>
        /// <returns></returns>
        public bool CheckIfSettlementAdapterConfigurationsExistByAdapterId(int adapterId)
        {
            return _transactionManager.GetSession().Query<PSSFeePartyAdapterConfiguration>().Count(sac => sac.Id == adapterId) > 0;
        }

    }

}