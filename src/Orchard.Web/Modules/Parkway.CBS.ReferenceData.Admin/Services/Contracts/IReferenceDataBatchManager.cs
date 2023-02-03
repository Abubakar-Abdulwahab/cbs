using Orchard;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.ReferenceData.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Admin.Services.Contracts
{
    public interface IReferenceDataBatchManager<ReferenceDataBatch> : IDependency, IBaseManager<ReferenceDataBatch>
    {
        IEnumerable<ReferenceDataBatch> GetBatchRecords(int skip, int take, ReferenceDataBatchSearchParams searchParams);

        IEnumerable<ReferenceDataBatchReportStats> GetAggregateForBatchRecords(ReferenceDataBatchSearchParams searchParams);

        /// <summary>
        /// Get batch ref from reference data batch
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string</returns>
        string GetBatchRef(int id);

        /// <summary>
        /// Get ReferenceDataBatch using BatchRef
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns>ReferenceDataBatch</returns>
        ReferenceDataBatch GetReferenceDataBatch(string batchRef);
    }
}
