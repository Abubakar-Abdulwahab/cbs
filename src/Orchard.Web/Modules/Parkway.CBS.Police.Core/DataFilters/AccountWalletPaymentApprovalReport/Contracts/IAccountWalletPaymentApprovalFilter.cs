using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentApprovalReport.Contracts
{
    public interface IAccountWalletPaymentApprovalFilter : IDependency
    {
        /// <summary>
        /// Get view model for Account Wallet payment approval report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalAccountWalletPaymentApprovalRecord }</returns>
        dynamic GetAccountWalletPaymentApprovalReportViewModel(AccountWalletPaymentApprovalSearchParams searchParams);
    }
}
