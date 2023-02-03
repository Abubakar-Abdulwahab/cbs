using Orchard;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Module.Controllers.Handlers.Contracts
{
    public interface IInvoiceHandler : IDependency
    {
        /// <summary>
        /// Get the invoice URL of this invoice with this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>string</returns>
        string GetInvoiceURL(string invoiceNumber);


        /// <summary>
        /// Search for tax payer by either TIN or Phone number
        /// <para>Does a count and returns true if count is 1 or false otherwise</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>List{TaxEntity}</returns>
        List<TaxPayerWithDetails> SearchForTaxPayer(SearchForTaxEntityVM model);


        /// <summary>
        /// Get tax payer for confirmation
        /// </summary>
        /// <param name="id"></param>
        /// <returns>GenerateInvoiceConfirmTaxPayer</returns>
        GenerateInvoiceConfirmTaxPayer GetTaxPayerAndRevenueHeads(string id);


        /// <summary>
        /// Get tax payer details
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <returns>TaxPayerWithDetails</returns>
        TaxPayerWithDetails GetTaxPayer(string taxPayerId);


        /// <summary>
        /// Get view for tax payer search for invoice generation
        /// </summary>
        /// <returns>SearchForTaxEntityVM</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        SearchForTaxEntityVM GetViewForSearchForTaxEntity();


        /// <summary>
        /// Get revenue heads details
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <returns>RevenueHeadDetails</returns>
        RevenueHeadDetails GetRevenueHeadDetails(string revenueHeadId);

        /// <summary>
        /// Get the view for input
        /// </summary>
        /// <param name="taxPayer"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <returns>ViewToShowVM</returns>
        ViewToShowVM GetViewForCreateBill(TaxPayerWithDetails taxPayer, RevenueHeadDetails revenueHeadDetails);


        /// <summary>
        /// Validate invoice confirmation
        /// </summary>
        /// <param name="model"></param>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>RevenueHeadDetails</returns>
        RevenueHeadDetails ValidateInvoiceDataInput(AdminGenerateInvoiceVM model, string revenueHeadId, int categoryId, ref List<ErrorModel> errors);


        /// <summary>
        /// If there was an error in the invoice data input page, we need to get the view model back
        /// </summary>
        /// <param name="model"></param>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <returns>ViewToShowVM</returns>
        ViewToShowVM GetCallBackViewModelForInvoiceDataInput(AdminGenerateInvoiceVM model, string taxPayerId, RevenueHeadDetails revenueHeadDetails);

        /// <summary>
        /// Add errors to model state
        /// </summary>
        /// <param name="invoiceController"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        void AddErrorsToModelState(InvoiceController invoiceController, List<ErrorModel> errors);


        /// <summary>
        /// Get model for confirming invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="taxPayerId"></param>
        /// <returns>AdminConfirmingInvoiceVM</returns>
        AdminConfirmingInvoiceVM GetConfirmingInvoiceVM(AdminGenerateInvoiceVM model, RevenueHeadDetails revenueHeadDetails, TaxPayerWithDetails taxPayer);


        /// <summary>
        /// Get tamper proof tokens
        /// <para>return a dynamic object with the values SubToken and Token</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>dynamic</returns>
        dynamic GetTamperProofTokens(string jsonModel);


        /// <summary>
        /// Get AdminConfirmingInvoiceVM model
        /// </summary>
        /// <param name="token"></param>
        /// <param name="subToken"></param>
        /// <returns>AdminConfirmingInvoiceVM</returns>
        AdminConfirmingInvoiceVM GetAdminConfirmingInvoiceModel(string token, string subToken);


        /// <summary>
        /// Get create invoice model
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>CreateInvoiceModel</returns>
        CreateInvoiceModel GetCreateInvoiceModel(AdminConfirmingInvoiceVM model);


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="createInvoiceModel"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGeneratedResponseExtn GenerateInvoice(CreateInvoiceModel createInvoiceModel);



        InvoiceGeneratedResponseExtn GetInvoiceDetails(string invoiceNumber);


        /// <summary>
        /// Get form controls for this taxpayer and revenue head
        /// </summary>
        /// <param name="taxPayerId"></param>
        /// <param name="revenueHeadId"></param>
        /// <returns>AdminGenerateInvoiceVM</returns>
        AdminGenerateInvoiceVM GetDetailsGenerateInvoiceDetailsWithFormFields(string taxPayerId, string revenueHeadId);


        /// <summary>
        /// Get payment refs used for this invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        InvoiceDetails GetInvoicePaymentRefs(string invoiceNumber);

    }
}
