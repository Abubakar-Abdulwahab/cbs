using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSSettlementFeePartyManager<PSSSettlementFeeParty> : IDependency, IBaseManager<PSSSettlementFeeParty>
    {
        /// <summary>
        /// Sets all max percentage to false
        /// </summary>
        /// <param name="settlementId"></param>
        void SetMaxPercentageToFalse(int settlementId);

        /// <summary>
        /// Sets the max percentage for settlement fee party
        /// </summary>
        /// <param name="settlementId"></param>
        void SetMaxPercentage(int settlementId);
    }
}
