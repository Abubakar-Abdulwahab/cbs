using Orchard;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.ThirdParty.Payment.Processor.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICorePaymentService : IDependency
    {

        /// <summary>
        /// update invoice payment 
        /// </summary>
        /// <param name="model">TransactionLog</param>
        /// <param name="provider">PaymentProvider</param>
        /// <param name="doNotAllowReferenceNumberReuse">this var defaults to true, what this means is that only one reference identifier for the same channel can be updated</param>
        /// <returns>InvoiceValidationResponseModel</returns>
        InvoiceValidationResponseModel UpdatePayment(TransactionLogVM model, PaymentProvider provider, bool doNotAllowReferenceNumberReuse = true, InvoiceDetailsHelperModel helperModel = null);


        /// <summary>
        /// Search for invoice number
        /// </summary>
        /// <param name="custReference"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGeneratedResponseExtn GetInvoiceDetails(string invoiceNumber);


        /// <summary>
        /// Get transaction with RetrievalReferenceNumber
        /// </summary>
        /// <param name="transactionRef"></param>
        /// <returns>TransactionLogGroup</returns>
        TransactionLogGroup GetTransactionLogByRetrievalReferenceNumber(string invoiceNumber, string retrievalRef, int paymentProviderId, PaymentChannel channel);


        /// <summary>
        /// Payment Reversal for pay direct
        /// <para></para>
        /// </summary>
        /// <param name="transactionLog"></param>
        /// <param name="payDirect"></param>
        /// <param name="originalPaymentReference"></param>
        /// <param name="originalPaymentLogId"></param>
        /// <returns>PaymentReversalResponseObj</returns>
        PaymentReversalResponseObj PaymentReversalForPayDirect(TransactionLogVM transactionLog, string originalPaymentReference, string originalPaymentLogId);


        /// <summary>
        /// Get transaction log by payment log Id
        /// </summary>
        /// <param name="paymentLogId"></param>
        /// <param name="channel"></param>
        /// <returns>TransactionLogGroup</returns>
        TransactionLogGroup GetTransactionLogByPaymentLogId(string paymentLogId, PaymentProvider channel);


        /// <summary>
        /// Send payment notification
        /// </summary>
        /// <param name="callBackURL"></param>
        /// <param name="transactionLog"></param>
        /// <param name="siteName"></param>
        /// <param name="messageEncryptionKey"></param>
        /// <param name="requestReference"></param>
        void SendNotifications(string callBackURL, TransactionLogVM transactionLog, string siteName, string messageEncryptionKey, string requestReference);


        /// <summary>
        /// Send notification for the specified payment reference and provider
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="provider"></param>
        /// <returns>APIResponse</returns>
        APIResponse SendNotifications(string paymentReference, PaymentProvider provider);



        /// <summary>
        /// Get the model for generating web payment for pay direct
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="payDirectConfig"></param>
        /// <returns>PayDirectWebPaymentModel</returns>
        PayDirectWebPaymentFormModel GetPayDirectWebFormParameters(PayDirectConfigurations payDirectConfig, string transactionRef, decimal amountDue);


        /// <summary>
        /// Get transaction ref for pay direct web
        /// </summary>
        /// <param name="payDirectConfig"></param>
        /// <param name="stateName"></param>
        /// <param name="amountDue"></param>
        /// <returns>string</returns>
        string GetTransactionRefForPayDirectWeb(PayDirectConfigurations payDirectConfig, decimal amountDue, decimal fee, PayDirectWebPaymentRequestModel tokenModel);


        /// <summary>
        /// Returns the fee to be paid 
        /// </summary>
        /// <param name="payDirectConfig"></param>
        /// <param name="amountDue"></param>
        /// <returns>decimal</returns>
        decimal GetFeeToBeAppliedForPayDirectWeb(PayDirectConfigurations payDirectConfig, decimal amountDue);


        /// <summary>
        /// Get transaction details for pay direct web
        /// </summary>
        /// <param name="model"></param>
        /// <returns>PayDirectWebServerResponse</returns>
        PayDirectWebServerResponse GetPayDirectWebTransaction(PayDirectConfigurations payDirectConfig, PayDirectWebPaymentResponseModel model);


        ///// <summary>
        ///// Get the web request for this transaction ref and payment channel
        ///// </summary>
        ///// <param name="txnref"></param>
        ///// <param name="payDirectWeb"></param>
        ///// <returns>WebPaymentRequest | null</returns>
        //WebPaymentRequest GetWebPaymentRequest(string txnref, PaymentChannel payDirectWeb);

        /// <summary>
        /// Get the amount paid less the transaction fee for pay direct web
        /// </summary>
        /// <param name="payDirectConfig"></param>
        /// <param name="amountPaid"></param>
        /// <param name="fee"></param>
        /// <returns>decimal</returns>
        decimal GetAmountPaidLessTransactionFeeForPayDirectWeb(PayDirectConfigurations payDirectConfig, int amountPaid, decimal fee);


        /// <summary>
        /// Get transaction group for the given provider and payment reference
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="provider"></param>
        /// <returns>TransactionLogGroup</returns>
        TransactionLogGroup GetByPaymentRef(string paymentReference, PaymentProvider provider);


        /// <summary>
        /// Save Reference Number, please make sure to catch the InvoiceAlreadyPaidForException thrown
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <returns></returns>
        PaymentReference SavePaymentReference(PaymentReference paymentReference);

        /// <summary>
        /// Get Payment Reference details using Id
        /// </summary>
        /// <param name="paymentReferenceId"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetPaymentReference(Int64 paymentReferenceId);


        /// <summary>
        /// Evict PaymentReference object from cache
        /// </summary>
        /// <param name="paymentReference"></param>
        void EvictPaymentReferenceObject(PaymentReference paymentReference);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns>PaymentReferenceVM</returns>
        PaymentReferenceVM GetPaymentReferenceDetail(string referenceNumber);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns>NetPayTransactionVM</returns>
        Task<NetPayTransactionVM> VerifyNetPayPayment(string referenceNumber);


        /// <summary>
        /// Save Netpay transaction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<InvoiceValidationResponseModel> SaveNetpayPayment(PaymentAcknowledgeMentModel model);
    }
}
