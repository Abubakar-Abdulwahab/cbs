using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigStateCommandRatioComputeDAOManager : IRepository<PSSSettlementRequestTransactionConfigStateCommandRatioCompute>
    {

        /// <summary>
        /// Move the state command ratio details to the compute table
        /// for further computation
        /// </summary>
        /// <param name="batchId"></param>
        void MoveStateCommandRatioToComputeTable(long batchId);


        /// <summary>
        /// Add ratio amount to state command on the compute
        /// table
        /// </summary>
        /// <param name="batchId"></param>
        void AddRatioAmountSumToStateCommand(long batchId);


        /// <summary>
        /// Update the ratio amount for non flaged values from the fee percentage
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateNoFlagWithRatioAmountFromFeePercentage(long batchId);


        /// <summary>
        /// Update the flag value ratio amount with the sum of the non flag values 
        /// ratio amount 
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateStateCommandFlagValueWithRatioAmountFromRatioAmountOfNonFlagRecords(long batchId);

    }
}
