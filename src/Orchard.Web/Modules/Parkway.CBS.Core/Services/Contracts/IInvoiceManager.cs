using Orchard;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System.Data;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IInvoiceManager<Invoice> : IDependency, IBaseManager<Invoice>
    {

        /// <summary>
        /// Get details pertaining to this invoice by invoiceType and resourceTypeId
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <param name="invoiceTypeId"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        InvoiceDetailsHelperModel GetInvoiceDetails(InvoiceType invoiceType, long invoiceTypeId);


        /// <summary>
        /// Get invoice Id for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        Int64 GetInvoiceId(string invoiceNumber);


        /// <summary>
        /// Get details pertaining to this invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        InvoiceDetailsHelperModel GetInvoiceDetails(string invoiceNumber);


        /// <summary>
        /// Get details pertaining to this invoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        InvoiceDetailsHelperModel GetInvoiceDetails(long invoiceId);


        /// <summary>
        /// Get invoice belonging to this entity and revenue head
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="revenueHead"></param>
        /// <returns>InvoiceGeneratedResponseExtn | null</returns>
        InvoiceGeneratedResponseExtn CheckInvoice(TaxEntity entity, RevenueHead revenueHead);


        /// <summary>
        /// Return the 
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="count"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        List<Invoice> GetRevenueCollectionPerMDA(MDA mda, DateTime startDate, DateTime endDate, int take, int skip);


        Dictionary<int, IEnumerable<Invoice>> GetRevenueInvoiceCollectionForRevenueHead(RevenueHead revenueHead, DateTime startDate, DateTime endDate, int count, int skip);


        MDAExpectationViewModel ExpectationPerMDA(MDA mda, DateTime startDate, DateTime endDate, int skip, int take, bool direction, string orderBy);


        MDAMonthlyPaymentViewModel GetMonthlyMDAsPayment(IEnumerable<MDA> strippedMDAs, DateTime startDate, DateTime endDate);


        MDAMonthlyPaymentPerRevenueViewModel GetMonthlyMDAsPaymentPerRevenueHead(IEnumerable<RevenueHead> strippedRevenHeads, DateTime endDate, DateTime startDate);


        /// <summary>
        /// Get details for an invoice by invoice id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        InvoiceGenerationResponse GetInvoiceByInvoiceIdForDuplicateRecordsX(long refResult);


        /// <summary>
        /// Get details for an invoice by invoice id
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        InvoiceGeneratedResponseExtn GetInvoiceByInvoiceIdForDuplicateRecords(long refResult);


        /// <summary>
        /// Check for invoice
        /// </summary>
        /// <param name="uniqueInvoiceIdentifier"></param>
        /// <param name="taxPayerIdentificationNumber"></param>
        /// <param name="revenueHead"></param>
        /// <param name="category"></param>
        /// <returns>InvoiceGeneratedResponseExtn | null</returns>
        InvoiceGeneratedResponseExtn CheckInvoice(string uniqueInvoiceIdentifier, long taxEntityId, RevenueHead revenueHead, TaxEntityCategory category);


        /// <summary>
        /// Get invoice URL
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>string</returns>
        string GetInvoiceURL(string invoiceNumber);


        /// <summary>
        /// Get the in5voice details for an invoice given the invoice number for make payment page
        /// <para>Will return null if invoice not found</para>
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGeneratedResponseExtn GetInvoiceDetailsForPaymentView(string invoiceNumber);


        bool SaveBundleBatchInvoiceResponse(DataTable listOfDataTables, string tableName);


        /// <summary>
        /// Get the receipts that belong to this invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGeneratedResponseExtn GetReceiptsBelongingToInvoiceNumber(string invoiceNumber);


        /// <summary>
        /// Get transaction for each individual invoice items for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        InvoiceDetails GetInvoiceTransactions(string invoiceNumber);


        /// <summary>
        /// Get the list of payment references and their providers for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        InvoiceDetails GetPaymentRefs(string invoiceNumber);

        /// <summary>
        /// Get status of an invoice with all the payment transactions on it.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceStatusDetailsVM</returns>
        Invoice GetInvoiceStatus(CollectionSearchParams searchParams);

        /// <summary>
        /// Confirm the existence of a paid development levy invoice using the invoice number and development levy revenue head
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="developmentLevyRevenueHeadId"></param>
        /// <returns>long</returns>
        long CheckDevelopmentLevyInvoice(string invoiceNumber, int developmentLevyRevenueHeadId);

        /// <summary>
        /// Returns InvoiceStatusVM of invoice if it exists
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceStatusVM</returns>
        InvoiceStatusVM CheckDevelopmentLevyInvoice(string invoiceNumber);

        /// <summary>
        /// Get invoice details using the specified invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>ValidateInvoiceVM</returns>
        ValidateInvoiceVM GetInvoiceInfo(string invoiceNumber);

        /// <summary>
        /// Checks if the invoice status is paid
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>boolean</returns>
        bool IsInvoicePaid(string invoiceNumber);
    }
}
