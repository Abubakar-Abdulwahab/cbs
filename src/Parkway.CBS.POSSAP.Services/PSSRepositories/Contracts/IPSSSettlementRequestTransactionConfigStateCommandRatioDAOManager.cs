using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigStateCommandRatioDAOManager : IRepository<PSSSettlementRequestTransactionConfigStateCommandRatio>
    {

        /// <summary>
        /// insert into the PSSSettlementRequestTransactionConfigStateCommandRatio table the 
        /// pairing of the transaction, request, commands ans state command
        /// </summary>
        /// <param name="batchId"></param>
        void SortCommandTransactionRequestByStateCommandRatio(long batchId);


        /// <summary>
        /// When we are done with the count for each state command
        /// we need to set the fall flag value to each group
        /// </summary>
        /// <param name="batchId"></param>
        void SetFallFlagForStateCommand(long batchId);

    }
}
