using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigCommandStateCommandDAOManager : IRepository<PSSSettlementRequestTransactionConfigCommandStateCommand>
    {

        /// <summary>
        /// merge the command transaction request with the state and zonal command 
        /// pairing of the transaction, request, the commands, state and zonal commands
        /// </summary>
        /// <param name="batchId"></param>
        void MergeCommandTransactionRequestWithStateCommand(long batchId);

    }
}
