using Orchard;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITransactionLogManager<TransactionLog> : IDependency, IBaseManager<TransactionLog>
    {

        /// <summary>
        /// this gets the total amount paid for the invoice.
        /// <para>Includes credits and debits</para>
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>decimal</returns>
        decimal GetSumOfAmountPaidForInvoice(string invoiceNumber);


        /// <summary>
        /// Get details for a particular payment log Id for channel
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        TransactionLogGroup GetGroupedTransactionLogByPaymentLogId(string paymentLogId, PaymentProvider provider);


        /// <summary>
        /// Get details for a particular payment reference for this channel
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        TransactionLogGroup GetGroupedTransactionLogByPayment(string paymentRef, PaymentProvider provider);


        /// <summary>
        /// Get the transaction for pay direct reversal
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <param name="payDirect"></param>
        /// <returns></returns>
        TransactionLogGroup GetTransactionForPaydirectReversal(string paymentLogId);


        /// <summary>
        /// Get details for a particular payment log Id for channel with reversal
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        TransactionLogGroup GetGroupedTransactionLogByPaymentLogIdWithReversal(string originalPaymentLogId, PaymentProvider payDirect);


        /// <summary>
        /// When a reversal has happened, we set the initial transaction reversed flag as true
        /// so as to indicate that this transaction has been reversed
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="payDirect"></param>
        /// <returns>bool</returns>
        bool UpdateTransactionToReversed(string paymentReference, PaymentProvider payDirect);

        /// <summary>
        /// Get details for a particular payment reference for this provider
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <param name="provider"></param>
        /// <returns>InvoiceDetails</returns>
        InvoiceDetails GetTransactionLogByPaymentReference(string paymentRef, PaymentProvider provider);


        /// <summary>
        /// Get transaction log with retrieval reference number
        /// </summary>
        /// <param name="transactionRef"></param>
        /// <param name="paymentProviderId"></param>
        /// <param name="channel"></param>
        /// <returns>TransactionLogGroup</returns>
        TransactionLogGroup GetGroupedTransactionLogByRetrievalReferenceNumber(string invoiceNumber, string retrievalReferenceNumber, int paymentProviderId, PaymentChannel channel);


        /// <summary>
        /// Get transaction log vm for receipt with specified receipt number
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns></returns>
        IEnumerable<TransactionLogVM> GetTransactionLogsForReceipt(string receiptNumber);

    }
}
