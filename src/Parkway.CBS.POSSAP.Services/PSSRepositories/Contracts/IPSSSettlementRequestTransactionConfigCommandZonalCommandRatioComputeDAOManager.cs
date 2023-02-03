using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigCommandZonalCommandRatioComputeDAOManager : IRepository<PSSSettlementRequestTransactionConfigZonalCommandRatioCompute>
    {

        /// <summary>
        /// Move PSSSettlementRequestTransactionConfigZonalCommandRatio to zonal ratio compute table
        /// </summary>
        /// <param name="batchId"></param>
        void MoveZonalRatioToComputeTable(long batchId);


        /// <summary>
        /// Updtae the compute table with the ratio sum
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateZonalRatioSumOnComputeTable(long batchId);

        /// <summary>
        /// update the ratio amount for non flag value 
        /// here we take the percentage from the fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateTheRatioAmountForNonFlagRecordsBasedOffFeePercentage(long batchId);


        /// <summary>
        /// Here once we have the ratio amount of the non flag value
        /// we get the sum of those value and use it in the computation for the flag value
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateTheRatioAmountForFlagRecordsBasedOffRatioAmountSum(long batchId);

    }
}
