using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSSettlementBatchItemsManager<PSSSettlementBatchItems> : IDependency, IBaseManager<PSSSettlementBatchItems>
    {
        /// <summary>
        /// Gets PSS Settlement Batch Items Records
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        IEnumerable<PSSSettlementBatchItemsVM> GetReportRecords(PSSSettlementReportSearchParams searchParams);

        /// <summary>
        /// Get report aggregate
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>IEnumerable{FileUploadReport}</returns>
        IEnumerable<long> GetCount(long batchId);
    }
}
