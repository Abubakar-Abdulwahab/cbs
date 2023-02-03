using Orchard;
using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreReceiptUtilizationService : IDependency
    {
        /// <summary>
        /// Get batch record with specified batch ref
        /// </summary>
        /// <param name="batchRef"></param>
        /// <returns></returns>
        PAYEBatchRecordVM GetBatchRecordWithBatchRef(string batchRef);

        /// <summary>
        /// Get amount paid for batch with specified Id
        /// </summary>
        /// <param name="batchId"></param>
        /// <returns></returns>
        decimal GetBatchAmountPaidWithId(Int64 batchId);

        /// <summary>
        /// Get PAYE Receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        PAYEReceiptVM GetPAYEReceiptVMWithNumber(string receiptNumber, long userId);

        /// <summary>
        /// Apply receipt with specified receipt number to batch with specified batch ref
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <param name="batchRef"></param>
        /// <param name="userId"></param>
        bool ApplyReceiptToBatch(string receiptNumber, string batchRef, long userId);

        /// <summary>
        /// Get receipts utilized for batch record with the specified Id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        IEnumerable<PAYEReceiptVM> GetUtilizedReceiptsForBatch(long batchRecordId);

        /// <summary>
        /// Get Revenue Head Id for PAYE Assessment
        /// </summary>
        /// <returns></returns>
        int GetRevenueHeadIdForPAYE();

        /// <summary>
        /// Get invoice number of unpaid invoice for batch with specified batch record id
        /// </summary>
        /// <param name="batchRecordId"></param>
        /// <returns></returns>
        IEnumerable<string> GetUnpaidInvoiceNumberForBatch(Int64 batchRecordId);
    }
}
