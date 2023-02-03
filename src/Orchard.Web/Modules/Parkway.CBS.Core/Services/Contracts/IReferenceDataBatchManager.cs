using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IReferenceDataBatchManager<ReferenceDataBatch> : IDependency, IBaseManager<ReferenceDataBatch>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        ReferenceDataBatch GetBatch(Int64 batchId);

        /// <summary>
        /// Get ReferenceData Batch Details using the General Reference Id
        /// </summary>
        /// <param name="generalBatchId"></param>
        /// <returns></returns>
        ReferenceDataBatch GetBatchDetails(Int64 generalBatchId);
    }
}
