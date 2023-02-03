using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Invoicing.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HTTP.Handlers.Invoicing
{
    public class FixedAssessmentInvoiceGeneration : BaseInvoiceGeneration, IInvoiceGenerationType
    {

        public BillingType InvoiceGenerationType => BillingType.Fixed;

        public FixedAssessmentInvoiceGeneration(IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(invoiceRepository, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            Logger = NullLogger.Instance;
        }

        public InvoiceGeneratedResponseExtn GenerateInvoice(GenerateInvoiceModel model, FileProcessModel fileUploadModel, bool showRemitta)
        {
            Logger.Debug("Generate invoice for fixed assesment");
            DateTime nextBillingDate = model.Billing.NextBillingDate.Value;
           
            AdditionalDetailsHelperModel additionalDetails = GetAdditionalDetails(model.AddtionalDetails);
            //so based on the next billing date, we would have to check if this user already has a billing waiting for
            string value = string.Format("{0}{1}{2}{3}{4}", model.Entity.Id, nextBillingDate.Ticks.ToString(), additionalDetails.DetailsConcat, model.Billing.Id, model.Mda.SMEKey);

            var uniqueIdentifier = Utilities.Util.OnWayHashThis(value, Utilities.AppSettingsConfigurations.EncryptionSecret);
            //the unique identifier here is the value we use to ensure that this invoice has not been generated before
            //for example if tax payer 1, with the additional details of say "Year 2017" and within the fixed billing period of March 2 20X,
            //wants to generate an invoice, we need to generate a unique identifier for this request that is tax payer 1 for year 2017 and billing period. This value would be store against this tax payer along with the invoice
            //so if this same tax payer comes within the given billing period the same invoice would be given to the tax payer, because the next billing cycle has not come around
            //Succint example: TaxPayer 1, additional details Year 2017, billing period March 2 20x, would give a unique identifier of xyz,
            //if the same TaxPayer 1 comes with the same details as above within the same billing period, the same invoice would be issued to them

            //now lets check that this unique identifier has not been generated before
            InvoiceGeneratedResponseExtn invoiceResponse = _invoiceRepository.CheckInvoice(uniqueIdentifier, model.Entity.Id, model.RevenueHead, model.Category);
            if(invoiceResponse != null)
            {
                invoiceResponse.Message = Lang.Lang.existinginvoicefoundforoneoffpayment().ToString();
                invoiceResponse.ShowRemitta = showRemitta;
                return invoiceResponse;
            }

            //if no invoice found, lets generate one mate!
            Logger.Information("Getting invoice helper");
            CreateInvoiceHelper invoiceHelper = GetBillingInvoiceItems(model, additionalDetails.DetailsConcat, uniqueIdentifier);

            throw new NotImplementedException();
        }



        private CreateInvoiceHelper GetBillingInvoiceItems(GenerateInvoiceModel generateInvoiceModel, string detailsConcat, string invoiceIdentifier)
        {
            Logger.Information("Getting invoice parameters");
            CreateInvoiceHelper model = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            Logger.Information("getting invoice amount");
            if(generateInvoiceModel.Amount <= 0.00m)
            { model.Amount = generateInvoiceModel.Billing.Amount; }
            else
            { model.Amount = generateInvoiceModel.Amount; }

            Logger.Information("Getting discounts");
            //get discount if applicable
            model.DiscountModel = GetApplicableDiscount(generateInvoiceModel.Billing, generateInvoiceModel.InvoiceDate);
            //get due date
            Logger.Information("Getting due date");
            model.DueDate = GetDueDate(generateInvoiceModel.Billing, generateInvoiceModel.InvoiceDate);
            //get title
            model.Title = generateInvoiceModel.RevenueHead.NameAndCode();
            //get type
            model.Type = "Single";
            //get items
            Logger.Information("setting invoice items");

            string description = generateInvoiceModel.RevenueHead.NameAndCode() + (string.IsNullOrEmpty(generateInvoiceModel.InvoiceDescription) ? "" : " | " + generateInvoiceModel.InvoiceDescription);
            description += string.IsNullOrEmpty(detailsConcat) ? "" : " | " + detailsConcat;

            model.Items = new List<CashFlowCreateInvoice.CashFlowProductModel>
            {
                {
                    new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = model.Amount + generateInvoiceModel.Surcharge,
                        ProductId = generateInvoiceModel.RevenueHead.CashFlowProductId,
                        ProductName = description,
                        Qty = generateInvoiceModel.Quantity == 0 ? 1 : generateInvoiceModel.Quantity
                    }
                }
            };
            //get footnotes for discounts and penalties that are to be applied
            Logger.Information("Getting footnotes");
            model.FootNotes = GetFootNotes(generateInvoiceModel.Billing);
            //invoice date
            model.InvoiceDate = generateInvoiceModel.InvoiceDate;

            model.UniqueInvoiceIdentifier = invoiceIdentifier;
            model.InvoiceDescription = description;
            return model;
        }


        protected string GetInvoiceDetails(BillingModel billing)
        {
            BillingFrequencyModel billingFrequencyModel = JsonConvert.DeserializeObject<BillingFrequencyModel>(billing.BillingFrequency);
            //get start date
            DateTime startDate = billingFrequencyModel.FixedBill.StartDateAndTime;
            //
            throw new NotImplementedException();
        }


    }
}