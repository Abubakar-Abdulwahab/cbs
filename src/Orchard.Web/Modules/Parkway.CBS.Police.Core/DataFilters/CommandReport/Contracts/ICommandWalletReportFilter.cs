using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.CommandReport.Contracts
{
    public interface ICommandWalletReportFilter : IDependency
    {
        /// <summary>
        /// Get view model for command reports
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfActiveCommand }</returns>
        dynamic GetCommandWalletReportViewModel(CommandWalletReportSearchParams searchParams);
    }
}
