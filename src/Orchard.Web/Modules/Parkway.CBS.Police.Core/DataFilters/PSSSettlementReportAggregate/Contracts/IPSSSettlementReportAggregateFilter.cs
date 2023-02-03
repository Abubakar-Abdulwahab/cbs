using Orchard;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.DataFilters.PSSSettlementReportAggregate.Contracts
{
    public interface IPSSSettlementReportAggregateFilter : IDependency
    {
        /// <summary>
        /// Get veiw model for settlement report aggregate
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>new { ReportRecords, TotalNumberOfSettlements }</returns>
        dynamic GetRequestReportViewModel(PSSSettlementReportAggregateSearchParams searchParams);
    }
}
