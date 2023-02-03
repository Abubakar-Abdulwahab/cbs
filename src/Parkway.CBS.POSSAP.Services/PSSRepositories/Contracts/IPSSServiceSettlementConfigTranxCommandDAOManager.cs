using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSServiceSettlementConfigTranxCommandDAOManager : IRepository<PSSSettlementRequestTransactionConfigCommand>
    {

        /// <summary>
        /// insert into the PSSSettlementRequestTransactionConfigCommand table the 
        /// pairing of the transaction, request and the commands
        /// </summary>
        /// <param name="batchId"></param>
        void PairTransactionWithCommand(long batchId);

    }
}
