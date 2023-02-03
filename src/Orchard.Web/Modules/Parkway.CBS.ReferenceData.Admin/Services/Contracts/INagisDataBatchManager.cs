using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ReferenceData.Admin.Services.Contracts
{
    public interface INagisDataBatchManager<NagisDataBatch> : IDependency, IBaseManager<NagisDataBatch>
    {
        /// <summary>
        /// Get batch ref from Nagis data batch
        /// </summary>
        /// <param name="id"></param>
        /// <returns>string</returns>
        string GetBatchRef(int id);

        /// <summary>
        /// Get GetNagisDataBatch using BatchRef
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns>ReferenceDataBatch</returns>
        NagisDataBatch GetNagisDataBatch(string batchRef);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<NagisDataBatch> GetBatchRecords();
    }
}
