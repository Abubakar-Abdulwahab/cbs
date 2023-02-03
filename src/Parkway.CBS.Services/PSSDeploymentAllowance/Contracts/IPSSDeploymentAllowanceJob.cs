using System;

namespace Parkway.CBS.Services.PSSDeploymentAllowance.Contracts
{
    public interface IPSSDeploymentAllowanceJob
    {
        /// <summary>
        /// This queue deployment allowance request on Hangfire
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <param name="tenantName"></param>
        void QueueDeploymentAllowanceRequest(long deploymentAllowanceRequestId, string tenantName = "POSSAP");
    }
}
