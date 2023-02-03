using Orchard;
using System.Linq;
using Orchard.Data;
using NHibernate.Linq;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Core.Services
{
    public class MDARevenueHeadEntryStagingManager : BaseManager<MDARevenueHeadEntryStaging>, IMDARevenueHeadEntryStagingManager<MDARevenueHeadEntryStaging>
    {
        private readonly IRepository<MDARevenueHeadEntryStaging> _mdaRevenueHeadEntryStagingRepository;
        private readonly IRepository<UserPartRecord> _user;
        private readonly IOrchardServices _orchardServices;
        private readonly ITransactionManager _transactionManager;

        public MDARevenueHeadEntryStagingManager(IRepository<MDARevenueHeadEntryStaging> mdaRevenueHeadEntryStagingRepository, IRepository<UserPartRecord> user, IOrchardServices orchardServices) : base(mdaRevenueHeadEntryStagingRepository, user, orchardServices)
        {
            _mdaRevenueHeadEntryStagingRepository = mdaRevenueHeadEntryStagingRepository;
            _orchardServices = orchardServices;
            _transactionManager = orchardServices.TransactionManager;
        }


        /// <summary>
        /// Get the Id of the reference
        /// </summary>
        /// <param name="mdaRevenueHeadEntryStagingReference"></param>
        /// <param name="implementingClassHashCode"></param>
        /// <param name="opsIdentifier">Operation identifier</param>
        /// <returns>long</returns>
        public long GetReferenceId(string mdaRevenueHeadEntryStagingReference, int implementingClassHashCode, int opsIdentifier)
        {
            return _orchardServices.TransactionManager.GetSession().Query<MDARevenueHeadEntryStaging>()
                .Where(m => ((m.OperationTypeIdentifierId == opsIdentifier) && (m.ReferenceNumber == mdaRevenueHeadEntryStagingReference) && (m.OperationType == implementingClassHashCode))).Select(m => m.Id).First();
        }

    }
}