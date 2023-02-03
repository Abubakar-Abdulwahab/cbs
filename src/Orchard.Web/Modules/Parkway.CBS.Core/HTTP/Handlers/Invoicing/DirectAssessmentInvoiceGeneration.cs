using Parkway.CBS.Core.HTTP.Handlers.Invoicing.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Events.Contracts;
using Orchard.Users.Models;
using Newtonsoft.Json;
using Parkway.CBS.Core.Exceptions;

namespace Parkway.CBS.Core.HTTP.Handlers.Invoicing
{
    public class DirectAssessmentInvoiceGeneration : BaseInvoiceGeneration, IInvoiceGenerationType
    {
        public BillingType InvoiceGenerationType => BillingType.DirectAssessment;

        private readonly IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> _directAssessmentRecordRepository;
        private readonly IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> _directAssessmentPayeeRepository;

        public DirectAssessmentInvoiceGeneration(IDirectAssessmentBatchRecordManager<DirectAssessmentBatchRecord> directAssessmentRecordRepository, IDirectAssessmentPayeeManager<DirectAssessmentPayeeRecord> directAssessmentPayeeRepository, IInvoiceManager<Invoice> invoiceRepository, IRevenueHeadStatisticsEventHandler statsEventHandler, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo) : base(invoiceRepository, invoicingService, taxEntityAccountReposirty, transactionLogRepository, apiRequestRepository, invoiceItemsRepository, formValueRepo)
        {
            _directAssessmentRecordRepository = directAssessmentRecordRepository;
            Logger = NullLogger.Instance;
            _directAssessmentPayeeRepository = directAssessmentPayeeRepository;
        }


        public InvoiceGeneratedResponseExtn GenerateInvoice(GenerateInvoiceModel model, FileProcessModel fileUploadModel, bool showRemitta)
        {
            Logger.Information("Generation invoice for direct assessment");
            Logger.Information("Getting next billing date");
            DateTime nextBillingDate = DateTime.Now.ToLocalTime();
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

            CashFlowCreateCustomerAndInvoice cashFlowRequestModel = GetCashFlowRequestModel(model.StateSettings.StateId, model.Entity, invoiceHelper);

            CashFlowCreateCustomerAndInvoiceResponse response = CreateInvoiceOnCashflow(model.Mda.SMEKey, cashFlowRequestModel);
            Invoice invoice = SaveInvoice(response, invoiceHelper, model, InvoiceType.DirectAssessment, directAssessmentRecord.Id);
            model.Entity.CashflowCustomerId = response.CustomerModel.CustomerId;
            model.Entity.PrimaryContactId = response.CustomerModel.PrimaryContactId;
            //Save the items
            List<InvoiceItems> invoiceItems = SaveInvoiceItems(invoiceHelper, invoice);
            SaveFormData(invoice, invoiceItems.ElementAt(0), model.FormValues);
            List<TransactionLog> tranlogs = SaveTransactionLog(invoice, model, invoiceItems);
            //if api request reference is available
            if (!string.IsNullOrEmpty(model.RequestReference))
            { invoice.APIRequest = SaveAPIRequestReference(model.ExpertSystem, invoice.Id, model.RequestReference); }

            directAssessmentRecord.Invoice = invoice;
            directAssessmentRecord.InvoiceConfirmed = true;
            directAssessmentRecord.InvoiceItem = invoiceItems.ElementAt(0);
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
        private CreateInvoiceHelper GetBillingInvoiceItems(GenerateInvoiceModel helperModel, DateTime nextBillingDate, string additionalDetails, DirectAssessmentBatchRecord directAssessmentRecord)
        {
            Logger.Information("Getting invoice parameters");
            CreateInvoiceHelper model = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            //model.Amount = directAssessmentRecord.Amount;
            model.Amount = helperModel.Amount; //Amount with surcharge inclusive
            //get discount if applicable
            model.DiscountModel = GetApplicableDiscount(helperModel.Billing, helperModel.InvoiceDate);
            //get due date
            model.DueDate = GetDueDate(helperModel.Billing, helperModel.InvoiceDate);
            //invoice description
            model.InvoiceDescription = "Paye Assessment for schedule " + directAssessmentRecord.BatchRef;
            //get title
            model.Title = string.IsNullOrEmpty(helperModel.InvoiceTitle) ? (helperModel.Mda.NameAndCode() + " : " + helperModel.RevenueHead.NameAndCode()) : helperModel.InvoiceTitle;
            //get type
            model.Type = "Single";
            //get items
            model.Items = GetListOfValidPayees(directAssessmentRecord, helperModel, model.InvoiceDescription);
            //get footnotes for discounts and penalties that are to be applied
            model.FootNotes = GetFootNotes(helperModel.Billing);
            //invoice date
            model.InvoiceDate = helperModel.InvoiceDate;
            //get additional info
            model.ExternalRef = helperModel.ExternalRefNumber;

            model.VAT = helperModel.VAT;
            return model;
        }        

        private List<CashFlowCreateInvoice.CashFlowProductModel> GetListOfValidPayees(DirectAssessmentBatchRecord directAssessmentRecord, GenerateInvoiceModel helperModel, string description)
        {
            return new List<CashFlowCreateInvoice.CashFlowProductModel>
            {
                { new CashFlowCreateInvoice.CashFlowProductModel
                    {
                        Pos = 1,
                        Price = directAssessmentRecord.Amount + helperModel.Surcharge,
                        ProductId = helperModel.RevenueHead.CashFlowProductId,
                        ProductName = description,
                        Qty = helperModel.Quantity == 0 ? 1 : helperModel.Quantity,
                    }
                }
            };
        }

        /// <summary>
        /// create invoice details
        /// </summary>
        /// <param name="invoiceHelper"></param>
        /// <param name="model"></param>
        /// <param name="resultfromCF"></param>
        /// <returns>Invoice</returns>
        /// <exception cref="CouldNotSaveInvoiceOnCentralBillingException"></exception>
        public override Invoice SaveInvoice(CreateInvoiceHelper invoiceHelper, CreateInvoiceUserInputModel model, CashFlowCreateCustomerAndInvoiceResponse resultfromCF, ExpertSystemVM expertSettingsVM, TaxEntity entity, GenerateInvoiceRequestModel revenueHeadDetails, UserPartRecord adminUser = null)
        {
            string smodel = JsonConvert.SerializeObject(invoiceHelper);
            Logger.Information(string.Format("saving invoice model: {0}, invoice number {1}", smodel, resultfromCF.Invoice.Number));

            Invoice invoice = new Invoice
            {
                Amount = resultfromCF.Invoice.AmountDue,
                CashflowInvoiceIdentifier = invoiceHelper.UniqueInvoiceIdentifier,
                CallBackURL = model.CallBackURL,
                DueDate = invoiceHelper.DueDate,
                ExpertSystemSettings = new ExpertSystemSettings { Id = expertSettingsVM.Id },
                InvoiceDescription = invoiceHelper.InvoiceDescription,
                InvoiceNumber = resultfromCF.Invoice.Number,
                InvoiceType = (int)InvoiceType.DirectAssessment,
                ExternalRefNumber = model.ExternalRefNumber,
                InvoiceURL = resultfromCF.Invoice.IntegrationPreviewUrl,
                InvoiceTitle = resultfromCF.Invoice.Title,
                TaxPayer = entity,
                Status = (int)InvoiceStatus.Unpaid,
                InvoiceModel = smodel,
                GeneratedByAdminUser = adminUser,
                Mda = new MDA { Id = revenueHeadDetails.MDAVM.Id },
                RevenueHead = new RevenueHead { Id = revenueHeadDetails.RevenueHeadVM.Id },
                TaxPayerCategory = entity.TaxEntityCategory,
                Quantity = invoiceHelper.Items.Count
            };

            if (!_invoiceRepository.Save(invoice))
            {
                throw new CouldNotSaveInvoiceOnCentralBillingException(string.Format("Could not save invoice cf number {0}, model {1}", resultfromCF.Invoice.Number, smodel));
            }
            return invoice;
        }

    }
}