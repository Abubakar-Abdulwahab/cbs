using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSSettlementFeePartyStagingManager<PSSSettlementFeePartyStaging> : IDependency, IBaseManager<PSSSettlementFeePartyStaging>
    {
        /// <summary>
        /// Sets IsDeleted to true for the removed settlement fee party in PSSSettlementFeeParty
        /// </summary>
        /// <param name="reference"></param>
        void UpdateSettlementFeePartyFromStaging(string reference);
    }
}
