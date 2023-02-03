using Orchard.Logging;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Invoicing.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.Cashflow.Ng.Models;
using System.Linq;

namespace Parkway.CBS.Core.HTTP.Handlers.Invoicing
{
    public class OneOffAssessmentInvoiceGeneration : BaseInvoiceGeneration, IInvoiceGenerationType
    {
        public BillingType InvoiceGenerationType => BillingType.OneOff;


        public OneOffAssessmentInvoiceGeneration(IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(invoiceRepository, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            Logger = NullLogger.Instance;
        }


        public InvoiceGeneratedResponseExtn GenerateInvoice(GenerateInvoiceModel model, FileProcessModel fileUploadModel, bool showRemitta)
        {
            Logger.Information("Generation invoice for one off assessment. Getting next billing date");
            //now we are going to check whether this bill has been generated for the tax payer before
            InvoiceGeneratedResponseExtn result = GetAlreadyGeneratedInvoice(model);
            if (result != null)
            {
                result.Message = Lang.Lang.existinginvoicefoundforoneoffpayment().ToString();
                result.ShowRemitta = showRemitta;
                result.IsDuplicateRequestReference = true;
                return result;
            }

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

        
        

        private CreateInvoiceHelper GetBillingInvoiceItems(GenerateInvoiceModel invModel, string additionalDetails)
        {
            Logger.Information("Getting invoice parameters");
            CreateInvoiceHelper model = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            Logger.Information("getting invoice amount");
            model.Amount = invModel.Billing.Amount;
            //model.Amount = invModel.Amount; //Amount with Surcharge inclusive
            Logger.Information("Getting discounts");
            //get discount if applicable
            model.DiscountModel = GetApplicableDiscount(invModel.Billing, invModel.InvoiceDate);
            //get due date
            Logger.Information("Getting due date");
            model.DueDate = GetDueDate(invModel.Billing, invModel.InvoiceDate);
            //get title
            model.Title =  string.IsNullOrEmpty(invModel.InvoiceTitle) ? (invModel.Mda.NameAndCode() + " : " +  invModel.RevenueHead.NameAndCode()) : invModel.InvoiceTitle;
            //get type
            model.Type = "Single";
            //get items
            Logger.Information("setting invoice items");

            string description = string.Empty;
            if (string.IsNullOrEmpty(model.InvoiceDescription))
            {
                description = ("One Off Payment: " + invModel.RevenueHead.NameAndCode() + " " + additionalDetails + " | " + model.InvoiceDescription).TrimEnd('|');
            }
            else
            { description = model.InvoiceDescription; }

            model.Items = new List<CashFlowCreateInvoice.CashFlowProductModel>
            {
                {
                    new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = invModel.Billing.Amount + invModel.Surcharge,
                        ProductId = invModel.RevenueHead.CashFlowProductId,
                        ProductName = description,
                        Qty = invModel.Quantity == 0 ? 1 : invModel.Quantity
                    }
                }
            };
            //get footnotes for discounts and penalties that are to be applied
            Logger.Information("Getting footnotes");
            model.FootNotes = GetFootNotes(invModel.Billing);
            //invoice date
            model.InvoiceDate = invModel.InvoiceDate;

            model.InvoiceDescription = description;
            model.VAT = invModel.VAT;
            return model;
        }


    }
}