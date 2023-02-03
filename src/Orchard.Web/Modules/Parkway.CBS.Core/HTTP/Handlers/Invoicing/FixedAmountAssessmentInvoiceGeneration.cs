using Orchard.Logging;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Invoicing.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Parkway.CBS.Core.HTTP.Handlers.Invoicing
{
    public class FixedAmountAssessmentInvoiceGeneration : BaseInvoiceGeneration, IInvoiceGenerationType
    {
        public BillingType InvoiceGenerationType => BillingType.FixedAmountAssessment;

        public FixedAmountAssessmentInvoiceGeneration(IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(invoiceRepository, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            Logger = NullLogger.Instance;
            //_formValueRepo = formValueRepo;
        }


        public InvoiceGeneratedResponseExtn GenerateInvoice(GenerateInvoiceModel model, FileProcessModel fileUploadModel, bool showRemitta)
        {
            Logger.Information("Generation invoice for Fixed Amount assessment");
            //if the invoice is null, that means this bill has never been generated for this tax payer for the external ref if applicable. 
            Logger.Information("Ordering additional details");
            AdditionalDetailsHelperModel additionalDetails = GetAdditionalDetails(model.AddtionalDetails);

            Logger.Information("Getting invoice helper");
            CreateInvoiceHelper invoiceHelper = GetBillingInvoiceItems(model, additionalDetails.DetailsConcat);

            //get model we are sending to cashflow
            CashFlowCreateCustomerAndInvoice cashFlowRequestModel = GetCashFlowRequestModel(model.StateSettings.StateId, model.Entity, invoiceHelper);

            CashFlowCreateCustomerAndInvoiceResponse response = CreateInvoiceOnCashflow(model.Mda.SMEKey, cashFlowRequestModel);

            Invoice invoice = SaveInvoice(response, invoiceHelper, model, InvoiceType.Standard);
            model.Entity.CashflowCustomerId = response.CustomerModel.CustomerId;
            model.Entity.PrimaryContactId = response.CustomerModel.PrimaryContactId;
            //Save the items
            var invoiceItems = SaveInvoiceItems(invoiceHelper, invoice);
            SaveFormData(invoice, invoiceItems.ElementAt(0), model.FormValues);
            List<TransactionLog> tranlogs = SaveTransactionLog(invoice, model, invoiceItems);
            //if api request reference is available
            if (!string.IsNullOrEmpty(model.RequestReference))
            { invoice.APIRequest = SaveAPIRequestReference(model.ExpertSystem, invoice.Id, model.RequestReference); }

            return ReturnInvoiceGenerationResponse(response, model, showRemitta, invoice);
        }



        private CreateInvoiceHelper GetBillingInvoiceItems(GenerateInvoiceModel model, string additionalDetails)
        {
            Logger.Information("Getting invoice parameters");
            CreateInvoiceHelper helperModel = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            //helperModel.Amount = model.Billing.Amount;
            helperModel.Amount = model.Amount; //Amount with surcharge inclusive
            //get discount if applicable
            helperModel.DiscountModel = GetApplicableDiscount(model.Billing, model.InvoiceDate);
            //get due date
            helperModel.DueDate = GetDueDate(model.Billing, model.InvoiceDate);
            //get title
            helperModel.Title = string.IsNullOrEmpty(model.InvoiceTitle) ? (model.Mda.NameAndCode() + " : " + model.RevenueHead.NameAndCode()) : model.InvoiceTitle;
            //get type
            helperModel.Type = "Single";
            //invoice description
            string description = string.IsNullOrEmpty(model.InvoiceDescription) ? model.RevenueHead.NameAndCode() + " " + additionalDetails : model.InvoiceDescription;
            //get items
            helperModel.Items = new List<CashFlowCreateInvoice.CashFlowProductModel>
            {
                {
                    new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = model.Billing.Amount + model.Surcharge,
                        ProductId = model.RevenueHead.CashFlowProductId,
                        ProductName = description,
                        Qty = model.Quantity == 0 ? 1 : model.Quantity
                    }
                }
            };
            //get footnotes for discounts and penalties that are to be applied
            Logger.Information("Getting footnotes");
            helperModel.FootNotes = GetFootNotes(model.Billing);
            //invoice date
            helperModel.InvoiceDate = model.InvoiceDate;
            helperModel.InvoiceDescription = description;
            helperModel.VAT = model.VAT;
            return helperModel;
        }


    }
}