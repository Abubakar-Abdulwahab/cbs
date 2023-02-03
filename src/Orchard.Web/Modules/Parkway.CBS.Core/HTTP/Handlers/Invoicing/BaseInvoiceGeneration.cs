using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Users.Models;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Parkway.CBS.Core.HTTP.Handlers.Invoicing
{
    public abstract class BaseInvoiceGeneration
    {
        public ILogger Logger { get; set; }

        protected readonly IInvoiceManager<Invoice> _invoiceRepository;

        protected IInvoicingService _invoicingService;
        protected readonly ITaxEntityAccountManager<TaxEntityAccount> _taxEntityAccountReposirty;
        protected readonly ITransactionLogManager<TransactionLog> _transactionLogRepository;
        protected readonly IAPIRequestManager<APIRequest> _apiRequestRepository;
        protected readonly IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> _formValueRepo;
        protected readonly IInvoiceItemsManager<InvoiceItems> _invoiceItemsRepository;


        public BaseInvoiceGeneration(IInvoiceManager<Invoice> invoiceRepository, IInvoicingService invoicingService, ITaxEntityAccountManager<TaxEntityAccount> taxEntityAccountReposirty, ITransactionLogManager<TransactionLog> transactionLogRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo)
        {
            Logger = NullLogger.Instance;
            _invoiceRepository = invoiceRepository;
            _invoicingService = invoicingService;
            _taxEntityAccountReposirty = taxEntityAccountReposirty;
            _transactionLogRepository = transactionLogRepository;
            _apiRequestRepository = apiRequestRepository;
            _invoiceItemsRepository = invoiceItemsRepository;
            _formValueRepo = formValueRepo;
        }


        protected AdditionalDetailsHelperModel GetAdditionalDetails(List<AdditionalDetails> addtionalDetails)
        {
            IEnumerable<AdditionalDetails> orderedDetails = new List<AdditionalDetails>();
            if (addtionalDetails != null && addtionalDetails.Count() > 0)
            {
                orderedDetails = addtionalDetails.OrderBy(x => x.ControlIdentifier);
            }

            string additionalInfoForInvoiceConcat = string.Join("; ", orderedDetails.Select(adifo => adifo.IdentifierName + ": " + adifo.IdentifierValue.Trim()).ToArray());

            var sListOfAdditionalDetails = orderedDetails.Select(adt => adt.ControlIdentifier.ToString() + adt.IdentifierValue.Trim());

            return new AdditionalDetailsHelperModel { DetailsAndControlIdConcat = sListOfAdditionalDetails, DetailsConcat = additionalInfoForInvoiceConcat };
        }


        protected InvoiceGeneratedResponseExtn GetAlreadyGeneratedInvoice(GenerateInvoiceModel model)
        { return _invoiceRepository.CheckInvoice(model.Entity, model.RevenueHead); }


        /// <summary>
        /// Get request model we are sending to cashflow for invoice generation
        /// </summary>
        /// <param name="stateId"></param>
        /// <param name="entity"></param>
        /// <param name="invoiceHelper"></param>
        /// <returns></returns>
        public virtual CashFlowCreateCustomerAndInvoice GetCashFlowRequestModel(int stateId, TaxEntity entity, CreateInvoiceHelper invoiceHelper)
        {
            return new CashFlowCreateCustomerAndInvoice
            {
                CreateCustomer = Utilities.InvoiceUtil.CreateCashflowCustomer(stateId, entity),
                CreateInvoice = Utilities.InvoiceUtil.CreateCashflowCustomerInvoice(invoiceHelper),
                InvoiceUniqueKey = invoiceHelper.UniqueInvoiceIdentifier,
                PropertyTitle = "CentralBillingSystem"
            };
        }


        /// <summary>
        /// Save transaction log for invoice generation
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="invoice"></param>
        /// <param name="model"></param>
        /// <returns>List<TransactionLog></returns>
        public virtual List<TransactionLog> SaveTransactionLog(Invoice invoice, GenerateInvoiceModel model, List<InvoiceItems> items)
        {
            List<TransactionLog> tranItems = new List<TransactionLog> { };
            foreach (var item in items)
            {
                tranItems.Add(new TransactionLog
                {
                    PaymentReference = string.Format("{0}", invoice.InvoiceNumber),
                    AmountPaid = Math.Round(item.Quantity * item.UnitAmount, 2),
                    Invoice = invoice,
                    PaymentDate = invoice.CreatedAtUtc,
                    Status = PaymentStatus.Pending,
                    TaxEntity = model.Entity,
                    TypeID = (int)PaymentType.Bill,
                    RevenueHead = item.RevenueHead,
                    MDA = item.Mda,
                    TaxEntityCategory = item.TaxEntityCategory,
                    AdminUser = model.AdminUser,
                    UpdatedByAdmin = model.AdminUser == null ? false : true,
                    InvoiceNumber = invoice.InvoiceNumber,
                    InvoiceItem = item,
                });
            }

            if (!_transactionLogRepository.SaveBundleUnCommit(tranItems))
            {
                Logger.Error("Cannot save transaction log");
                throw new CannotSaveTaxEntityException();
            }
            return tranItems;
        }


        /// <summary>
        /// Get result for invoice generation
        /// </summary>
        /// <param name="response"></param>
        /// <param name="model"></param>
        /// <param name="showRemitta"></param>
        /// <param name="invoice"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public virtual InvoiceGeneratedResponseExtn ReturnInvoiceGenerationResponse(CashFlowCreateCustomerAndInvoiceResponse response, GenerateInvoiceModel model, bool showRemitta, Invoice invoice)
        {
            return new InvoiceGeneratedResponseExtn
            {
                InvoiceNumber = response.Invoice.Number,
                AmountDue = response.Invoice.AmountDue,
                InvoicePreviewUrl = response.Invoice.IntegrationPreviewUrl,
                CustomerId = response.CustomerModel.CustomerId,
                CustomerPrimaryContactId = response.CustomerModel.PrimaryContactId,
                TIN = model.Entity.TaxPayerIdentificationNumber,
                Recipient = model.Entity.Recipient,
                Email = model.Entity.Email,
                PhoneNumber = model.Entity.PhoneNumber,
                MDAName = model.Mda.Name,
                RevenueHeadName = model.RevenueHead.Name,
                ShowRemitta = showRemitta,
                ExternalRefNumber = model.ExternalRefNumber,
                Description = invoice.InvoiceDescription,
                PayerId = model.Entity.PayerId,
                InvoiceId = invoice.Id,
                MDAId = model.Mda.Id,
                RevenueHeadID = model.Mda.Id
            };
        }



        public virtual APIRequest SaveAPIRequestReference(ExpertSystemSettings expertSystem, Int64 invoiceId, string apiRequestReference)
        {
            APIRequest request = new APIRequest { CallType = (int)CallTypeEnum.Invoice, ExpertSystemSettings = expertSystem, ResourceIdentifier = invoiceId, RequestIdentifier = apiRequestReference };
            if (!_apiRequestRepository.Save(request)) { throw new Exception("Could not save API request ref " + apiRequestReference); }
            return request;
        }


        public virtual CashFlowCreateCustomerAndInvoiceResponse CreateInvoiceOnCashflow(string smeKey, CashFlowCreateCustomerAndInvoice cashFlowRequestModel)
        {
            try
            {
                Logger.Information("Calling cashflow");
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", smeKey } });
                var invoiceService = _invoicingService.InvoiceService(context);
                IntegrationResponseModel response = invoiceService.CreateCustomerAndInvoice(cashFlowRequestModel);
                if (response.HasErrors)
                {
                    Logger.Error("Error on invoice request on cashflow ");
                    string errors = response.ResponseObject;
                    string message = string.Format("ErrorCode : {0} Error: {1}", response.ErrorCode, errors);
                    Logger.Error(message);
                    throw new CannotConnectToCashFlowException(message);
                }
                var objjson = JsonConvert.SerializeObject(response.ResponseObject);
                return JsonConvert.DeserializeObject<CashFlowCreateCustomerAndInvoiceResponse>(objjson);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw new CannotConnectToCashFlowException(exception.Message + exception.StackTrace);
            }
        }


        #region Billing utils

        /// <summary>
        /// Return the discount to be applied to a billing
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="invoiceGenerationDate"></param>
        /// <returns>DiscountModel | null is no applicable discount is found</returns>
        public DiscountModel GetApplicableDiscount(BillingModel billing, DateTime invoiceGenerationDate)
        {
            Logger.Information("Getting applicable discounts");
            if (billing.Discounts == null) { Logger.Information("No discount found"); return null; }

            DateTime discountPeriod = DateTime.Now.ToLocalTime();
            List<DiscountModel> discounts = JsonConvert.DeserializeObject<List<DiscountModel>>(billing.Discounts);
            Dictionary<int, DateTime> discountsAndTime = new Dictionary<int, DateTime>();
            int counter = -1;

            foreach (var item in discounts)
            {
                counter++;
                if (item.EffectiveFromType == EffectiveFromType.Days) { discountsAndTime.Add(counter, discountPeriod.AddDays(item.EffectiveFrom)); continue; }
                if (item.EffectiveFromType == EffectiveFromType.Weeks) { discountsAndTime.Add(counter, discountPeriod.AddDays(7 * item.EffectiveFrom)); continue; }
                if (item.EffectiveFromType == EffectiveFromType.Months) { discountsAndTime.Add(counter, discountPeriod.AddMonths(item.EffectiveFrom)); continue; }
                if (item.EffectiveFromType == EffectiveFromType.Years) { discountsAndTime.Add(counter, discountPeriod.AddYears(item.EffectiveFrom)); continue; }
            }

            discountsAndTime.OrderBy(order => order.Value);
            var discountTime = discountsAndTime.Where(dsct => invoiceGenerationDate <= dsct.Value).FirstOrDefault();
            if (discountsAndTime == null) { Logger.Information("No discount range found " + invoiceGenerationDate.ToString()); return null; }
            var discountItem = discounts.ElementAt(discountTime.Key);
            //calculate discount value
            if (discountItem.BillingDiscountType == BillingDiscountType.Flat) { discountItem.DiscountValue = discountItem.Discount; }
            else if (discountItem.BillingDiscountType == BillingDiscountType.Percent) { discountItem.DiscountValue = (discountItem.Discount / 100); }
            return new DiscountModel { Discount = discountItem.Discount, BillingDiscountType = discountItem.BillingDiscountType, DiscountValue = discountItem.DiscountValue };
        }


        /// <summary>
        /// Get footnotes for the invoice. Footnotes are the full list of discount, penalties and invoice terms as they apply
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>string</returns>
        public string GetFootNotes(BillingModel billing)
        {
            string discountConcat = ""; string penaltyConcat = "";
            if (!string.IsNullOrEmpty(billing.Discounts))
            {
                List<DiscountModel> discounts = JsonConvert.DeserializeObject<List<DiscountModel>>(billing.Discounts);
                if (discounts.Any())
                {
                    discountConcat = "Discounts:\r\n";
                    foreach (var item in discounts)
                    {
                        var rate = item.BillingDiscountType == BillingDiscountType.Flat ? "Naira flat rate" : "% percent";
                        discountConcat += string.Format("\u2022 {0} {1} discount is applicable {2} {3} after invoice generation \r\n", item.Discount, rate, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }

            if (!string.IsNullOrEmpty(billing.Penalties))
            {
                List<PenaltyModel> penalties = JsonConvert.DeserializeObject<List<PenaltyModel>>(billing.Penalties);
                if (penalties.Any())
                {
                    penaltyConcat = "Penalties:\r\n";
                    foreach (var item in penalties)
                    {
                        var rate = item.PenaltyValueType == PenaltyValueType.FlatRate ? "Naira flat rate" : "% percent";
                        penaltyConcat += string.Format("\u2022 penalty is applicable {2} {3} after due date \r\n", item.Value, item.PenaltyValueType, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }
            return discountConcat + penaltyConcat;
        }


        protected CashFlowCreateCustomer CreateCashflowCustomer(TenantCBSSettings stateSettings, TaxEntity entity)
        {
            Logger.Information("Creating customer model for cashflow");
            return new CashFlowCreateCustomer
            {
                Address = entity.Address,
                CountryID = stateSettings.CountryId,
                CustomerId = entity.CashflowCustomerId,
                Identifier = entity.Id.ToString(),
                Name = entity.Recipient,
                StateID = stateSettings.StateId,
                Type = entity.TaxEntityCategory.Identifier == 0 ? Cashflow.Ng.Models.Enums.CashFlowCustomerType.Individual : Cashflow.Ng.Models.Enums.CashFlowCustomerType.Business,
                PryContact = new CashFlowCreateCustomer.Contact
                {
                    Name = entity.Recipient,
                    Email = entity.Email,
                }
            };
        }


        protected CashFlowCreateInvoice CreateCashflowCustomerInvoice(CreateInvoiceHelper invoiceHelper, string detailsConcat)
        {
            return new CashFlowCreateInvoice
            {
                Discount = 0m,
                DiscountType = "",
                //Discount = invoiceHelper.DiscountModel != null ? invoiceHelper.DiscountModel.Discount : 0m,
                //DiscountType = invoiceHelper.DiscountModel != null ? invoiceHelper.DiscountModel.BillingDiscountType.ToString() : "",
                DueDate = invoiceHelper.DueDate,
                FootNote = invoiceHelper.FootNotes,
                InvoiceDate = invoiceHelper.InvoiceDate,
                Items = invoiceHelper.Items,
                Title = invoiceHelper.Title,
                Type = invoiceHelper.Type,
            };
        }


        /// <summary>
        /// Get due date for the invoice
        /// </summary>
        /// <param name="billing">Billing model</param>
        /// <param name="invoiceDate">this is the invoice date</param>
        /// <param name="nextBillingDate">this is the next billing date. If the invoice is due on the next billing date, this value is returned</param>
        /// <returns>Datetime</returns>
        public DateTime GetDueDate(BillingModel billing, DateTime invoiceDate)
        {
            Logger.Information("Getting due date");
            if (string.IsNullOrEmpty(billing.DueDate)) { Logger.Error("Null due date found"); throw new NoDueDateTypeFoundException("No due date found for billing " + billing.Id); }

            DueDateModel dueDate = JsonConvert.DeserializeObject<DueDateModel>(billing.DueDate);
            if (dueDate == null) { Logger.Error("Null due date found"); throw new NoDueDateTypeFoundException("No due date found for billing. Due date is null for billing Id " + billing.Id); }

            //if the due date is due on the next billing date
            if (dueDate.DueOnNextBillingDate) { return billing.NextBillingDate.Value; }

            switch (dueDate.DueDateAfter)
            {
                case DueDateAfter.Days:
                    return invoiceDate.AddDays(dueDate.DueDateInterval);
                case DueDateAfter.Weeks:
                    return invoiceDate.AddDays(7 * dueDate.DueDateInterval);
                case DueDateAfter.Months:
                    return invoiceDate.AddMonths(dueDate.DueDateInterval);
                case DueDateAfter.Years:
                    return invoiceDate.AddYears(dueDate.DueDateInterval);
                default:
                    throw new NoDueDateTypeFoundException("No due date found for billing ");
            }
        }


        /// <summary>
        /// save invoice details, invoice number , amount and amount due are not persisted here
        /// after inovoice generation on the invoicing service, the listed fields are to be updated
        /// </summary>
        /// <param name="invoiceHelper"></param>
        /// <param name="model"></param>
        /// <param name="payeAssessment"></param>
        /// <returns>Invoice</returns>
        protected virtual Invoice SaveInvoice(CashFlowCreateCustomerAndInvoiceResponse cashflowResponse, CreateInvoiceHelper invoiceHelper, GenerateInvoiceModel model, InvoiceType invoiceType, Int64 typeId = 0)
        {
            //decimal discount = 0;
            //if (invoiceHelper.DiscountModel != null)
            //{
            //    if (invoiceHelper.DiscountModel.BillingDiscountType == BillingDiscountType.Flat)
            //    { discount = invoiceHelper.DiscountModel.DiscountValue; }
            //    else if (invoiceHelper.DiscountModel.BillingDiscountType == BillingDiscountType.Percent)
            //    { discount = Math.Round(invoiceHelper.Amount * invoiceHelper.DiscountModel.DiscountValue, 2); }
            //}

            Invoice invoice = new Invoice
            {
                InvoiceNumber = cashflowResponse.Invoice.Number,
                Amount = Math.Round(cashflowResponse.Invoice.AmountDue, 2),
                Mda = model.Mda,
                TaxPayer = model.Entity,
                Status = (int)InvoiceStatus.Unpaid,
                RevenueHead = model.RevenueHead,
                TaxPayerCategory = model.Category,
                DueDate = invoiceHelper.DueDate,
                InvoiceURL = cashflowResponse.Invoice.IntegrationPreviewUrl,
                CashflowInvoiceIdentifier = invoiceHelper.UniqueInvoiceIdentifier,
                ExpertSystemSettings = model.ExpertSystem,
                ExternalRefNumber = model.ExternalRefNumber,
                InvoiceType = (int)invoiceType,
                InvoiceTypeId = typeId,
                GeneratedByAdminUser = model.AdminUser,
                InvoiceDescription = invoiceHelper.InvoiceDescription,
                CallBackURL = model.CallBackURL,
                Quantity = model.Quantity == 0 ? 1 : model.Quantity,
                InvoiceTitle = invoiceHelper.Title,
            };
            invoice.InvoiceModel = JsonConvert.SerializeObject(invoiceHelper);
            if (!_invoiceRepository.Save(invoice)) { throw new CouldNotSaveInvoiceOnCentralBillingException(); }
            return invoice;
        }


        /// <summary>
        /// create invoice details
        /// </summary>
        /// <param name="invoiceHelper"></param>
        /// <param name="model"></param>
        /// <param name="resultfromCF"></param>
        /// <returns>Invoice</returns>
        /// <exception cref="CouldNotSaveInvoiceOnCentralBillingException"></exception>
        public virtual Invoice SaveInvoice(CreateInvoiceHelper invoiceHelper, CreateInvoiceUserInputModel model, CashFlowCreateCustomerAndInvoiceResponse resultfromCF, ExpertSystemVM expertSettingsVM, TaxEntity entity, GenerateInvoiceRequestModel revenueHeadDetails, UserPartRecord adminUser = null)
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
                InvoiceType = (int)InvoiceType.Standard,
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


        #endregion


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public CreateInvoiceHelper GetHelperModel(GenerateInvoiceRequestModel groupDetails, CreateInvoiceUserInputModel model, DateTime invoiceDate)
        {
            CreateInvoiceHelper helperModel = new CreateInvoiceHelper { };
            ////we need an amount for that invoice
            //get due date
            helperModel.DueDate = GetDueDate(groupDetails.BillingModelVM, invoiceDate);
            //get title
            helperModel.Title = model.InvoiceTitle;
            //get type
            helperModel.Type = "Single";
            //invoice description
            helperModel.InvoiceDescription = model.InvoiceDescription;
            //get footnotes for discounts and penalties that are to be applied
            helperModel.FootNotes = GetFootNotes(groupDetails.BillingModelVM);
            //invoice date
            helperModel.InvoiceDate = invoiceDate;
            helperModel.VAT = model.VAT;

            return helperModel;
        }


        /// <summary>
        /// Build foot notes text
        /// </summary>
        /// <param name="billingModelVM"></param>
        /// <returns>string</returns>
        private string GetFootNotes(BillingModelVM billingVM)
        {
            string discountConcat = ""; string penaltyConcat = "";
            if (!string.IsNullOrEmpty(billingVM.DiscountJSONModel))
            {
                List<DiscountModel> discounts = JsonConvert.DeserializeObject<List<DiscountModel>>(billingVM.DiscountJSONModel);
                if (discounts.Any())
                {
                    discountConcat = "Discounts:\r\n";
                    foreach (var item in discounts)
                    {
                        var rate = item.BillingDiscountType == BillingDiscountType.Flat ? "Naira flat rate" : "% percent";
                        discountConcat += string.Format("\u2022 {0} {1} discount is applicable {2} {3} after invoice generation \r\n", item.Discount, rate, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }

            if (!string.IsNullOrEmpty(billingVM.PenaltyJSONModel))
            {
                List<PenaltyModel> penalties = JsonConvert.DeserializeObject<List<PenaltyModel>>(billingVM.PenaltyJSONModel);
                if (penalties.Any())
                {
                    penaltyConcat = "Penalties:\r\n";
                    foreach (var item in penalties)
                    {
                        var rate = item.PenaltyValueType == PenaltyValueType.FlatRate ? "Naira flat rate" : "% percent";
                        penaltyConcat += string.Format("\u2022 penalty is applicable {2} {3} after due date \r\n", item.Value, item.PenaltyValueType, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                }
            }
            return discountConcat + penaltyConcat;
        }


        /// <summary>
        /// Get due date on invoice date
        /// </summary>
        /// <param name="billingVM"></param>
        /// <param name="invoiceDate"></param>
        /// <returns>DateTime</returns>
        private DateTime GetDueDate(BillingModelVM billingVM, DateTime invoiceDate)
        {
            if (string.IsNullOrEmpty(billingVM.DueDate))
            { throw new NoDueDateTypeFoundException("No due date found for billing " + billingVM.Id); }

            DueDateModel dueDate = JsonConvert.DeserializeObject<DueDateModel>(billingVM.DueDate);
            if (dueDate == null)
            { throw new NoDueDateTypeFoundException("No due date found for billing. Due date is null for billing Id " + billingVM.Id); }

            //if the due date is due on the next billing date
            if (dueDate.DueOnNextBillingDate) { return billingVM.NextBillingDate.Value; }

            switch (dueDate.DueDateAfter)
            {
                case DueDateAfter.Days:
                    return invoiceDate.AddDays(dueDate.DueDateInterval);
                case DueDateAfter.Weeks:
                    return invoiceDate.AddDays(7 * dueDate.DueDateInterval);
                case DueDateAfter.Months:
                    return invoiceDate.AddMonths(dueDate.DueDateInterval);
                case DueDateAfter.Years:
                    return invoiceDate.AddYears(dueDate.DueDateInterval);
                default:
                    throw new NoDueDateTypeFoundException("No due date found for billing ");
            }
        }


        protected List<InvoiceItems> SaveInvoiceItems(CreateInvoiceHelper invoiceHelper, Invoice invoice)
        {
            try
            {
                if (invoiceHelper.Items.Count > 1) { throw new NotImplementedException("Multiple invoice intems not supported now"); }

                List<InvoiceItems> items = new List<InvoiceItems>(invoiceHelper.Items.Count);
                foreach (var item in invoiceHelper.Items)
                {
                    items.Add(new InvoiceItems
                    {
                        InvoiceNumber = invoice.InvoiceNumber,
                        Invoice = invoice,
                        Mda = invoice.Mda,
                        RevenueHead = invoice.RevenueHead,
                        UnitAmount = item.Price,
                        Quantity = item.Qty,
                        InvoicingUniqueIdentifier = invoice.CashflowInvoiceIdentifier,
                        TaxEntity = invoice.TaxPayer,
                        TaxEntityCategory = invoice.TaxPayerCategory,
                    });
                }

                if (!_invoiceItemsRepository.SaveBundleUnCommitStateless(items))
                { throw new Exception("Error saving invoice items details for invoice" + invoice.InvoiceNumber); }

                return items;
            }
            catch (Exception exception)
            {
                throw;
            }

        }


        protected void SaveFormData(Invoice invoice, InvoiceItems invoiceItems, List<UserFormDetails> formFieldsValues)
        {
            if(formFieldsValues == null || !formFieldsValues.Any()) { return; }

            List<FormControlRevenueHeadValue> formValues = new List<FormControlRevenueHeadValue> { };
            foreach (var form in formFieldsValues)
            {
                if (string.IsNullOrEmpty(form.FormValue)) { continue; }
                formValues.Add(new FormControlRevenueHeadValue
                {
                    FormControlRevenueHead = new FormControlRevenueHead { Id = form.ControlIdentifier },
                    Invoice = invoice,
                    InvoiceItem = invoiceItems,
                    Value = form.FormValue
                });
            }

            if (!formValues.Any()) { return; }

            if (!_formValueRepo.SaveBundleUnCommit(formValues))
            {
                Logger.Error("Cannot save transaction log");
                throw new CannotSaveTaxEntityException();
            }
        }

    }
}