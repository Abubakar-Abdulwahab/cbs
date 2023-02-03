using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.NAGIS.Contracts
{
    public interface INAGISDataBatchDAOManager : IRepository<NagisDataBatch>
    {
        /// <summary>
        /// Get Nagis Data Batch record using batchId
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>NagisDataBatch</returns>
        NagisDataBatch GetBatchRecord(long batchId);

        /// <summary>
        /// Get Nagis Data Batch record using GeneralBatchReference Id
        /// </summary>
        /// <param name="generalBatchReferenceId"></param>
        /// <returns>NagisDataBatch</returns>
        NagisDataBatch GetBatchDetails(long generalBatchReferenceId);

    }
}
