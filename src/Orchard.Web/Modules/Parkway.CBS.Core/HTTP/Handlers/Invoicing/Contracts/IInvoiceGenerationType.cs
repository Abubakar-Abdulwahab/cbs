using System;
using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.Cashflow.Ng.Models;
using Orchard.Users.Models;

namespace Parkway.CBS.Core.HTTP.Handlers.Invoicing.Contracts
{
    public interface IInvoiceGenerationType : IDependency
    {
        /// <summary>
        /// Invoice type
        /// </summary>
        BillingType InvoiceGenerationType { get; }


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expertSystem"></param>
        /// <param name="stateSettings"></param>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="billing"></param>
        /// <param name="category"></param>
        /// <param name="amount"></param>
        /// <param name="addtionalDetails"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="fileUploadModel"></param>
        /// <returns></returns>
        InvoiceGeneratedResponseExtn GenerateInvoice(GenerateInvoiceModel model, FileProcessModel fileUploadModel, bool showRemitta);


        /// <summary>
        /// Get the helper model for invoice generation
        /// </summary>
        /// <param name="groupDetails">RevenueHeadForInvoiceGenerationHelper</param>
        /// <param name="model">CreateInvoiceUserInputModel</param>
        /// <param name="invoiceDate">DateTime</param>
        /// <returns>CreateInvoiceHelper</returns>
        CreateInvoiceHelper GetHelperModel(GenerateInvoiceRequestModel groupDetails, CreateInvoiceUserInputModel model, DateTime invoiceDate);


        CashFlowCreateCustomerAndInvoice GetCashFlowRequestModel(int stateId, TaxEntity entity, CreateInvoiceHelper invoiceHelper);


        /// <summary>
        /// Create invoice on cashflow
        /// </summary>
        /// <param name="smeKey"></param>
        /// <param name="cashFlowRequestModel"></param>
        /// <returns>CashFlowCreateCustomerAndInvoiceResponse</returns>
        /// <exception cref="CannotConnectToCashFlowException"></exception>
        CashFlowCreateCustomerAndInvoiceResponse CreateInvoiceOnCashflow(string smeKey, CashFlowCreateCustomerAndInvoice cashFlowRequestModel);


        /// <summary>
        /// Save invoice
        /// </summary>
        /// <param name="invoiceHelper"></param>
        /// <param name="model"></param>
        /// <param name="resultfromCF"></param>
        /// <param name="expertSettingsVM"></param>
        /// <param name="entity"></param>
        /// <param name="revenueHeadDetails"></param>
        /// <param name="adminUser"></param>
        /// <returns>Invoice</returns>
        Invoice SaveInvoice(CreateInvoiceHelper invoiceHelper, CreateInvoiceUserInputModel model, CashFlowCreateCustomerAndInvoiceResponse resultfromCF, ExpertSystemVM expertSettingsVM, TaxEntity entity, GenerateInvoiceRequestModel revenueHeadDetails, UserPartRecord adminUser = null);


        /// <summary>
        /// Save transactions
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="model"></param>
        /// <param name="items"></param>
        /// <returns></returns>
        List<TransactionLog> SaveTransactionLog(Invoice invoice, GenerateInvoiceModel model, List<InvoiceItems> items);

        /// <summary>
        /// Save API request reference
        /// </summary>
        /// <param name="expertSystem"></param>
        /// <param name="invoiceId"></param>
        /// <param name="apiRequestReference"></param>
        /// <returns></returns>
        APIRequest SaveAPIRequestReference(ExpertSystemSettings expertSystem, Int64 invoiceId, string apiRequestReference);


        /// <summary>
        /// Get return response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="model"></param>
        /// <param name="showRemitta"></param>
        /// <param name="invoice"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        InvoiceGeneratedResponseExtn ReturnInvoiceGenerationResponse(CashFlowCreateCustomerAndInvoiceResponse response, GenerateInvoiceModel model, bool showRemitta, Invoice invoice);
    }
}
