using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.ClientRepository.Repositories.Contracts;

namespace Parkway.CBS.POSSAP.Services.PSSRepositories.Contracts
{
    public interface IPSSServiceSettlementConfigurationTransactionDAOManager : IRepository<PSSServiceSettlementConfigurationTransaction>
    {

        /// <summary>
        /// insert into the PSSServiceSettlementConfigurationTransaction table the 
        /// pairing of the transaction and the configurations
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="psssettlementId"></param>
        /// <param name="serviceId"></param>
        void PairTransactionWithConfigurations(long batchId, int psssettlementId, int serviceId);

    }
}
