using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigCommandRatioDAOManager : IRepository<PSSSettlementRequestTransactionConfigCommandRatio>
    {

        /// <summary>
        /// insert into the PSSSettlementRequestTransactionConfigCommandRatio table the 
        /// pairing of the ratio of the command, per transaction and request
        /// </summary>
        /// <param name="batchId"></param>
        void SortCommandTransactionRequestByCommandRatio(long batchId);


        /// <summary>
        /// When we are done command transaction ratio, we need to set one of the value as the fall value
        /// for computation
        /// </summary>
        /// <param name="batchId"></param>
        void SetFallRatioFlag(long batchId);

    }
}
