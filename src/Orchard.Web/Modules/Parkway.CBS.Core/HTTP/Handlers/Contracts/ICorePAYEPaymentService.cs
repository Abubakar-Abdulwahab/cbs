using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICorePAYEPaymentService : IDependency
    {
        /// <summary>
        /// This method is used to do payment processing
        /// for PAYE assessments when payment has been done
        /// </summary>
        /// <param name="transactionLogs">list of transactions that contain the designated PAYE revenue head items</param>
        /// <param name="invoiceId">invoice Id</param>
        /// <param name="receiptId">receipt Id</param>
        void ProcessPAYEPayment(List<TransactionLogVM> transactionLogs, long invoiceId, long receiptId);


        /// <summary>
        /// This method is used to do payment processing
        /// for PAYE assessments with batch that do not have an invoice attached
        /// </summary>
        /// <param name="transactionLogs">Transaction log for the PAYE Receipt</param>
        /// <param name="receipt">PAYE receipt</param>
        /// <param name="batch">Batch which the receipt will be applied to</param>
        void ProcessPAYEPaymentForApplyingReceipt(List<TransactionLogVM> transactionLogs, PAYEReceiptVM receipt, PAYEBatchRecordVM batch);


        /// <summary>
        /// Get the reveneue head for PAYE assessment
        /// </summary>
        /// <returns>int</returns>
        int GetPAYERevenueHeadId();

    }
}
