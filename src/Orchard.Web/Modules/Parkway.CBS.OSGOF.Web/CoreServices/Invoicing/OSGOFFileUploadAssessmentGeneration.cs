using Orchard.Logging;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Invoicing;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.OSGOF.Admin.Models;
using Parkway.CBS.OSGOF.Admin.Services.Contracts;
using Parkway.CBS.OSGOF.Web.CoreServices.Invoicing.Contracts;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.OSGOF.Web.CoreServices.Invoicing
{
    public class OSGOFFileUploadAssessmentGeneration : BaseInvoiceGeneration, IOSGOFInvoiceGenerationType
    {
        private readonly ICellSiteClientPaymentBatchManager<CellSiteClientPaymentBatch> _osgofBatchRepository;
        private readonly ICellSitesPaymentManager<CellSitesPayment> _cellSitesPaymentRepo;


        public OSGOFFileUploadAssessmentGeneration(IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, ICellSiteClientPaymentBatchManager<CellSiteClientPaymentBatch> osgofBatchRepository, ICellSitesPaymentManager<CellSitesPayment> cellSitesPaymentRepo, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(invoiceRepository, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            Logger = NullLogger.Instance;
            _osgofBatchRepository = osgofBatchRepository;
            _cellSitesPaymentRepo = cellSitesPaymentRepo;
        }

        public BillingType InvoiceGenerationType => BillingType.FileUpload;



        public InvoiceGeneratedResponseExtn GenerateInvoice(GenerateInvoiceModel model, FileProcessModel fileUploadModel, bool showRemitta)
        {
            Logger.Information("Generation invoice for cell sites assessment");
            Logger.Information("Getting next billing date");
            DateTime nextBillingDate = DateTime.Now.ToLocalTime();
            //let check if this tax payer is requesting for an invoice with an external ref number that already has an invoice attached to it
            Logger.Information("Ordering additional details");
            AdditionalDetailsHelperModel additionalDetails = GetAdditionalDetails(model.AddtionalDetails);
            //
            CellSiteClientPaymentBatch record = null;
            if (fileUploadModel.FileBatchRecord == null)
            {
                record = _osgofBatchRepository.GetRecord(fileUploadModel.Id);
            }
            else { record = fileUploadModel.FileBatchRecord; }

            record.InvoiceConfirmed = true;
            var amount = _cellSitesPaymentRepo.GetTotalAmountForSchedule(record);
            Logger.Information("Getting invoice helper");
            var invoiceHelper = GetBillingInvoiceItems(model, nextBillingDate, "", record, amount);
            //
            //get model we are sending to cashflow
            CashFlowCreateCustomerAndInvoice cashFlowRequestModel = GetCashFlowRequestModel(model.StateSettings.StateId, model.Entity, invoiceHelper);

            CashFlowCreateCustomerAndInvoiceResponse response = CreateInvoiceOnCashflow(model.Mda.SMEKey, cashFlowRequestModel);

            Invoice invoice = SaveInvoice(response, invoiceHelper, model, InvoiceType.OSGOF, record.Id);
            model.Entity.CashflowCustomerId = response.CustomerModel.CustomerId;
            model.Entity.PrimaryContactId = response.CustomerModel.PrimaryContactId;

            record.Invoice = invoice;
            //Save the items
            var invoiceItems = SaveInvoiceItems(invoiceHelper, invoice);
            List<TransactionLog> tranlogs = SaveTransactionLog(invoice, model, invoiceItems);
            //if api request reference is available
            if (!string.IsNullOrEmpty(model.RequestReference))
            { invoice.APIRequest = SaveAPIRequestReference(model.ExpertSystem, invoice.Id, model.RequestReference); }

            return ReturnInvoiceGenerationResponse(response, model, showRemitta, invoice);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestModelList"></param>
        /// <param name="dBModel"></param>
        /// <returns></returns>
        public RevenueHeadRequestValidationObject ValidateRequestModel(List<GenerateInvoiceRequestModel> requestModelList)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get list if helper fields
        /// </summary>
        /// <param name="helperModel"></param>
        /// <param name="nextBillingDate"></param>
        /// <param name="additionalDetails"></param>
        /// <param name="fileUploadModel"></param>
        /// <returns></returns>
        protected CreateInvoiceHelper GetBillingInvoiceItems(GenerateInvoiceModel helperModel, DateTime nextBillingDate, string additionalDetails, CellSiteClientPaymentBatch record, decimal amount)
        {
            Logger.Information("Getting invoice parameters");
            CreateInvoiceHelper model = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            Logger.Information("getting invoice amount");

            model.Amount = amount;
            Logger.Information("Getting discounts");
            //get discount if applicable
            model.DiscountModel = GetApplicableDiscount(helperModel.Billing, helperModel.InvoiceDate);
            //get due date
            Logger.Information("Getting due date");
            model.DueDate = GetDueDate(helperModel.Billing, helperModel.InvoiceDate);
            //get title
            model.Title = helperModel.RevenueHead.NameAndCode();
            //get type
            model.Type = "Single";
            //get items
            Logger.Information("setting invoice items");
            model.Items = GetListOfValidCellSites(amount, record.BatchRef, helperModel.RevenueHead.CashFlowProductId, helperModel.Quantity);
            //get footnotes for discounts and penalties that are to be applied
            Logger.Information("Getting footnotes");
            model.FootNotes = GetFootNotes(helperModel.Billing);
            //invoice date
            model.InvoiceDate = helperModel.InvoiceDate;
            //get additional info
            model.ExternalRef = helperModel.ExternalRefNumber;
            model.VAT = helperModel.VAT;
            return model;
        }
       

        protected List<CashFlowCreateInvoice.CashFlowProductModel> GetListOfValidCellSites(decimal amount, string batchRef, long productId, int quantity)
        {
            return new List<CashFlowCreateInvoice.CashFlowProductModel>
            {
                { new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = amount,
                        ProductId = productId,
                        ProductName = "Cell Sites Assessment for schedule " + batchRef,
                        Qty = quantity == 0 ? 1 : quantity,
                    }
                }
            };
        }


    }
}