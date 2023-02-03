using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Client.Controllers.Handlers.Contracts
{
    public interface IPSSMakePaymentHandler : IDependency
    {
        InvoiceGeneratedResponseExtn GetInvoiceDetails(string invoiceNumber);

        /// <summary>
        /// Get Payment Reference using the Invoice Number
        /// </summary>
        /// <param name="InvoiceId"></param>
        /// <param name="provider"></param>
        /// <returns>string</returns>
        APIResponse GetPaymentReferenceNumber(int InvoiceId, string InvoiceNumber, PaymentProvider provider);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns>PaymentReferenceVM</returns>
        PaymentReferenceVM GetPaymentReferenceDetail(string referenceNumber);

        /// <summary>
        /// Save Netpay payment details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<InvoiceValidationResponseModel> SavePayment(PaymentAcknowledgeMentModel model);

        /// <summary>
        /// Send sms notification
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cbsUser"></param>
        void SendSMSNotification(PaymentAcknowledgeMentModel model, CBSUserVM cbsUser);

        /// <summary>
        /// Gets cbs user vm for user that generated invoice with specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        CBSUserVM GetCBSUserWithInvoiceNumber(string invoiceNumber);
    }
}
