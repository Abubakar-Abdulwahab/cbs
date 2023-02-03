using Parkway.CBS.ClientRepository.Repositories.Models;

namespace Parkway.CBS.ClientRepository.Repositories.DeploymentAllowance.Contracts
{
    public interface IPSSDeploymentAllowanceSettlementEngineDetailsDAOManager
    {
        /// <summary>
        /// Get the deployment allowance settlement engine request details
        /// </summary>
        /// <param name="settlementAllowanceRequestId"></param>
        /// <returns>DeploymentAllowanceSettlementVM</returns>
        DeploymentAllowanceSettlementVM GetDeploymentAllowanceSettlementDetails(long settlementAllowanceRequestId);


        /// <summary>
        /// Update deployment allowance settlement details after sending the request to the Settlement Engine
        /// </summary>
        /// <param name="deploymentAllowanceVM"></param>
        void UpdateDeploymentAllowanceSettlementDetails(DeploymentAllowanceSettlementVM deploymentAllowanceVM);
    }
}
