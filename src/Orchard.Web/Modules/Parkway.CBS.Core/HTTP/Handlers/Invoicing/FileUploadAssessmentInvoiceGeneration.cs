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
using System.Linq;

namespace Parkway.CBS.Core.HTTP.Handlers.Invoicing
{
    public class FileUploadAssessmentInvoiceGeneration : BaseInvoiceGeneration, IInvoiceGenerationType
    {
        public BillingType InvoiceGenerationType => BillingType.FileUpload;

        private readonly IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> _directAssessmentRecordRepository;
        //private readonly IAPIRequestManager<APIRequest> _apiRequestRepository;

        public FileUploadAssessmentInvoiceGeneration(IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> directAssessmentRecordRepository, IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(invoiceRepository, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            _directAssessmentRecordRepository = directAssessmentRecordRepository;
            Logger = NullLogger.Instance;
            //_formValueRepo = formValueRepo;
            //_apiRequestRepository = apiRequestRepository;
        }


        public virtual InvoiceGeneratedResponseExtn GenerateInvoice(GenerateInvoiceModel model, FileProcessModel fileUploadModel, bool showRemitta)
        {
            Logger.Information("Generation invoice for direct assessment");
            Logger.Information("Getting next billing date");
            DateTime nextBillingDate = DateTime.Now.ToLocalTime();
            //let check if this tax payer is requesting for an invoice with an external ref number that already has an invoice attached to it
            Logger.Information("Ordering additional details");
            AdditionalDetailsHelperModel additionalDetails = GetAdditionalDetails(model.AddtionalDetails);
            //
            DirectAssessmentBatchRecord directAssessmentRecord = null;
            if (fileUploadModel.DirectAssessmentBatchRecord == null)
            {
                directAssessmentRecord = _directAssessmentRecordRepository.GetCollection(x => x.Id == fileUploadModel.Id).FirstOrDefault();
            }
            else { directAssessmentRecord = fileUploadModel.DirectAssessmentBatchRecord; }

            Logger.Information("Getting invoice helper");
            var invoiceHelper = GetBillingInvoiceItems(model, nextBillingDate, "", directAssessmentRecord);
            //
            CashFlowCreateCustomerAndInvoice cashFlowRequestModel = GetCashFlowRequestModel(model.StateSettings.StateId, model.Entity, invoiceHelper);

            CashFlowCreateCustomerAndInvoiceResponse response = CreateInvoiceOnCashflow(model.Mda.SMEKey, cashFlowRequestModel);

            Invoice invoice = SaveInvoice(response, invoiceHelper, model, InvoiceType.DirectAssessment, directAssessmentRecord.Id);
            model.Entity.CashflowCustomerId = response.CustomerModel.CustomerId;
            model.Entity.PrimaryContactId = response.CustomerModel.PrimaryContactId;
            //Save the items
            var invoiceItems = SaveInvoiceItems(invoiceHelper, invoice);
            List<TransactionLog> tranlogs = SaveTransactionLog(invoice, model, invoiceItems);
            //if api request reference is available
            if (!string.IsNullOrEmpty(model.RequestReference))
            { invoice.APIRequest = SaveAPIRequestReference(model.ExpertSystem, invoice.Id, model.RequestReference); }

            directAssessmentRecord.Invoice = invoice;
            directAssessmentRecord.InvoiceConfirmed = true;

            return ReturnInvoiceGenerationResponse(response, model, showRemitta, invoice);
        }


        /// <summary>
        /// Get list if helper fields
        /// </summary>
        /// <param name="helperModel"></param>
        /// <param name="nextBillingDate"></param>
        /// <param name="additionalDetails"></param>
        /// <param name="fileUploadModel"></param>
        /// <returns></returns>
        protected virtual CreateInvoiceHelper GetBillingInvoiceItems(GenerateInvoiceModel helperModel, DateTime nextBillingDate, string additionalDetails, DirectAssessmentBatchRecord directAssessmentRecord)
        {
            Logger.Information("Getting invoice parameters");
            CreateInvoiceHelper model = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            model.Amount = directAssessmentRecord.Amount;
            //get discount if applicable
            model.DiscountModel = GetApplicableDiscount(helperModel.Billing, helperModel.InvoiceDate);
            //get due date
            model.DueDate = GetDueDate(helperModel.Billing, helperModel.InvoiceDate);
            //get title
            model.Title = string.IsNullOrEmpty(helperModel.InvoiceTitle) ? (helperModel.Mda.NameAndCode() + " : " + helperModel.RevenueHead.NameAndCode()) : helperModel.InvoiceTitle;
            //get type
            model.Type = "Single";
            //get items
            model.Items = GetListOfValidPayees(directAssessmentRecord, helperModel);
            //get footnotes for discounts and penalties that are to be applied
            model.FootNotes = GetFootNotes(helperModel.Billing);
            //invoice date
            model.InvoiceDate = helperModel.InvoiceDate;
            //get additional info
            model.ExternalRef = helperModel.ExternalRefNumber;
            model.VAT = helperModel.VAT;
            return model;
        }

        protected virtual List<CashFlowCreateInvoice.CashFlowProductModel> GetListOfValidPayees(DirectAssessmentBatchRecord directAssessmentRecord, GenerateInvoiceModel helperModel)
        {
            return new List<CashFlowCreateInvoice.CashFlowProductModel>
            {
                { new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = directAssessmentRecord.Amount + helperModel.Surcharge,
                        ProductId = helperModel.RevenueHead.CashFlowProductId,
                        ProductName = "Paye Assessment for schedule " + directAssessmentRecord.BatchRef,
                        Qty = helperModel.Quantity == 0 ? 1 : helperModel.Quantity,
                    }
                }
            };
        }

    }
}