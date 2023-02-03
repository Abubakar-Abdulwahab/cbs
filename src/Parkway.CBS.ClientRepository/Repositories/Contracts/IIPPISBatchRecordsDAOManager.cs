using Parkway.CBS.Core.Models;
using Parkway.CBS.Payee.PayeeAdapters.IPPIS;
using System;
using System.Collections.Concurrent;

namespace Parkway.CBS.ClientRepository.Repositories.Contracts
{
    public interface IIPPISBatchRecordsDAOManager : IRepository<IPPISBatchRecords>
    {
        /// <summary>
        /// Save the paye records
        /// </summary>
        /// <param name="recordId"></param>
        /// <param name="payees"></param>
        int SaveIPPISRecords(Int64 recordId, ConcurrentStack<IPPISAssessmentLineRecordModel> payees);


        void GroupRecordsFromIPPISBatchRecordsTableByTaxPayerCode(long batchId);

        /// <summary>
        /// Map the TaxPayerCode to the Id of the TaxEntity that has the same agency code as the TaxPayerCode
        /// </summary>
        /// <param name="batchId"></param>
        void MapTaxPayerCodeToTaxEntityId(long batchId);

        /// <summary>
        /// Set the value of TaxEntity_Id == null to unknown tax entity value
        /// </summary>
        /// <param name="batchId"></param>
        /// <param name="unknownTaxPayerCodeId"></param>
        void MapNullTaxPayerCodeToUnknownTaxEntityId(long batchId, Int64 unknownTaxPayerCodeId);
    }
}
