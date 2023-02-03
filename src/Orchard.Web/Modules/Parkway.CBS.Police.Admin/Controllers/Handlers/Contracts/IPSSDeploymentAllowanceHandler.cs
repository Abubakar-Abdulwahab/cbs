using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;
using System;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSDeploymentAllowanceHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="permission"></param>
        void CheckForPermission(Permission permission);

        /// <summary>
        /// Get view model for deployment allowance requests
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>DeploymentAllowanceReportVM</returns>
        DeploymentAllowanceReportVM GetVMForReports(OfficerDeploymentAllowanceSearchParams searchParams);


        /// <summary>
        /// Get the deployment allowance request details using deployment allowance request id
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <returns>EscortDeploymentRequestDetailsVM</returns>
        EscortDeploymentRequestDetailsVM GetDeploymentAllowanceRequestDetails(Int64 deploymentAllowanceRequestId);


        /// <summary>
        /// Save approval details
        /// </summary>
        /// <param name="deploymentAllowanceRequestId"></param>
        /// <param name="sRequestFormDump"></param>
        /// <returns>bool</returns>
        bool SaveRequestApprovalDetails(long deploymentAllowanceRequestId, dynamic sRequestFormDump);

    }
}