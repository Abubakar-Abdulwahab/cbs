using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IServiceWorkflowDifferentialDAOManager : IRepository<ServiceWorkflowDifferential>
    {
        /// <summary>
        /// Gets flow definition id for service workflow differential with specified differential details and service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="differential"></param>
        /// <returns></returns>
        int GetFlowDefinitionIdAttachedToServiceWithDifferentialValue(int serviceId, Police.Core.VM.ServiceWorkFlowDifferentialDataParam differential);
    }
}
