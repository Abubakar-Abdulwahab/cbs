using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.PSSUserReport.Contracts
{
    public interface IPSSUserReportFilter : IDependency
    {
        /// <summary>
        /// Gets the view model based of the filter parameter
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        dynamic GetUserReportViewModel(PSSUserReportSearchParams searchParams);
    }
}
