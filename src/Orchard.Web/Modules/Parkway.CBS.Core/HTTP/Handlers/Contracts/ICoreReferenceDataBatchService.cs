using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreReferenceDataBatchService : IDependency
    {
        /// <summary>
        /// Get Reference Data batch record using the BatchRef
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>ReferenceDataBatch</returns>
        ReferenceDataBatch GetBatch(Int64 batchId);

        /// <summary>
        /// Get ReferenceData Batch Details using the General Reference Id
        /// </summary>
        /// <param name="generalBatchId"></param>
        /// <returns></returns>
        ReferenceDataBatch GetBatchDetails(Int64 generalBatchId);

    }
}
