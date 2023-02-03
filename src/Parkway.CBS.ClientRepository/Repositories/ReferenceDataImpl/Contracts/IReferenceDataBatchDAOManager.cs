using Parkway.CBS.ClientRepository.Repositories.Contracts;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IReferenceDataBatchDAOManager : IRepository<ReferenceDataBatch>
    {
        /// <summary>
        /// Get Reference Data Batch record using batchId
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns>ReferenceDataBatch</returns>
        ReferenceDataBatch GetBatchRecord(long batchId);

        /// <summary>
        /// Get Reference Data Batch record using GeneralBatchReference Id
        /// </summary>
        /// <param name="generalBatchReferenceId"></param>
        /// <returns>ReferenceDataBatch</returns>
        ReferenceDataBatch GetBatchDetails(long generalBatchReferenceId);
    }
}
