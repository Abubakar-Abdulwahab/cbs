using Orchard;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.DataFilters.FeePartyReport.Contracts
{
    public interface IFeePartyReportFilter : IDependency
    {
        /// <summary>
        /// Gets view model containing settlement fee parties
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        dynamic GetFeePartyReportViewModel(FeePartyReportSearchParams searchParams);

    }
}
