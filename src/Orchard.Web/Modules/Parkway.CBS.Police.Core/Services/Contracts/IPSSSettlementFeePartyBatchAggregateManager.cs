using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSSettlementFeePartyBatchAggregateManager<PSSSettlementFeePartyBatchAggregate> : IDependency, IBaseManager<PSSSettlementFeePartyBatchAggregate>
    {
        /// <summary>
        /// Get fee party batch aggregate model for fee party with specified id belonging to batch with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <param name="feePartyBatchAggregateId"></param>
        /// <returns></returns>
        PSSSettlementFeePartyBatchAggregateVM GetFeePartyBatchInfo(string batchRef, long feePartyBatchAggregateId);
    }
}
