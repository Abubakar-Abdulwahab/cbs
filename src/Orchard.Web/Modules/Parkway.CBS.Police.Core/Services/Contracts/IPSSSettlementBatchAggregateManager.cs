using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSSettlementBatchAggregateManager<PSSSettlementBatchAggregate> : IDependency, IBaseManager<PSSSettlementBatchAggregate>
    {
        /// <summary>
        /// Get PSS Settlement Batch Aggregate Records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<PSSSettlementBatchAggregateVM> GetReportRecords(PSSSettlementReportSearchParams searchParams);

        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<int> GetCount(PSSSettlementReportSearchParams searchParams);
    }
}
