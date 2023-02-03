using Orchard;
using Orchard.Security.Permissions;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IAccountWalletPaymentReportHandler : IDependency
    {
        /// <summary>
        /// Check for user permission
        /// </summary>
        /// <param name="canViewWalletPaymentReport"></param>
        void CheckForPermission(Permission canViewWalletPaymentReport);

        /// <summary>
        /// Gets the Account Wallet Payment Report VM
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        AccountWalletPaymentReportVM GetAccountWalletPaymentReportVM(AccountWalletPaymentSearchParams searchParams);
    }
}
