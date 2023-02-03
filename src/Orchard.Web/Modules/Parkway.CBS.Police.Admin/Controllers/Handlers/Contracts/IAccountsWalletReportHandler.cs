using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountsWalletReportHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewAccountWalletReport"></param>
        void CheckForPermission(Permission canViewAccountWalletReport);

        /// <summary>
        /// Get Reports VM for account wallet report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        AccountsWalletReportVM GetVMForReports(AccountWalletReportSearchParams searchParams);

    }
}
