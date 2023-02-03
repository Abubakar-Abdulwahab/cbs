using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.Models;

namespace Parkway.CBS.POSSAP.EGSRegularization.PSSRepositories.Contracts
{
    public interface IPSServiceRequestFlowDefinitionLevelDAOManager : IRepository<PSServiceRequestFlowDefinitionLevel>
    {
        /// <summary>
        /// Get the last level defintiion with the specified workflow action value in flow definition with specified id
        /// </summary>
        /// <param name="serviceId"></param>
        PSServiceRequestFlowDefinitionLevelDTO GetLastLevelDefinitionWithWorkflowActionValue(int definitionId, Police.Core.Models.Enums.RequestDirection actionValue);
    }
}
