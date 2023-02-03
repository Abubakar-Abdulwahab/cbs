using Parkway.CBS.Payee.PayeeAdapters.ReferenceData;
using Parkway.CBS.Payee.ReferenceDataImplementation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.ClientRepository.Repositories.ReferenceDataImpl.Contracts
{
    public interface IReferenceDataBatchRecordsDAOManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="referenceDataLineRecords"></param>
        /// <returns></returns>
        int SaveReferenceDataRecords(string tenantName, string LGAId, long recordId, ConcurrentStack<ReferenceDataLineRecordModel> referenceDataLineRecords);

        /// <summary>
        /// Match the Reference Data records to the Revenue Head Mapping using the Serial Number and Batch Id.
        /// </summary>
        /// <param name="batchId"></param>
        void MatchReferenceDataRecordsToTypeOfTaxpaid(long batchId);

        /// <summary>
        /// Move the Reference Data records to the Tax Entity Staging.
        /// </summary>
        /// <param name="batchId"></param>
        void MoveReferenceDataToTaxEntityStaging(long batchId);

    }
}
