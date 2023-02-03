using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.PSSServiceType.Contracts
{
    public interface IPSSDeploymentAllowanceImpl : IDependency
    {
        /// <summary>
        /// Get the deployment allowance request details using deployment allowance request id
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <returns>EscortDeploymentRequestDetailsVM</returns>
        EscortDeploymentRequestDetailsVM GetRequestViewDetails(long deploymentAllowanceRequestId);


        /// <summary>
        /// Update the deployment allowance request status and queue the job for settlement
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <param name="requestVM"></param>
        /// <returns>RequestApprovalResponse</returns>
        bool SaveRequestApprovalDetails(long deploymentAllowanceRequestId, dynamic requestVM);
    }
}
