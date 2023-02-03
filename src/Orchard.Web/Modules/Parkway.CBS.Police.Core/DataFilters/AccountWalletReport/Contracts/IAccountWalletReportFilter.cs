using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.AccountWalletReport.Contracts
{
    public interface IAccountWalletReportFilter : IDependency
    {
        /// <summary>
        /// Get view model for Account Wallet report
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalAccountWalletRecord }</returns>
        dynamic GetAccountWalletReportViewModel(AccountWalletReportSearchParams searchParams);
    }
}
