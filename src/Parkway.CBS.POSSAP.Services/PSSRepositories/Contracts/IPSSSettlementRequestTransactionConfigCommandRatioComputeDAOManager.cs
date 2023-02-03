using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigCommandRatioComputeDAOManager : IRepository<PSSSettlementRequestTransactionConfigCommandRatioCompute>
    {

        /// <summary>
        /// When we have gotten the commands that serviced the request, we need to compute their fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        void AddRecordsFromRatioToCompute(long batchId);


        /// <summary>
        /// Update the compute table with the sum
        /// of the command ratio when grouped by config and batch
        /// </summary>
        /// <param name="batchId"></param>
        void SumCommandRatio(long batchId);


        /// <summary>
        /// Calculate ratio amount for non fall flag records
        /// </summary>
        /// <param name="batchId"></param>
        void CalculateRatioAmount(long batchId);

        /// <summary>
        /// When every other ratio amount has been calculated we
        /// need to get the value for the fall ratio which would be a sum of all not fall ratio
        /// substracted from the fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateFallRatio(long batchId);
    }
}
