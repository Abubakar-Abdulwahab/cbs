using Orchard;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Orchard.Users.Models;


namespace Parkway.CBS.Core.HTTP.Handlers.Contracts
{
    public interface ICoreInvoiceService : IDependency
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <param name="expertSystem"></param>
        /// <returns></returns>
        InvoiceGenerationResponse TryGenerateInvoice(CreateInvoiceUserInputModel model, ref List<ErrorModel> errors, ExpertSystemVM expertSystemVM, TaxEntityViewModel entityVM, UserPartRecord adminUser = null);


        /// <summary>
        /// try create invoice
        /// </summary>
        /// <param name="model"></param>
        /// <param name="tenant"></param>
        /// <param name="errors"></param>
        /// <returns>IntegrationResponseModel</returns>
        InvoiceGeneratedResponseExtn TryCreateInvoice(CreateInvoiceModel model, ref List<ErrorModel> errors, ExpertSystemSettings expertySystem = null, string requestRef = null);
        InvoiceGeneratedResponseExtn GetInvoiceReceiptsVM(string invoiceNumber);


        /// <summary>
        /// Try create invoice for paye assessment
        /// </summary>
        /// <param name="createInvoiceModelForPayeAssessment"></param>
        /// <param name="expertSystem"></param>
        InvoiceGeneratedResponseExtn TryCreateInvoice(CreateInvoiceModelForPayeAssessment createInvoiceModelForPayeAssessment, string requestRef, ExpertSystemSettings expertSystem);


        /// <summary>
        /// Get the URL link for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>string</returns>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        string GetInvoiceURL(string invoiceNumber);


        /// <summary>
        /// Get the invoice details, along with the transactions that have been done on the invoice
        /// <para>This method does not in any way group transaction for invoice items</para>
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        InvoiceDetails GetInvoiceTransactions(string invoiceNumber);


        /// <summary>
        /// Will get 
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        InvoiceGeneratedResponseExtn GetInvoiceDetailsForPaymentView(string invoiceNumber);


        /// <summary>
        /// Check if this request has been processed already
        /// </summary>
        /// <param name="requestRef"></param>
        InvoiceGeneratedResponseExtn CheckRequestReference(string requestRef, ExpertSystemSettings expertSystem);


        /// <summary>
        /// Get the invoice for this invoice type
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <param name="invoiceTypeId"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        InvoiceDetailsHelperModel GetInvoiceHelperDetailsByInvoiceType(InvoiceType invoiceType, long invoiceTypeId);


        InvoiceDetailsHelperModel GetInvoiceHelperDetailsByInvoiceType(long invoiceId);



        InvoiceDetailsHelperModel GetInvoiceHelperDetails(string custReference);


        /// <summary>
        /// This method checks if the mdaId or revenue head has any validation restriction
        /// on provied payment provider.
        /// <para>Check that the payment provider is constrainted from validating the mda or revenue head</para>
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="revenueHeadID"></param>
        /// <param name="paymentProviderId"></param>
        /// <returns>bool</returns>
        bool CheckForValidationConstraint(int mdaId, int revenueHeadID, int paymentProviderId);


        /// <summary>
        /// Get payment references for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        InvoiceDetails GetPaymentReferencesForInvoice(string invoiceNumber);

        /// <summary>
        /// Get status of an invoice with all the payment transactions on it.
        /// This method does not in any way group transaction for invoice items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>InvoiceStatusDetailsVM</returns>
        InvoiceStatusDetailsVM GetInvoiceStatus(CollectionSearchParams searchParams);

        /// <summary>
        /// Invalidate an invoice
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>bool</returns>
        InvalidateInvoiceVM InvalidateInvoice(CollectionSearchParams searchParams);


        /// <summary>
        /// Check if payment provider has restrictions on this given MDA and revenue head
        /// if restrictions returns true, else false
        /// </summary>
        bool CheckForRestrictions(InvoiceGeneratedResponseExtn invoiceDetails, int paymentProviderId);

    }
}
