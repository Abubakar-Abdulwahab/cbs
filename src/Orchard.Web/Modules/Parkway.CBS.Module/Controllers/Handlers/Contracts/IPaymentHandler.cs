using Orchard;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.HelperModels;
using Orchard.Security.Permissions;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IPaymentHandler : IDependency
    {
        /// <summary>
        /// Get view model for add payment to invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>AddPaymentVM</returns>
        AddPaymentVM GetAddPaymentVM(string invoiceNumber);


        /// <summary>
        /// update invoice payment
        /// </summary>
        /// <param name="model">AddPaymentVM</param>
        void UpdateInvoicePayment(PaymentController callback, AddPaymentVM model);

        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission permission);


        /// <summary>
        /// Get byte doc for receipt
        /// </summary>
        /// <param name="receiptNumber"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        CreateReceiptDocumentVM CreateReceiptByteFile(string receiptNumber);

    }
}
