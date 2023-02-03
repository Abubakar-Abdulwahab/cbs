using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletPaymentReport.Contracts
{
    public interface IAccountWalletPaymentReportFilter : IDependency
    {
        /// <summary>
        /// Get view model for Account Wallet payment report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalAccountWalletPaymentReportRecord }</returns>
        dynamic GetAccountWalletPaymentReportViewModel(AccountWalletPaymentSearchParams searchParams);
    }
}
