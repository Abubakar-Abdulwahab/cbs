using Orchard;
using System.Web;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Module.API.Controllers.Handlers.Contracts
{
    public interface IAPIInvoiceHandler : IDependency
    {
        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse GenerateInvoice(InvoiceController callback, CreateInvoiceUserInputModel model, dynamic headerParams);


        APIResponse CreateInvoice(CreateInvoiceModel model, dynamic headerParams);


        /// <summary>
        /// Process paye assessments
        /// </summary>
        /// <param name="integrationController"></param>
        /// <param name="model"></param>
        /// <param name="p"></param>
        /// <returns>APIResponse</returns>
        APIResponse ProcessPayeeInvoice(InvoiceController invoiceController, ProcessPayeModel model, HttpPostedFile file, dynamic headerParams);


        /// <summary>
        /// Validate invoice
        /// </summary>
        /// <param name="ValidationRequest"></param>
        /// <param name="flatScheme"></param>
        /// <returns>APIResponse</returns>
        APIResponse ValidateInvoice(ValidationRequest model, bool flatScheme = false);


        /// <summary>
        /// Validate invoice 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse InvoiceValidation(ValidationRequest model, dynamic headerParams);

        /// <summary>
        /// Validate invoice for readycash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse ValidateInvoice(ReadycashInvoiceValidationModel model);

        /// <summary>
        /// Do invoice validate for NIBSS Ebills Pay
        /// </summary>
        /// <param name="invoiceController"></param>
        /// <param name="requestStreamString"></param>
        /// <returns>APIResponse</returns>
        APIResponse ValidateInvoiceNIBSS(string requestStreamString);

        /// <summary>
        /// Create batch invoices
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        APIResponse BatchInvoiceResponse(InvoiceController callback, CashflowBatchCustomerAndInvoicesResponse model);

        /// <summary>
        /// Get invoice status with payment history
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse GetInvoiceStatus(ValidationRequest model, dynamic headerParams);

        /// <summary>
        /// Invalidate an invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="headerParams"></param>
        /// <returns>APIResponse</returns>
        APIResponse InvalidateInvoice(ValidationRequest model, dynamic headerParams);

        /// <summary>
        /// Do model check
        /// </summary>
        /// <param name="callback">InvoiceController</param>
        /// <returns>List{ErrorModel}</returns>
        List<ErrorModel> DoModelCheck(InvoiceController callback);

    }
}
