using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IDeploymentReportHandler : IDependency
    {
        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="canViewRequests"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission permission);


        /// <summary>
        /// Get view model for deployments
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>DeploymentReportVM</returns>
        DeploymentReportVM GetVMForReports(OfficerDeploymentSearchParams searchParams);

    }
}
