using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSSettlementManager<PSSSettlement> : IDependency, IBaseManager<PSSSettlement>
    {
        /// <summary>
        /// Gets all active settlements
        /// </summary>
        /// <returns></returns>
        IEnumerable<PSSSettlementVM> GetActiveSettlements();

        /// <summary>
        /// Gets settlement using Id
        /// </summary>
        /// <param name="settlementId"></param>
        /// <returns></returns>
        PSSSettlementVM GetSettlementById(int settlementId);

        /// <summary>
        /// Updates updated at time for settlement with specified id
        /// </summary>
        /// <param name="settlementId"></param>
        void UpdateSettlementBatchUpdatedAtDate(int settlementId);
    }
}
