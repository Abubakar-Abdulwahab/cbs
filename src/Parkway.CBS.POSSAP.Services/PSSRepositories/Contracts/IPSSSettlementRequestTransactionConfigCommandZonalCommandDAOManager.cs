using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;


namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSSettlementRequestTransactionConfigCommandZonalCommandDAOManager : IRepository<PSSSettlementRequestTransactionConfigCommandZonalCommand>
    {

        /// <summary>
        /// merge the command transaction request with the zonal command 
        /// pairing of the transaction, request, the commands, and zonal commands
        /// </summary>
        /// <param name="batchId"></param>
        void MergeCommandTransactionRequestWithZonalCommand(long batchId);


        /// <summary>
        /// When we have the records with the corresponding zonal command
        /// we need to update the state and LGA columns with the state and LGA of the 
        /// zonal commnad
        /// </summary>
        /// <param name="batchId"></param>
        void UpdateBatchWithStateAndLGAOfZonalCommand(long batchId);

    }   
}
