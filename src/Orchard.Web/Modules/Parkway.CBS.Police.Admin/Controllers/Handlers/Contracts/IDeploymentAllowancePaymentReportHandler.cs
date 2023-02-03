using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IDeploymentAllowancePaymentReportHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewDeploymentAllowancePaymentReport"></param>
        void CheckForPermission(Permission canViewDeploymentAllowancePaymentReport);


        /// <summary>
        /// Gets the Deployment Allowance Payment Report VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        DeploymentAllowancePaymentReportVM GetDeploymentAllowancePaymentReportVM(DeploymentAllowancePaymentSearchParams searchParams);
    }
}
