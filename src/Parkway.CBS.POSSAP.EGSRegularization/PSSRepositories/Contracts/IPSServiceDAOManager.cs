using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSServiceDAOManager : IRepository<PSService>
    {
        /// <summary>
        /// Get service with specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        PSServiceVM GetService(int id);


        /// <summary>
        /// Gets flow definition id for service with specified id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        int GetFlowDefinitionForService(int serviceId);
    }
}
