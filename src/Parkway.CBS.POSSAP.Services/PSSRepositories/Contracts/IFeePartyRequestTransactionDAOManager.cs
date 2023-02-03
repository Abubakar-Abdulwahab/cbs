using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IFeePartyRequestTransactionDAOManager : IRepository<PSSSettlementFeePartyRequestTransaction>
    {

        /// <summary>
        /// Here we pair the fee parties for this particular settlement with the transaction
        /// request, confirguration for eventual computation
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="serviceId"></param>
        void PairFeePartyWithoutAdditionalSplitsWithTransactionRequestConfigurationTransaction(long batchId, int serviceId);

        /// <summary>
        /// When then fee party and transaction, requests have been paired we need to
        /// do the calculation for the max percentage
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="serviceId"></param>
        void DoCalculationForMaxPercentage(long batchId, int serviceId);

    }

    
}
