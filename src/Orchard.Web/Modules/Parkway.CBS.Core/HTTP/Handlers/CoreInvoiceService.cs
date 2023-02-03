using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Models;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Events.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Invoicing.Contracts;
using System.Text;
using System.Collections.Concurrent;
using Parkway.Cashflow.Ng.Models;
using Newtonsoft.Json;
using Orchard.Users.Models;
using System.Data;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreInvoiceService : ICoreInvoiceService
    {
        public IInvoicingService _invoicingService;
        private readonly ICoreRevenueHeadService _revenueHeadCoreService;
        private readonly IBillingScheduleManager<BillingSchedule> _scheduleRepository;
        private readonly IInvoiceManager<Invoice> _invoiceRepository;
        private readonly IPaymentProviderValidationConstraintManager<PaymentProviderValidationConstraint> _validationConstraintRepo;


        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxEntityCategoryRepository;
        private readonly ICoreTaxPayerService _taxPayerService;

        private readonly IInvoiceCreatedEventHandler _invoiceEventHandler;
        private readonly IRevenueHeadStatisticsEventHandler _statsEventHandler;

        private readonly ICoreBillingService _billingService;
        private readonly IEnumerable<IInvoiceGenerationType> _invoiceGenerationTypes;
        private readonly IAPIRequestManager<APIRequest> _apiRequestRepository;
        private readonly ITransactionLogManager<TransactionLog> _tranlogRepository;
        private readonly IInvoiceItemsManager<InvoiceItems> _invoiceItemsRepository;
        public ILogger Logger { get; set; }
        private readonly Lazy<ICoreFormService> _coreFormService;
        private readonly IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> _formValueRepo;


        public CoreInvoiceService(IInvoicingService invoicingService, ICoreRevenueHeadService revenueHeadCoreService, IBillingScheduleManager<BillingSchedule> scheduleRepository, IInvoiceManager<Invoice> invoiceRepository, ITaxEntityCategoryManager<TaxEntityCategory> taxPayerCategoryRepository, ICoreTaxPayerService taxPayerService, IInvoiceCreatedEventHandler invoiceEventHandler,
                                    IRevenueHeadStatisticsEventHandler statsEventHandler, ICoreBillingService billingService, IEnumerable<IInvoiceGenerationType> invoiceGenerationTypes, IAPIRequestManager<APIRequest> apiRequestRepository, ITransactionLogManager<TransactionLog> tranlogRepository, IInvoiceItemsManager<InvoiceItems> invoiceItemsRepository, IPaymentProviderValidationConstraintManager<PaymentProviderValidationConstraint> validationConstraintRepo, Lazy<ICoreFormService> coreFormService, IFormControlRevenueHeadValueManager<FormControlRevenueHeadValue> formValueRepo)
        {
            _invoicingService = invoicingService;
            _revenueHeadCoreService = revenueHeadCoreService;
            Logger = NullLogger.Instance;
            _scheduleRepository = scheduleRepository;
            _invoiceRepository = invoiceRepository;
            _taxEntityCategoryRepository = taxPayerCategoryRepository;
            _taxPayerService = taxPayerService;
            _invoiceEventHandler = invoiceEventHandler;
            _statsEventHandler = statsEventHandler;
            _billingService = billingService;
            _invoiceGenerationTypes = invoiceGenerationTypes;
            _apiRequestRepository = apiRequestRepository;
            _tranlogRepository = tranlogRepository;
            _invoiceItemsRepository = invoiceItemsRepository;
            _validationConstraintRepo = validationConstraintRepo;
            _coreFormService = coreFormService;
            _formValueRepo = formValueRepo;
        }


        /// <summary>
        /// using the request refrence check if this call already exists
        /// </summary>
        /// <param name="requestRef"></param>
        /// <param name="expertSystemVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        private InvoiceGenerationResponse CheckForRef(string requestRef, ExpertSystemVM expertSystemVM)
        {
            if (!string.IsNullOrEmpty(requestRef))
            {
                Logger.Information("Searching for reference");
                Int64 refResult = _apiRequestRepository.GetResourseIdentifier(requestRef, new ExpertSystemSettings { Id = expertSystemVM.Id }, CallTypeEnum.Invoice);
                if (refResult != 0)
                {
                    Logger.Information(string.Format("Request reference found for TryCreateInvoice client ID: {0} Ref: {1} refResult: {2}", expertSystemVM.Id, requestRef, (object)refResult));
                    //get the invoice details
                    InvoiceGenerationResponse refResultOBJ = _invoiceRepository.GetInvoiceByInvoiceIdForDuplicateRecordsX(refResult);
                    if (refResultOBJ == null)
                    { throw new NoInvoicesMatchingTheParametersFoundException("Cannot find invoice with ref " + refResult); }
                    return refResultOBJ;
                }
            }
            return null;
        }



        /// <summary>
        /// We are checking if all the revenue heads in the user input are of the same revenue head Id
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        public void CheckIfRevenueHeadAreallTheSameIfNoGroupIdFound(CreateInvoiceUserInputModel model, ref List<ErrorModel> errors)
        {
            //check that this either contains only one revenue head model or there is only one distinct revenue head model
            if (model.RevenueHeadModels.Count > 1)
            {
                if (model.RevenueHeadModels.GroupBy(v => v.RevenueHeadId).Count() > 1)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Group Revenue head Id is required for this operation " + model.GroupId, FieldName = "GroupId" });
                    throw new DirtyFormDataException();
                }
            }
        }


        /// <summary>
        /// Here we check that the group Id is not part of the revenue head item
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        public void CheckGroupIdIsPartOfRevenueHead(CreateInvoiceUserInputModel model, ref List<ErrorModel> errors)
        {
            if (model.RevenueHeadModels.Where(m => m.RevenueHeadId == model.GroupId).Any())
            {
                errors.Add(new ErrorModel
                {
                    ErrorMessage = string.Format("Revenue head {0} does not belong to this group", model.GroupId),
                    FieldName = "RevenueHeadId"
                });
                throw new DirtyFormDataException();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errors"></param>
        /// <param name="expertSystemVM"></param>
        /// <returns>InvoiceGenerationResponse</returns>
        public InvoiceGenerationResponse TryGenerateInvoice(CreateInvoiceUserInputModel model, ref List<ErrorModel> errors, ExpertSystemVM expertSystemVM, TaxEntityViewModel entityVM, UserPartRecord adminUser = null)
        {

            InvoiceGenerationResponse exisitingRequest = CheckForRef(model.RequestReference, expertSystemVM);
            if (exisitingRequest != null) return exisitingRequest;

            try
            {
                DateTime invoiceDate = DateTime.Now.ToLocalTime();
                //get the tax entity and the tax entity category
                var taxEntityAndCategoryObj = GetTaxEntity(entityVM, expertSystemVM.Id);
                TaxEntity entity = taxEntityAndCategoryObj.TaxEntity;

                //
                Logger.Information(string.Format("Generating invoice {0}, Date {1}", JsonConvert.SerializeObject(model), invoiceDate));

                //get group id
                //if this bundle of revenue heads does not contain a group Id
                //we are assumming that this is a bunch of revenue heads
                //that are of the same Id
                //or
                //we need to check that the group Id is not part of the revenue head ids
                GenerateInvoiceRequestModel groupDetails = GetGroupRevenueHeadDetails(model, ref errors);

                Logger.Information(string.Format("Group Id has been found {0}. Continuing processing..", model.GroupId));

                //get the revenue head details from the database
                List<RevenueHeadCombinedHelper> rhHelperModels = GetRevenueHeadDetailsHelper(model.RevenueHeadModels, model.DontValidateFormControls);
                SetPositionForRevenueHeadModels(rhHelperModels);
                List<GenerateInvoiceRequestModel> requestModel = ValidateRevenueHeadDetailsFromDB(rhHelperModels, taxEntityAndCategoryObj.TaxEntityCategory.Id, ref errors);
                rhHelperModels = null;
                //
                //Add form values to helper model
                ValidateFormValues(requestModel, ref errors, model.DontValidateFormControls);
                //
                List<CashFlowCreateInvoice.CashFlowProductModel> invoiceItemsCF = DoValidationForRevenueHeadsAndGetInvoiceItems(requestModel, ref errors);
                //what happens here is that if this request is not attached to a group, we take the first element as the gropu leader
                if (groupDetails == null) { groupDetails = requestModel.ElementAt(0); }
                //get implementation
                IInvoiceGenerationType implementation = GetInvoiceGenerationImplementation(groupDetails.BillingModelVM.BillingType);

                CreateInvoiceHelper invoiceHelper = implementation.GetHelperModel(groupDetails, model, invoiceDate);

                invoiceHelper.Items = invoiceItemsCF;
                //get helper model for invoice generation
                CashFlowCreateCustomerAndInvoice cashFlowRequestModel = implementation.GetCashFlowRequestModel(expertSystemVM.StateId, entity, invoiceHelper);
                //add invoice items

                CashFlowCreateCustomerAndInvoiceResponse resultfromCF = implementation.CreateInvoiceOnCashflow(groupDetails.MDAVM.SMEKey, cashFlowRequestModel);

                Invoice invoice = implementation.SaveInvoice(invoiceHelper, model, resultfromCF, expertSystemVM, entity, groupDetails, null);

                //save invoice items
                List<InvoiceItemFormsAndPosition> invoiceItemsFormsAndPosition = SaveInvoiceItems(invoice, requestModel);
                implementation.SaveTransactionLog(invoice, new GenerateInvoiceModel { Entity = entity, AdminUser = null }, invoiceItemsFormsAndPosition.Select(invpos => invpos.InvoiceItems).ToList());
                //
                //do form things here
                SaveAddtionalFormDetails(invoiceItemsFormsAndPosition);
                //
                if (!string.IsNullOrEmpty(model.RequestReference))
                { invoice.APIRequest = implementation.SaveAPIRequestReference(new ExpertSystemSettings { Id = expertSystemVM.Id }, invoice.Id, model.RequestReference); }

                return new InvoiceGenerationResponse
                {
                    InvoiceTitle = resultfromCF.Invoice.Title,
                    InvoiceDescription = invoiceHelper.InvoiceDescription,
                    AmountDue = resultfromCF.Invoice.AmountDue + 00.00m,
                    CallBackURL = model.CallBackURL,
                    Email = entity.Email,
                    InvoiceNumber = resultfromCF.Invoice.Number,
                    ExternalRefNumber = model.ExternalRefNumber,
                    PhoneNumber = entity.PhoneNumber,
                    Recipient = entity.Recipient,
                    InvoicePreviewUrl = invoice.InvoiceURL,
                    TIN = entity.TaxPayerIdentificationNumber,
                    InvoiceId = invoice.Id,
                    PayerId = entity.PayerId,
                    InvoiceItemsSummaries = requestModel.Select(hlp => new InvoiceItemsSummary
                    {
                        MDAName = hlp.MDAVM.Name,
                        RevenueHeadName = hlp.RevenueHeadVM.Name,
                        Quantity = hlp.Quantity,
                        RevenueHeadId = hlp.RevenueHeadVM.Id,
                        UnitAmount = hlp.Amount,
                    }).ToList()
                };
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Error in TryGenerateInvoice {0}", exception), exception);
                _invoiceRepository.RollBackAllTransactions();
                throw;
            }
        }



        private void SaveAddtionalFormDetails(List<InvoiceItemFormsAndPosition> invoiceItemsAndPosition)
        {
            List<FormControlRevenueHeadValue> formValues = new List<FormControlRevenueHeadValue> { };
            foreach (var item in invoiceItemsAndPosition)
            {
                if (item.AssociatedForms == null || !item.AssociatedForms.Any()) { continue; }
                foreach (var form in item.AssociatedForms)
                {
                    if (string.IsNullOrEmpty(form.FormValue)) { continue; }
                    formValues.Add(new FormControlRevenueHeadValue
                    {
                        FormControlRevenueHead = new FormControlRevenueHead { Id = form.ControlIdentifier },
                        Invoice = item.InvoiceItems.Invoice,
                        InvoiceItem = item.InvoiceItems,
                        Value = form.FormValue
                    });
                }
            }

            if (!formValues.Any()) { return; }

            if (!_formValueRepo.SaveBundleUnCommit(formValues))
            {
                Logger.Error("Cannot save transaction log");
                throw new CannotSaveTaxEntityException();
            }
        }



        /// <summary>
        /// this method labels the user input revenue head models with thier position  
        /// </summary>
        /// <param name="rhHelperModels"></param>
        private void SetPositionForRevenueHeadModels(List<RevenueHeadCombinedHelper> rhHelperModels)
        {
            int counter = 0;
            foreach (var item in rhHelperModels)
            {
                item.RequestModel.Index = counter++;
            }
        }


        private void ValidateFormValues(List<GenerateInvoiceRequestModel> reqModel, ref List<ErrorModel> errors, bool dontValidateForms)
        {
            IEnumerable<FormControlViewModel> formSpread = reqModel.SelectMany(uf => uf.FormValues ?? Enumerable.Empty<FormControlViewModel>());
            if (formSpread != null && formSpread.Any())
            {
                errors = _coreFormService.Value.ValidateFormValues(formSpread, dontValidateForms);
                if (errors.Count > 0) { throw new DirtyFormDataException(string.Format("Error in form validation " + string.Join(",", errors.Select(e => e.FieldName +" "+ e.ErrorMessage)))); }
            }
        }



        private List<GenerateInvoiceRequestModel> ValidateRevenueHeadDetailsFromDB(List<RevenueHeadCombinedHelper> rhHelperModels, int categoryId, ref List<ErrorModel> errors)
        {
            if (rhHelperModels == null || rhHelperModels.Count < 1)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Revenue head not founds", FieldName = "RevenueHeadId" });
                throw new DirtyFormDataException();
            }
            List<GenerateInvoiceRequestModel> requestModel = new List<GenerateInvoiceRequestModel> { };

            //check if each revenue head in the list has a details from database
            //this might not be neccessary
            int counter = 0;
            foreach (RevenueHeadCombinedHelper x in rhHelperModels)
            {
                if (x.RevenueHeadDBModel == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Revenue head not found " + x.RequestModel.RevenueHeadId, FieldName = "RevenueHeadId" });
                    throw new DirtyFormDataException();
                }

                List<FormControlViewModel> expectedForms = new List<FormControlViewModel> { };

                foreach (FormControlViewModel item in x.RevenueHeadDBModel.Forms?.Where(f => f.TaxEntityCategoryId == categoryId))
                {
                    FormControlViewModel newForm = new FormControlViewModel { };
                    newForm.ControlIdentifier = item.ControlIdentifier;
                    newForm.IsCompulsory = item.IsCompulsory;
                    newForm.ValidationProps = item.ValidationProps;
                    newForm.Validators = item.Validators;
                    newForm.RevenueHeadId = item.RevenueHeadId;
                    newForm.FormId = item.FormId;

                    FormControlViewModel userForm = x.RequestModel.FormValues?.Where(uf => uf.ControlIdentifier == item.ControlIdentifier)?.FirstOrDefault();
                    if (userForm != null)
                    {
                        newForm.FormValue = userForm.FormValue;
                        newForm.FormIndex = userForm.FormIndex;
                    }
                    else
                    {
                        newForm.FormIndex = counter++;
                    }
                    expectedForms.Add(newForm);
                }

                requestModel.Add(new GenerateInvoiceRequestModel
                {
                    MDAVM = x.RevenueHeadDBModel.MDAVM,
                    RevenueHeadVM = x.RevenueHeadDBModel.RevenueHeadVM,
                    BillingModelVM = x.RevenueHeadDBModel.BillingModelVM,
                    AdditionalDescription = x.RequestModel.AdditionalDescription,
                    Amount = x.RequestModel.ApplySurcharge ? x.RequestModel.Surcharge + x.RequestModel.Amount : x.RequestModel.Amount,
                    AmountCanVary = x.RequestModel.AmountCanVary,
                    Index = x.RequestModel.Index,
                    Quantity = x.RequestModel.Quantity,
                    FormValues = expectedForms
                });
            }

            return requestModel;
        }



        private void CheckThatTheRevenueHeadsProvidedArePartOfGroup(List<RevenueHeadUserInputModel> revenueHeadModels, GenerateInvoiceRequestModel groupDetails, ref List<ErrorModel> errors)
        {
            Dictionary<int, RevenueHeadGroupVM> groupRevenueHeads = groupDetails.RevenueHeadGroupVM.ToDictionary(k => k.RevenueHeadsInGroup);

            foreach (var item in revenueHeadModels)
            {
                if (!groupRevenueHeads.ContainsKey(item.RevenueHeadId))
                {
                    errors.Add(new ErrorModel
                    {
                        ErrorMessage = string.Format("Revenue head {0} does not belong to this group", item.RevenueHeadId),
                        FieldName = "RevenueHeadId"
                    });
                    throw new DirtyFormDataException();
                }
            }
        }


        /// <summary>
        /// Do a validation on the requst for invoice generation for this billing type
        /// Validate the request model agianst the rules for invoice generation for this revenue head
        /// </summary>
        /// <param name="requestModels">List{GenerateInvoiceRequestModel}, list of model to be validated</param>
        /// <returns>RevenueHeadRequestValidationObject</returns>
        public RevenueHeadRequestValidationObject ValidateRequestModel(List<GenerateInvoiceRequestModel> requestModels)
        {
            ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel> partInvoiceItems = new ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel> { };

            foreach (var item in requestModels)
            {
                if (item.Amount <= 0)
                { return new RevenueHeadRequestValidationObject { HasError = true, ErrorMessage = string.Format("Amount {0} for Revenue head {1} is invalid", item.Amount, item.RevenueHeadVM.Id) }; }

                if (!item.AmountCanVary)
                {
                    if (item.Amount < item.BillingModelVM.Amount)
                    {
                        return new RevenueHeadRequestValidationObject { HasError = true, ErrorMessage = string.Format("Amount {0} is less than the expected amount {1}", item.Amount, item.BillingModelVM.Amount) };
                    }
                }

                partInvoiceItems.Push(new CashFlowCreateInvoice.CashFlowProductModel
                {
                    Pos = 1,
                    Price = Math.Round(item.Amount, 2),
                    ProductId = item.RevenueHeadVM.CashflowProductId,
                    ProductName = string.Format("{0}-{1}: {2}", item.RevenueHeadVM.Name, item.RevenueHeadVM.Code, item.AdditionalDescription),
                    Qty = item.Quantity,
                });
            }
            return new RevenueHeadRequestValidationObject { PartInvoiceItems = partInvoiceItems };
        }



        /// <summary>
        /// Save Invoice Items
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="rhHelperModels"></param>
        private List<InvoiceItemFormsAndPosition> SaveInvoiceItems(Invoice invoice, List<GenerateInvoiceRequestModel> requestModel)
        {
            List<InvoiceItemFormsAndPosition> invoiceItemAndPostion = new List<InvoiceItemFormsAndPosition> { };
            //FormControlRevenueHeadValue
            foreach (var item in requestModel)
            {
                invoiceItemAndPostion.Add(new InvoiceItemFormsAndPosition
                {
                    InvoiceItems = new InvoiceItems
                    {
                        Invoice = invoice,
                        Mda = new MDA { Id = item.MDAVM.Id },
                        RevenueHead = new RevenueHead { Id = item.RevenueHeadVM.Id },
                        TaxEntity = invoice.TaxPayer,
                        InvoiceNumber = invoice.InvoiceNumber,
                        Quantity = item.Quantity,
                        UnitAmount = item.Amount,
                        InvoicingUniqueIdentifier = item.CashflowUniqueIdentifier,
                        TaxEntityCategory = invoice.TaxPayerCategory,
                    },
                    Position = item.Index,
                    AssociatedForms = item.FormValues
                });
            }
            if (!_invoiceItemsRepository.SaveBundleUnCommit(invoiceItemAndPostion))
            { throw new CannotSaveRecordException("cannot save invoice items"); }

            return invoiceItemAndPostion;
        }


        /// <summary>
        /// Get invoice generation implementation for billing type
        /// </summary>
        /// <param name="billingType"></param>
        /// <returns>IInvoiceGenerationType</returns>
        private IInvoiceGenerationType GetInvoiceGenerationImplementation(BillingType billingType)
        {
            foreach (var item in _invoiceGenerationTypes)
            {
                if (item.InvoiceGenerationType == billingType)
                {
                    return item;
                }
            }
            throw new NoBillingTypeSpecifiedException("No billing type " + billingType);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityVM"></param>
        /// <returns></returns>
        private dynamic GetTaxEntity(TaxEntityViewModel entityVM, int expertSystemId)
        {
            TaxEntity taxEntity = null;
            TaxEntityCategory category = null;

            if (!string.IsNullOrEmpty(entityVM.PayerId))
            {
                //if the tax entity Id is given
                taxEntity = _taxPayerService.GetTaxEntity(txp => txp.PayerId == entityVM.PayerId);
                if (taxEntity == null) { throw new NoRecordFoundException("Specified tax profile not found"); }
                category = taxEntity.TaxEntityCategory;
            }
            else if (entityVM.Id > 0)
            {
                //if the tax entity Id is given
                taxEntity = _taxPayerService.GetTaxEntity(txp => txp.Id == entityVM.Id);
                if (taxEntity == null) { throw new NoRecordFoundException("Specified tax profile not found"); }
                category = taxEntity.TaxEntityCategory;
            }
            else
            {
                category = _taxEntityCategoryRepository.Get(entityVM.CategoryId);
                if (category == null) { throw new NoCategoryFoundException(); }

                taxEntity = new TaxEntity
                {
                    TaxPayerIdentificationNumber = string.IsNullOrEmpty(entityVM.TaxPayerIdentificationNumber) ? null : entityVM.TaxPayerIdentificationNumber.Trim(),
                    Address = entityVM.Address.Trim(),
                    Email = string.IsNullOrEmpty(entityVM.Email) ? null : entityVM.Email.Trim(),
                    PhoneNumber = string.IsNullOrEmpty(entityVM.PhoneNumber) ? null : entityVM.PhoneNumber.Trim(),
                    Recipient = entityVM.Recipient.Trim(),
                    StateLGA = new LGA { Id = entityVM.DefaultLGAId }
                };

                if (expertSystemId > 0) { taxEntity.AddedByExternalExpertSystem = new ExpertSystemSettings { Id = expertSystemId }; }

                var result = _taxPayerService.ValidateAndSaveTaxEntity(taxEntity, category);
                category = result.Category;
                taxEntity = result.TaxEntity;
            }
            return new { TaxEntity = taxEntity, TaxEntityCategory = category };
        }


        /// <summary>
        /// Get the group details for this
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns>RevenueHeadForInvoiceGenerationHelper</returns>
        private GenerateInvoiceRequestModel GetGroupRevenueHeadDetails(CreateInvoiceUserInputModel model, ref List<ErrorModel> errors)
        {
            if (model.GroupId < 1)
            {
                //here the revenue head bundle is not attached to any group
                //this could mean that the revenue heads here are of the same revenue head Id
                //so we do a check to make sure all revenue heads are the same
                CheckIfRevenueHeadAreallTheSameIfNoGroupIdFound(model, ref errors);
                return null;
            }

            //we need to check that the group Id is not part of the revenue head ids
            CheckGroupIdIsPartOfRevenueHead(model, ref errors);

            //here we get the group details
            GenerateInvoiceRequestModel groupDetails = _revenueHeadCoreService.GetGroupRevenueHeadDetailsForInvoice(model.GroupId);

            if (groupDetails == null)
            {
                errors.Add(new ErrorModel { ErrorMessage = string.Format("Group Revenue head {0} not found", model.GroupId), FieldName = "GroupId" });
                throw new DirtyFormDataException();
            }

            //check that the revenue heads are in the group
            //here we confirm that the revenue heads in the user input are indeed part of the group
            CheckThatTheRevenueHeadsProvidedArePartOfGroup(model.RevenueHeadModels, groupDetails, ref errors);
            return groupDetails;
        }





        /// <summary>
        /// Do validation for revenue heads
        /// <para>This method groups the revenue heads by revenue head id, 
        /// then does the neccessary validation for each revenue head.
        /// A RevenueHeadRequestValidationObject object is returned for the validation bit
        /// This would in turn contain the CashFlowProductModel for invoice generation or
        /// would contain and error if the revenue head model fails validation
        /// </para>
        /// </summary>
        /// <param name="reqModels">List{GenerateInvoiceRequestModel}</param>
        /// <returns>List{CashFlowCreateInvoice.CashFlowProductModel}</returns>
        private List<CashFlowCreateInvoice.CashFlowProductModel> DoValidationForRevenueHeadsAndGetInvoiceItems(List<GenerateInvoiceRequestModel> reqModels, ref List<ErrorModel> errors)
        {
            var grp = reqModels
               .GroupBy(r => r.RevenueHeadVM.Id, r => r,
               (key, values) => new
               {
                   Key = key,
                   values.ElementAt(0).BillingModelVM.BillingType,
                   RequestModelsList = values.ToList(),
               });

            ConcurrentStack<ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel>> invoiceItems = new ConcurrentStack<ConcurrentStack<CashFlowCreateInvoice.CashFlowProductModel>> { };

            foreach (var grpItem in grp)
            {
                RevenueHeadRequestValidationObject result = ValidateRequestModel(grpItem.RequestModelsList);
                if (result.HasError)
                {
                    errors.Add(new ErrorModel { ErrorMessage = result.ErrorMessage, FieldName = "RevenueHeadId" });
                    throw new DirtyFormDataException { };
                }
                invoiceItems.Push(result.PartInvoiceItems);
            }

            return invoiceItems.SelectMany(invItms => invItms).ToList();
        }


        /// <summary>
        /// Get helper details for revenue heads
        /// <para>This method makes database calls to get the needed models for invoice generation.
        /// The provided list is grouped by revenue head. 
        /// It groups the revenue heads by revenue head id, do a database call,
        /// then adds the result to the list in each group. So at the end of the day
        /// the number of revenue head items would be returned instead of the group count.
        /// Example: id of revenue heads 1, 2, 3, 4, 3. the grouping would have 3 elements
        /// with key 1, 2, 3, 4. Element at key 3 would have a list of two revenue heads model,
        /// while the remaining would have one element each.
        /// </para>
        /// </summary>
        /// <param name="revenueHeadModels"></param>
        /// <returns>List{RevenueHeadCombinedHelper}</returns>
        private List<RevenueHeadCombinedHelper> GetRevenueHeadDetailsHelper(List<RevenueHeadUserInputModel> revenueHeadModels, bool dontGetFormDetails)
        {
            StringBuilder revList = new StringBuilder();
            try
            {
                var grp = revenueHeadModels.GroupBy(r => r.RevenueHeadId, r => r);
                List<RevenueHeadCombinedHelper> rhList = new List<RevenueHeadCombinedHelper>();

                foreach (var revenueHeadModel in grp)
                {
                    //this gets the revenue head details from the database
                    IEnumerable<RevenueHeadForInvoiceGenerationHelper> result = _revenueHeadCoreService.GetRevenueHeadDetailsForInvoice(revenueHeadModel.Key, dontGetFormDetails);
                    //this adds to the list, where each item has a combiantion for the revenue head model from the user and the corresponding revenue head details from the database
                    //this allows us to do what ever comaprison or addtional validation requirements
                    rhList.AddRange(revenueHeadModel.Select(r => new RevenueHeadCombinedHelper { RequestModel = r, RevenueHeadEssentialsFromDB = result }));
                    revList.AppendFormat(revenueHeadModel.Key + ", ");
                }
                //lets fire one, just triggering the future query
                rhList.ElementAt(0).RevenueHeadDBModel.RevenueHeadVM.Id.ToString();
                //
                Logger.Information(string.Format("Revenue head details gotten from DB values {0}", revList.ToString().Trim(", ".ToCharArray())));
                return rhList;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Format("Exception getting revenue head models from DB values {0}. Exception {1}", revList.ToString().Trim(", ".ToCharArray()), exception), exception);
                throw;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Create invoice
        /// </summary>
        /// <typeparam name="API"></typeparam>
        /// <param name="callback"></param>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        public InvoiceGeneratedResponseExtn TryCreateInvoice(CreateInvoiceModel model, ref List<ErrorModel> errors, ExpertSystemSettings expertSystem = null, string requestRef = null)
        {
            if (!string.IsNullOrEmpty(requestRef))
            {
                Logger.Information("Searching for reference");
                Int64 refResult = _apiRequestRepository.GetResourseIdentifier(requestRef, expertSystem, CallTypeEnum.Invoice);
                if (refResult != 0)
                {
                    Logger.Information(string.Format("Request reference found for TryCreateInvoice client ID: {0} Ref: {1} refResult: {2}", expertSystem.ClientId, requestRef, (object)refResult));
                    //get the invoice details
                    InvoiceGeneratedResponseExtn refResultOBJ = _invoiceRepository.GetInvoiceByInvoiceIdForDuplicateRecords(refResult);
                    if (refResultOBJ == null) { throw new NoInvoicesMatchingTheParametersFoundException("Cannot find invoice with ref " + refResult); }
                    return refResultOBJ;
                }
            }


            DateTime invoiceDate = DateTime.Now.ToLocalTime();
            Logger.Information("Generating invoice " + invoiceDate);
            Logger.Information("Getting revenue head and mda record RH:" + model.RevenueHeadId);

            RevenueHeadDetailsForInvoiceGeneration revenueHeadDetails = _revenueHeadCoreService.GetRevenueHeadDetailsForInvoiceGeneration(model.RevenueHeadId);

            if (expertSystem != null) { revenueHeadDetails.ExpertSystem = expertSystem; }

            if (revenueHeadDetails.Mda == null) { throw new MDARecordNotFoundException("No mda for revenue head found " + model.RevenueHeadId); }

            if (revenueHeadDetails.ExpertSystem == null) { Logger.Error("Expert system not found"); throw new TenantNotFoundException("Could not find expert system"); }
            //now check that the expert system is active
            if (!revenueHeadDetails.ExpertSystem.IsActive)
            { Logger.Error("Expert system has been disbaled " + revenueHeadDetails.ExpertSystem.Id); throw new BillingIsNotAllowedException("Expert system has been disabled " + revenueHeadDetails.ExpertSystem.Id); }

            if (revenueHeadDetails.Tenant == null) { throw new TenantNotFoundException("State settings not found"); }
            //CHECK IF BILLING IS ALLOWED
            Logger.Information("Checking is billing is allowed on mda and revenue head");
            bool isBillingAllowed = IsBillingAllowed(revenueHeadDetails.Mda, revenueHeadDetails.RevenueHead);
            if (!isBillingAllowed)
            {
                Logger.Error("Billing not allowed RH " + model.RevenueHeadId);
                throw new BillingIsNotAllowedException("no billing allowed for revenue head " + model.RevenueHeadId);
            }
            Logger.Information("Getting billing info");
            //lets check if this revenue head indeed has billing info
            //TODO: i THINK MOST OF THESE CHECKS ARE UNECCESSARY, an inner has already been done in the select query
            if (revenueHeadDetails.Billing == null)
            { throw new NoBillingInformationFoundException("No billing information for this revenue head" + model.RevenueHeadId); }

            if (!revenueHeadDetails.Billing.StillRunning)
            {
                Logger.Error("Billing has expired");
                throw new BillingHasEndedException("no billing allowed for revenue head " + model.RevenueHeadId);
            }

            //lets check if the create invoice model tax payer Id has a value
            TaxEntity taxEntity = null;
            TaxEntityCategory category = null;

            if (!string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.PayerId))
            {
                //if the tax entity Id is given
                taxEntity = _taxPayerService.GetTaxEntity(txp => txp.PayerId == model.TaxEntityInvoice.TaxEntity.PayerId);
                if (taxEntity == null) { throw new NoRecordFoundException("Specified tax profile not found"); }
                category = taxEntity.TaxEntityCategory;
            }
            else if (model.TaxEntityInvoice.TaxEntity.Id > 0)
            {
                //if the tax entity Id is given
                taxEntity = _taxPayerService.GetTaxEntity(txp => txp.Id == model.TaxEntityInvoice.TaxEntity.Id);
                if (taxEntity == null) { throw new NoRecordFoundException("Specified tax profile not found"); }
                category = taxEntity.TaxEntityCategory;
            }
            else
            {
                category = _taxEntityCategoryRepository.Get(model.TaxEntityInvoice.CategoryId);
                if (category == null) { throw new NoCategoryFoundException(); }

                taxEntity = new TaxEntity
                {
                    TaxPayerIdentificationNumber = string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.TaxPayerIdentificationNumber) ? null : model.TaxEntityInvoice.TaxEntity.TaxPayerIdentificationNumber.Trim(),
                    Address = string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.Address) ? null : model.TaxEntityInvoice.TaxEntity.Address.Trim(),
                    Email = string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.Email) ? null : model.TaxEntityInvoice.TaxEntity.Email.Trim(),
                    PhoneNumber = string.IsNullOrEmpty(model.TaxEntityInvoice.TaxEntity.PhoneNumber) ? null : model.TaxEntityInvoice.TaxEntity.PhoneNumber.Trim(),
                    Recipient = model.TaxEntityInvoice.TaxEntity.Recipient.Trim(),
                    TaxPayerCode = model.TaxEntityInvoice.TaxEntity.TaxPayerCode,
                    StateLGA = model.TaxEntityInvoice.TaxEntity.StateLGA,
                };
                var result = _taxPayerService.ValidateAndSaveTaxEntity(taxEntity, category);
                category = result.Category;
                taxEntity = result.TaxEntity;
            }
            Logger.Information("Getting tax payer");
            BillingType billingType = _billingService.GetBillingType(revenueHeadDetails.Billing.BillingType);

            GenerateInvoiceModel invoiceModel = new GenerateInvoiceModel
            {
                Entity = taxEntity,
                ExpertSystem = revenueHeadDetails.ExpertSystem,
                RevenueHead = revenueHeadDetails.RevenueHead,
                Mda = revenueHeadDetails.Mda,
                AddtionalDetails = model.TaxEntityInvoice.AdditionalDetails,
                Amount = model.TaxEntityInvoice.Amount,
                Billing = revenueHeadDetails.Billing,
                Category = category,
                InvoiceDate = invoiceDate,
                StateSettings = revenueHeadDetails.Tenant,
                ExternalRefNumber = model.ExternalRefNumber,
                AdminUser = model.AdminUser,
                InvoiceDescription = model.TaxEntityInvoice.InvoiceDescription,
                RequestReference = requestRef,
                CallBackURL = model.CallBackURL,
                Quantity = model.Quantity,
                VAT = model.VAT,
                Surcharge = (model.ApplySurcharge) ? revenueHeadDetails.Billing.Surcharge : 0.00m,
                FormValues = model.Forms,
            };
            return InvoiceGenerator(invoiceModel, billingType, model.FileUploadModel, category.GetSettings().IsFederalAgency);
        }


        /// <summary>
        /// Check if this request has been processed already
        /// </summary>
        /// <param name="requestRef"></param>
        public InvoiceGeneratedResponseExtn CheckRequestReference(string requestRef, ExpertSystemSettings expertSystem)
        {
            if (!string.IsNullOrEmpty(requestRef))
            {
                Logger.Information("Searching for reference");
                Int64 refResult = _apiRequestRepository.GetResourseIdentifier(requestRef, expertSystem, CallTypeEnum.Invoice);
                if (refResult != 0)
                {
                    Logger.Information(string.Format("Request reference found for client ID: {0} Ref: {1} CBSUser: {2}", expertSystem.ClientId, requestRef, refResult));
                    var persistedInvoice = _invoiceRepository.Get(x => x.Id == refResult);

                    if (persistedInvoice == null)
                    {
                        Logger.Error("Cannot find invoice request with Id " + refResult);
                        throw new Exception("Cannot find invoice request with Id " + refResult);
                    }
                    Logger.Information(string.Format("Returning ref {0}", requestRef));
                    return new InvoiceGeneratedResponseExtn { AmountDue = persistedInvoice.Amount, InvoiceNumber = persistedInvoice.InvoiceNumber, InvoicePreviewUrl = persistedInvoice.InvoiceURL };
                }
            }
            return null;
        }


        /// <summary>
        /// Get the invoice for this invoice type
        /// </summary>
        /// <param name="invoiceType"></param>
        /// <param name="invoiceTypeId"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        public InvoiceDetailsHelperModel GetInvoiceHelperDetailsByInvoiceType(InvoiceType invoiceType, long invoiceTypeId)
        { return _invoiceRepository.GetInvoiceDetails(invoiceType, invoiceTypeId); }


        public InvoiceDetailsHelperModel GetInvoiceHelperDetailsByInvoiceType(long invoiceId)
        { return _invoiceRepository.GetInvoiceDetails(invoiceId); }


        /// <summary>
        /// Get invoice details for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetailsHelperModel</returns>
        public InvoiceDetailsHelperModel GetInvoiceHelperDetails(string invoiceNumber)
        {
            return _invoiceRepository.GetInvoiceDetails(invoiceNumber);
        }


        /// <summary>
        /// Try create invoice for paye assessment
        /// <para>Check for request reference if any</para>
        /// </summary>
        /// <param name="createInvoiceModelForPayeAssessment"></param>
        /// <param name="expertSystem"></param>
        public InvoiceGeneratedResponseExtn TryCreateInvoice(CreateInvoiceModelForPayeAssessment invModel, string requestRef, ExpertSystemSettings expertSystem)
        {
            DateTime invoiceDate = DateTime.Now.ToLocalTime();
            Logger.Information("Generating invoice " + invoiceDate);
            //now check that the expert system is active
            if (!expertSystem.IsActive) { Logger.Error("Expert system has been disbaled " + expertSystem.Id); throw new BillingIsNotAllowedException("Expert system has been disabled " + expertSystem.Id); }
            //get state tenant
            TenantCBSSettings stateSettings = expertSystem.TenantCBSSettings;
            if (stateSettings == null) { throw new TenantNotFoundException("State settings not found"); }
            //CHECK IF BILLING IS ALLOWED
            Logger.Information("Checking is billing is allowed on mda and revenue head");
            var isBillingAllowed = IsBillingAllowed(invModel.MDA, invModel.RevenueHead);
            if (!isBillingAllowed)
            {
                Logger.Error("Billing not allowed RH " + invModel.RevenueHead.Id);
                throw new BillingIsNotAllowedException("no billing allowed for revenue head " + invModel.RevenueHead.Id);
            }
            Logger.Information("Getting billing info");
            if (!invModel.Billing.StillRunning)
            {
                Logger.Error("Billing has expired");
                throw new BillingHasEndedException("no billing allowed for revenue head " + invModel.RevenueHead.Id);
            }

            BillingType billingType = _billingService.GetBillingType(invModel.Billing.BillingType);
            GenerateInvoiceModel invoiceModel = new GenerateInvoiceModel
            {
                Entity = invModel.TaxEntity,
                ExpertSystem = expertSystem,
                RevenueHead = invModel.RevenueHead,
                Mda = invModel.MDA,
                AddtionalDetails = invModel.AdditionalDetails,
                Amount = invModel.Amount,
                Billing = invModel.Billing,
                Category = invModel.TaxEntityCategory,
                InvoiceDate = invoiceDate,
                StateSettings = stateSettings,
                RequestReference = requestRef
            };
            return InvoiceGenerator(invoiceModel, billingType, new FileProcessModel { DirectAssessmentBatchRecord = invModel.DirectAssessmentBatchRecord }, true);
        }


        /// <summary>
        /// Get the URL link for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>string</returns>
        /// <exception cref="NoInvoicesMatchingTheParametersFoundException"></exception>
        public string GetInvoiceURL(string invoiceNumber)
        {
            string URL = _invoiceRepository.GetInvoiceURL(invoiceNumber);
            if (string.IsNullOrEmpty(URL)) { throw new NoInvoicesMatchingTheParametersFoundException("Invoice not found"); }
            return URL;
        }


        /// <summary>
        /// Generate invoice
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="expertSystem"></param>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="billing"></param>
        /// <param name="category"></param>
        /// <param name="amount"></param>
        /// <param name="addtionalDetails"></param>
        /// <param name="invoiceDate"></param>
        /// <param name="billingType"></param>
        /// <returns>CashFlowCreateCustomerAndInvoiceResponse</returns>
        protected virtual InvoiceGeneratedResponseExtn InvoiceGenerator(GenerateInvoiceModel invoiceModel, BillingType billingType, FileProcessModel fileUploadModel, bool showRemitta)
        {
            foreach (var item in _invoiceGenerationTypes)
            {
                if (item.InvoiceGenerationType == billingType)
                {
                    try
                    {
                        //maybe we should validate first, maybe
                        return item.GenerateInvoice(invoiceModel, fileUploadModel, showRemitta);
                    }
                    catch (Exception)
                    {
                        //clean up the transcation
                        _apiRequestRepository.RollBackAllTransactions();
                        throw;
                    }
                }
            }
            throw new NoBillingTypeSpecifiedException("No billing type " + billingType.ToString());
        }


        private bool IsBillingAllowed(MDA mda, RevenueHead revenueHead)
        {
            return _billingService.IsBillingAllowed(mda, revenueHead);
        }



        /// <summary>
        /// Get receipts for this invoice
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GetInvoiceReceiptsVM(string invoiceNumber)
        {
            return _invoiceRepository.GetReceiptsBelongingToInvoiceNumber(invoiceNumber);
        }


        /// <summary>
        /// Get the invoice details for an invoice given the invoice number for make payment
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GetInvoiceDetailsForPaymentView(string invoiceNumber)
        {
            return _invoiceRepository.GetInvoiceDetailsForPaymentView(invoiceNumber);
        }


        /// <summary>
        /// Get the invoice details, along with the transactions that have been done on the invoice
        /// <para>This method does not in any way group transaction for invoice items</para>
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        public InvoiceDetails GetInvoiceTransactions(string invoiceNumber)
        {
            return _invoiceRepository.GetInvoiceTransactions(invoiceNumber);
        }


        /// <summary>
        /// This method checks if the mdaId or revenue head has any validation restriction
        /// on provied payment provider.
        /// <para>Check that the payment provider is constrainted from validating the mda or revenue head</para>
        /// </summary>
        /// <param name="mdaId"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="paymentProviderId"></param>
        /// <returns>bool</returns>
        public bool CheckForValidationConstraint(int mdaId, int revenueHeadId, int paymentProviderId)
        {
            if (_validationConstraintRepo.CountNumberOfValidationRestrictions(mdaId, revenueHeadId, paymentProviderId) > 0) { return true; }
            return false;
        }


        /// <summary>
        /// Get payment references for this invoice number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <returns>InvoiceDetails</returns>
        public InvoiceDetails GetPaymentReferencesForInvoice(string invoiceNumber)
        {
            return _invoiceRepository.GetPaymentRefs(invoiceNumber);
        }

        /// <summary>
        /// Get status of an invoice with all the payment transactions on it.
        /// This method does not in any way group transaction for invoice items
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>InvoiceStatusDetailsVM</returns>
        public InvoiceStatusDetailsVM GetInvoiceStatus(CollectionSearchParams searchParams)
        {
            try
            {
                Invoice invoice = _invoiceRepository.GetInvoiceStatus(searchParams);

                if (invoice == null) { throw new NoRecordFoundException { }; }

                InvoiceStatusDetailsVM invoiceDetails = new InvoiceStatusDetailsVM
                {
                    Recipient = invoice.TaxPayer.Recipient,
                    TIN = invoice.TaxPayer.TaxPayerIdentificationNumber,
                    Email = invoice.TaxPayer.Email,
                    PhoneNumber = invoice.TaxPayer.PhoneNumber,
                    PayerId = invoice.TaxPayer.PayerId,
                    CallBackURL = invoice.CallBackURL,
                    DueDate = invoice.DueDate,
                    RevenueHeadCallBackURL = invoice.RevenueHead.CallBackURL,
                    Status = ((InvoiceStatus)invoice.Status).ToString(),
                    RequestRef = invoice.APIRequest != null ? invoice.APIRequest.RequestIdentifier : null,
                    InvoiceNumber = invoice.InvoiceNumber,
                    AmountDue = Math.Round(invoice.InvoiceAmountDueSummary.AmountDue, 2),
                    Payments = invoice.Payments.DefaultIfEmpty().Where(p => p.TypeID != (int)PaymentType.Bill)
                            .Select(p => new TransactionLogInvoiceStatusVM
                            {
                                PaymentDate = p.PaymentDate,
                                BankCode = p.BankCode,
                                BankBranch = p.BankBranch,
                                Bank = p.Bank,
                                Channel = ((PaymentChannel)p.Channel).ToString(),
                                PaymentProvider = ((PaymentProvider)p.PaymentProvider).ToString(),
                                AmountPaid = Math.Round(p.AmountPaid, 2),
                                PaymentReference = p.PaymentReference,
                                TransactionDate = p.TransactionDate
                            }).ToList()
                };

                return invoiceDetails;
            }
            catch (UserNotAuthorizedForThisActionException ex)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns>InvalidateInvoiceVM</returns>
        public InvalidateInvoiceVM InvalidateInvoice(CollectionSearchParams searchParams)
        {
            try
            {
                Invoice invoice = _invoiceRepository.GetInvoiceStatus(searchParams);

                if (invoice == null) { throw new NoRecordFoundException { }; }

                if (invoice.IsCancel)
                {
                    return new InvalidateInvoiceVM { InvoiceNumber = invoice.InvoiceNumber, IsInvalidated = true };
                }

                if (invoice.InvoiceAmountDueSummary.AmountDue != 0.0m)
                {
                    invoice.Status = (int)InvoiceStatus.WriteOff;
                    invoice.CancelDate = DateTime.Now;
                    invoice.IsCancel = true;
                    invoice.CancelBy = new UserPartRecord { Id = searchParams.AdminUserId };
                    _invoiceRepository.Update(invoice);
                    return new InvalidateInvoiceVM { InvoiceNumber = invoice.InvoiceNumber, IsInvalidated = true };
                }
                throw new InvoiceHasPaymentException($"Invoice number {searchParams.InvoiceNumber} has been fully paid.");
            }
            catch (UserNotAuthorizedForThisActionException ex)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Check if payment provider has restrictions on this given MDA and revenue head
        /// </summary>
        public bool CheckForRestrictions(InvoiceGeneratedResponseExtn invoiceDetails, int paymentProviderId)
        {
            //do check if validation check is required
            if (invoiceDetails.HasPaymentProviderValidationConstraint)
            {
               return CheckForValidationConstraint(invoiceDetails.MDAId, invoiceDetails.RevenueHeadID, paymentProviderId);               
            }
            return false;
        }

    }
}