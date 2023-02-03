using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementPercentageRecalculationFeePartyBatchAggregateDAOManager : IRepository<PSSSettlementPercentageRecalculationFeePartyBatchAggregate>
    {
        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveAdditionalSplitsToPercentageRecalculationAggregateTable(long batchId, string adapterValue);


        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits and additional split value State
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveAdditionalSplitsForStateToPercentageRecalculationAggregateTable(long batchId, string adapterValue);


        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits and additional split value Zonal
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveAdditionalSplitsForZonalToPercentageRecalculationAggregateTable(long batchId, string adapterValue);

        /// <summary>
        /// When split have been made we move the 
        /// aggregates for the fee parties with additional splits and additional split value in Adapter command table
        /// to the percentage recalculation aggregate table
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="adapterValue"></param>
        void MoveAdditionalSplitsForAdapterCommandToPercentageRecalculationAggregateTable(long batchId, string adapterValue);


        /// <summary>
        /// When we are done command transaction ratio, we need to set one of the value as the fall value
        /// for computation
        /// </summary>
        /// <param name="batchId"></param>
        void SetFallRatioFlag(long batchId);


        /// <summary>
        /// Computes command percentage for non fall flags
        /// </summary>
        /// <param name="batchId"></param>
        void ComputeCommandPercentageForNonFallFlags(long batchId);


        /// <summary>
        /// Computes command percentage for fall flags
        /// </summary>
        /// <param name="batchId"></param>
        void ComputeCommandPercentageForFallFlags(long batchId);

    }
}
