using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSUserReportHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewUsers"></param>
        void CheckForPermission(Permission canViewUsers);

        /// <summary>
        /// Gets the view model for report display
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        PSSUserReportVM GetVMForReports(PSSUserReportSearchParams searchParams);

        /// <summary>
        /// Revalidate user with specified payer id
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="errorMessage"></param>
        void RevalidateUserWithIdentificationNumber(string payerId, out string errorMessage);
    }
}
