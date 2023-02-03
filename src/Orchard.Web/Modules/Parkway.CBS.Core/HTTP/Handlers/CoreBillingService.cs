using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.DataFilters.BillingFrequency.Contracts;
using System.Globalization;
using Orchard.Users.Models;
using Newtonsoft.Json;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.HTTP.Handlers.Billing.Contracts;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.FileUpload;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreBillingService : CoreBaseService, ICoreBillingService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IBillingModelManager<BillingModel> _biilingModelRepository;
        public IInvoicingService _invoicingService;
        private readonly ICoreRevenueHeadService _revenueHeadCoreService;
        private readonly IEnumerable<IBillingFrequencyFilter> _billingFrequencyFilter;
        public Localizer T { get; set; }
        private readonly IBillingScheduleManager<BillingSchedule> _scheduleRepository;
        private readonly IBillingModelManager<BillingModel> _billingRepository;
        private readonly IAuditTrailManager<AuditTrail> _auditTrailRepo;
        private readonly ICoreBillingScheduleService _coreSchedulerService;
        private readonly IAPIRequestManager<APIRequest> _apiRequestRepository;
        private readonly IEnumerable<IBillingTypes> _billingTypeCollection;
        private readonly ICoreSettingsService _coreSettingsService;
        private readonly IPayeeAssessmentConfiguration _payeeConfig;
        private readonly IFileUploadConfiguration _fileUploadConfig;
        private CashFlowProduct cashflowProduct;

        public CoreBillingService(IOrchardServices orchardServices, IBillingModelManager<BillingModel> biilingModelRepository,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider,
            IInvoicingService invoicingService, ICoreRevenueHeadService revenueHeadCoreService, IEnumerable<IBillingFrequencyFilter> billingFrequencyFilter, IBillingScheduleManager<BillingSchedule> scheduleRepository, ICoreBillingScheduleService schedulerService, IBillingModelManager<BillingModel> billingRepository, IAPIRequestManager<APIRequest> apiRequestRepository, IEnumerable<IBillingTypes> billingTypeCollection, ICoreSettingsService coreSettingsService, IPayeeAssessmentConfiguration payeeConfig, IAuditTrailManager<AuditTrail> auditTrail)
            : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _biilingModelRepository = biilingModelRepository;
            _invoicingService = invoicingService;
            _revenueHeadCoreService = revenueHeadCoreService;
            _billingFrequencyFilter = billingFrequencyFilter;
            Logger = NullLogger.Instance;
            _scheduleRepository = scheduleRepository;
            _coreSchedulerService = schedulerService;
            _billingRepository = billingRepository;
            _apiRequestRepository = apiRequestRepository;
            _billingTypeCollection = billingTypeCollection;
            _coreSettingsService = coreSettingsService;
            _payeeConfig = payeeConfig;
            _fileUploadConfig = new FileUploadConfiguration();
            _auditTrailRepo = auditTrail;
        }



        #region Create

        /// <summary>
        /// Try create billing information
        /// </summary>
        /// <param name="revenueHead">RevenueHead</param>
        /// <param name="user">UserPartRecord</param>
        /// <param name="errors">ref List{ErrorModel}</param>
        /// <param name="billingHelperModel">BillingHelperModel</param>
        /// <returns>BillingCreatedModel</returns>
        public BillingCreatedModel TryPostBillingForCollection(MDA mda, RevenueHead revenueHead, UserPartRecord user, ref List<ErrorModel> errors, BillingHelperModel billingHelperModel, bool isEdit, string requestRef = null, ExpertSystemSettings expertSystem = null)
        {
            //request ref is for API calls
            if (!string.IsNullOrEmpty(requestRef))
            {
                Logger.Information("Searching for reference");
                Int64 refResult = _apiRequestRepository.GetResourseIdentifier(requestRef, expertSystem, CallTypeEnum.Billing);
                if (refResult != 0)
                {
                    Logger.Information(string.Format("Request reference found for client ID: {0} Ref: {1} MDA: {2}", expertSystem.ClientId, requestRef, refResult));
                    if (revenueHead == null) { throw new CannotFindRevenueHeadException("Cannot find revenue head in api request with Id " + revenueHead.Id); }
                    else if (revenueHead.BillingModel == null) { throw new NoBillingInformationFoundException("Cannot find billing in api request for revenue head with Id " + revenueHead.Id); }

                    Logger.Information(string.Format("Returning ref {0}", requestRef));
                    var nextBillingDate = revenueHead.BillingModel.NextBillingDate.HasValue ? revenueHead.BillingModel.NextBillingDate.Value.ToString("dd'/'MM'/'yyyy") : "";
                    return new BillingCreatedModel
                    { InvoicingServiceProductCode = revenueHead.CashFlowProductCode, RevenueHeadId = revenueHead.Id, NextRunDate = nextBillingDate };
                }
            }

            Logger.Information("Validating model");
            if (isEdit)
            {
                errors = ValidateBillingModel(billingHelperModel);
            }
            else
            {
                errors = HasBilling(revenueHead).HasSubRevenueHeads(revenueHead).ValidateBillingModel(billingHelperModel);
            }

            if (errors.Count > 0) { throw new DirtyFormDataException(); }

            Logger.Information("Creating billing model");
            BillingModel billing = new BillingModel { };

            BillingSchedule schedule = null;

            //here we do validation based on the billing type
            ProcessCreateBillingRequest(mda, revenueHead, ref errors, billingHelperModel, ref billing, ref schedule, isEdit);

            billing.AddedBy = user;
            billing.LastUpdatedBy = user;

            if (isEdit) { billing.Id = revenueHead.BillingModel.Id; }

            //create product on cash flow
            if (!isEdit)
            {
                cashflowProduct = CreateProductsOnCashflow(revenueHead, billing, mda.SMEKey);
                revenueHead.CashFlowProductCode = cashflowProduct.ProductCode;
                revenueHead.CashFlowProductId = cashflowProduct.Id;
                if (revenueHead.IsActive) { Logger.Information("Setting revenue head visibility"); _revenueHeadCoreService.TurnOnParentVisibilty(revenueHead); revenueHead.IsVisible = true; }
            }

            //if the revenue head is active
            APIRequest requestLog = null;
            if (!string.IsNullOrEmpty(requestRef))
            {
                requestLog = new APIRequest
                { ResourceIdentifier = revenueHead.Id, RequestIdentifier = requestRef, ExpertSystemSettings = expertSystem, CallType = (short)CallTypeEnum.Billing };
            }


            if ((billingHelperModel.AssessmentModel.IsRecurring) && (billingHelperModel.BillingType == BillingType.Fixed))
            {
                Logger.Information("Setting up schedule on scheduler");
                var fixedBilling = JsonConvert.DeserializeObject<BillingFrequencyModel>(billing.BillingFrequency).FixedBill;
                string CRONExpression = fixedBilling.CRONExpression;
                DurationModel duration = JsonConvert.DeserializeObject<DurationModel>(billing.Duration);

                ScheduleHelperModel helper = new ScheduleHelperModel { BillingIdentifier = revenueHead.Id, CronExpression = fixedBilling.CRONExpression, StartDateAndTime = fixedBilling.StartDateAndTime, Duration = duration };
                //_coreSchedulerService.RegisterAFixedBillingSchedule(tenant, revenueHead, helper, BillingType.Fixed);
                //_coreSchedulerService.RegisterAFixedBillingSchedule(tenant, revenueHead, billing, BillingType.Fixed);
            }
            else if ((billingHelperModel.AssessmentModel.IsRecurring) && (billingHelperModel.BillingType == BillingType.Variable))
            {
                billing.NextBillingDate = null;
                Logger.Information("Setting up schedule on scheduler");
                //TODO scheduler for variable billing
                //_coreSchedulerService.RegisterAFixedBillingSchedule(tenant, revenueHead, billing, BillingType.Fixed);
            }

            if (isEdit)
            {
                try
                {
                    revenueHead.BillingModel = billing;
                    TryUpdatingBilling(billing, revenueHead, user);
                }
                catch (Exception exception) { Logger.Error(exception, exception.Message + exception.StackTrace); throw; }
            }
            else
            {
                try { SaveBillingAndSchedule(billing, schedule, requestLog); }
                catch (CouldNotSaveBillingException exception) { Logger.Error(exception, exception.Message + exception.StackTrace); throw; }
                revenueHead.BillingModel = billing;
            }

            return new BillingCreatedModel { InvoicingServiceProductCode = revenueHead.CashFlowProductCode, RevenueHeadId = revenueHead.Id, NextRunDate = billing.NextBillingDate.HasValue ? billing.NextBillingDate.Value.ToString("dd'/'MM'/'yyyy") : "" };
        }



        /// <summary>
        /// Do validation of the billing model from user input
        /// based on the billing type specififed.
        /// <para>if no errors are found, the type implementation creates a billing model for database insert</para>
        /// </summary>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="errors"></param>
        /// <param name="billingHelperModel"></param>
        /// <param name="billing"></param>
        /// <param name="schedule"></param>
        /// <param name="isEdit"></param>
        private void ProcessCreateBillingRequest(MDA mda, RevenueHead revenueHead, ref List<ErrorModel> errors, BillingHelperModel billingHelperModel, ref BillingModel billing, ref BillingSchedule schedule, bool isEdit)
        {
            Logger.Information("Processing billing request");
            foreach (var item in _billingTypeCollection)
            {
                if (billingHelperModel.BillingType == item.BillingType)
                {
                    Logger.Information("Billing type found " + item.BillingType);
                    if (billingHelperModel.AssessmentModel.IsDirectAssessment)
                    {
                        int? payeeId = _revenueHeadCoreService.PayeeId();
                        if (!payeeId.HasValue) { payeeId = 0; }
                        if (payeeId != 0 && payeeId != revenueHead.Id)
                        {
                            Logger.Error("Another paye revenue head already exists");
                            errors.Add(new ErrorModel { ErrorMessage = ErrorLang.payealreadyexists().ToString(), FieldName = "DirectAssessment.AdapterValue" });
                            throw new DirtyFormDataException();
                        }
                        revenueHead.IsPayeAssessment = true;
                    }
                    else if(revenueHead.IsPayeAssessment)
                    {
                        Logger.Error("Revenue head for paye cannot be changed. Please contact Parkway admin.");
                        errors.Add(new ErrorModel { ErrorMessage = ErrorLang.onlyonepayeecanexists().ToString(), FieldName = "DirectAssessment.AdapterValue" });
                        throw new DirtyFormDataException();
                    }
                    item.ValidateModel(billingHelperModel, ref errors);
                    if (errors.Count > 0) { throw new DirtyFormDataException { }; }
                    //create billing model
                    billing = item.CreateBilling(mda, billingHelperModel, ref errors);

                    if (errors.Count > 0) { throw new DirtyFormDataException { }; }
                    schedule = item.CreateSchedule(billing, revenueHead, mda);
                    item.ScheduleJob(billing);
                    return;
                }
            }
            errors.Add(new ErrorModel { ErrorMessage = "No billing type added " + billingHelperModel.BillingType, FieldName = "BillingType" });
            throw new DirtyFormDataException("No billing type added " + billingHelperModel.BillingType);
        }



        private BillingSchedule GetSchedule(RevenueHead revenueHead, BillingHelperModel billingHelperModel, BillingModel newBilling)
        {
            //validate frequency model
            ScheduleHelperModel scheduleHelper = ValidateBillingFrequency(billingHelperModel.BillingFrequencyModel, billingHelperModel.BillingType);
            BillingSchedule schedule;
            //SetDuration(billingHelperModel.BillingFrequencyModel.Duration, newBilling);

            //if the billing type is variable we have no schedule, schedules are to be created per tax payer
            if (billingHelperModel.BillingType == BillingType.Fixed)
            {
                //schedule = new BillingSchedule { StartDateAndTime = scheduleHelper.StartDateAndTime, CronSchedule = scheduleHelper.CronExpression };
                schedule = new BillingSchedule { /*StartDateAndTime = scheduleHelper.StartDateAndTime*/ };

                schedule.MDA = newBilling.Mda;
                schedule.RevenueHead = revenueHead;
                if (revenueHead.IsActive) { schedule.ScheduleStatus = (int)ScheduleStatus.Running; }

                //schedule.NextRunDate = GetNextRunDateForFixedBilling(scheduleHelper.CronExpression, scheduleHelper.StartDateAndTime);
                newBilling.BillingFrequency = JsonConvert.SerializeObject(billingHelperModel.BillingFrequencyModel.FixedBill);
            }
            else
            {
                newBilling.BillingFrequency = JsonConvert.SerializeObject(billingHelperModel.BillingFrequencyModel.VariableBill);
                schedule = null;
            }
            return schedule;
        }


        #endregion


        /// <summary>
        ///Edit billing Amount
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <param name="user"></param>
        public void TryUpdatingBilling(BillingModel billing, RevenueHead revenueHead, UserPartRecord user)
        {
            var alteredBillingModel = JsonConvert.SerializeObject(ConvertBillingToHelperModel(revenueHead.BillingModel));
            _billingRepository.Update(billing);
            //Update audit trail table to store this change
            if (!_auditTrailRepo.Save(new AuditTrail { Model = alteredBillingModel, Source_Id = billing.Id, Type = (int)AuditType.Edit_Billing, AddedBy = user, }))
            {
                throw new CouldNotSaveRecord("Could not save autdit trail for billing record");
            }

        }


        /// <summary>
        /// Convert BillingModel to BillingHelperModel
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>BillingHelperModel</returns>
        /// <exception cref="HasNoBillingException"></exception>
        public BillingHelperModel ConvertBillingToHelperModel(BillingModel billing)
        {
            if (billing == null) { throw new HasNoBillingException(); }
            //get assessment model
            AssessmentModel assessmentModel = GetAssessmentModel(billing);
            //get model for billing freq
            BillingFrequencyModel frequencyModel = GetFrequencyModel(billing);
            frequencyModel.Duration = GetDuration(billing);

            //get billing type
            BillingType billingType = GetBillingType(billing.BillingType);
            //get demand notices
            BillingDemandNotice demandNotices = GetDemandNotice(billing.DemandNotice);
            //get discounts
            ICollection<DiscountModel> discounts = GetDiscountCollection(billing.Discounts);
            //get penalties
            ICollection<PenaltyModel> penalties = GetPenaltyCollection(billing.Penalties);
            //get due date
            DueDateModel dueDate = GetDueDateModel(billing);
            //get duration
            //get direct assessment model
            DirectAssessmentModel directAssessment = GetDirectAssessmentModel(billing);
            //get file upload settings
            FileUploadTemplates fileUploadModel = GetFileUploadModel(billing);

            return new BillingHelperModel
            {
                AssessmentModel = assessmentModel,
                BillingType = billingType,
                Surcharge = billing.Surcharge,
                DemandNotice = demandNotices,
                DiscountCollection = discounts,
                PenaltyCollection = penalties,
                DueDateModel = dueDate,
                DirectAssessment = directAssessment,
                FileUploadTemplates = fileUploadModel,
                BillingFrequencyModel = frequencyModel,
                
            };
        }


        /// <summary>
        /// get file upload settings 
        /// <para>Returns null is not specified</para>
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>FileUploadTemplates | null</returns>
        private FileUploadTemplates GetFileUploadModel(BillingModel billing)
        {
            if (string.IsNullOrEmpty(billing.FileUploadModel)) { return null; }
            try
            {
                return JsonConvert.DeserializeObject<FileUploadTemplates>(billing.FileUploadModel);
            }
            catch (Exception excep)
            {
                Logger.Error(excep, excep.Message);
                throw;
            }
        }


        /// <summary>
        /// Get assessment model
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>AssessmentModel</returns>
        private AssessmentModel GetAssessmentModel(BillingModel billing)
        {
            try
            {
                var assessementModel = JsonConvert.DeserializeObject<AssessmentModel>(billing.Assessment);
                return assessementModel;
            }
            catch (Exception excep)
            {
                Logger.Error(excep, excep.Message);
                return null;
            }
        }


        /// <summary>
        /// Get direct assessment model
        /// <para>if the value is null, null is returned </para>
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>DirectAssessmentModel | null</returns>
        private DirectAssessmentModel GetDirectAssessmentModel(BillingModel billing)
        {
            if (string.IsNullOrEmpty(billing.DirectAssessmentModel)) { return null; }
            try
            {
                return JsonConvert.DeserializeObject<DirectAssessmentModel>(billing.DirectAssessmentModel);
            }
            catch (Exception excep)
            {
                Logger.Error(excep, excep.Message);
                throw;
            }
        }


        /// <summary>
        /// Get model for billing frequency
        /// </summary>
        /// <param name="billing"></param>
        /// <returns></returns>
        private BillingFrequencyModel GetFrequencyModel(BillingModel billing)
        {
            if (billing.BillingFrequency == null) { throw new NoBillingTypeSpecifiedException { }; }
            BillingFrequencyModel value = JsonConvert.DeserializeObject<BillingFrequencyModel>(billing.BillingFrequency);
            if (billing.BillingType == (int)BillingType.Fixed)
            {
                if (value.FixedBill == null) { throw new NoBillingTypeSpecifiedException { }; }
                if (value.FixedBill.DailyBill == null) { value.FixedBill.DailyBill = new DailyBillingModel { }; }
                if (value.FixedBill.WeeklyBill == null) { value.FixedBill.WeeklyBill = new WeeklyBillingModel { }; }
                if (value.FixedBill.MonthlyBill == null) { value.FixedBill.MonthlyBill = new MonthlyBillingModel { }; }
                if(value.FrequencyType == FrequencyType.Yearly) { value.FixedBill.MonthlyBill = value.FixedBill.YearlyBill.MonthlyBill; }
                else if (value.FixedBill.YearlyBill == null) { value.FixedBill.YearlyBill = new YearlyBillingModel { MonthlyBill = new MonthlyBillingModel { } }; }
                
                if (value.VariableBill == null) { value.VariableBill = new VariableBillingModel { }; }
                value.FixedBill.StartFrom = value.FixedBill.StartDateAndTime.ToString("dd'/'MM'/'yyyy");
                value.FixedBill.StartTime = value.FixedBill.StartDateAndTime.ToString("HH:mm");
            }
            else if (billing.BillingType == (int)BillingType.Variable)
            {
                if (value.VariableBill == null) { throw new NoBillingTypeSpecifiedException { }; }
                value.FixedBill = new FixedBillingModel { DailyBill = new DailyBillingModel { }, MonthlyBill = new MonthlyBillingModel { Months = new List<Months> { } }, WeeklyBill = new WeeklyBillingModel { Days = new List<Days> { } }, YearlyBill = new YearlyBillingModel { MonthlyBill = new MonthlyBillingModel { Months = new List<Months> { } } } };
             }
            else
            {
                value = new BillingFrequencyModel { Duration = new DurationModel { }, FixedBill = new FixedBillingModel { DailyBill = new DailyBillingModel { }, MonthlyBill = new MonthlyBillingModel { Months = new List<Months> { } }, WeeklyBill = new WeeklyBillingModel { Days = new List<Days> { } }, YearlyBill = new YearlyBillingModel { MonthlyBill = new MonthlyBillingModel { Months = new List<Months> { } } } } };
            }
            return value;
        }



        private ICollection<PenaltyModel> GetPenaltyCollection(string penalties)
        {
            if (string.IsNullOrEmpty(penalties)) { return new List<PenaltyModel>(); }
            return JsonConvert.DeserializeObject<List<PenaltyModel>>(penalties);
        }

        private ICollection<DiscountModel> GetDiscountCollection(string discounts)
        {
            if (string.IsNullOrEmpty(discounts)) { return new List<DiscountModel>(); }
            return JsonConvert.DeserializeObject<List<DiscountModel>>(discounts);
        }


        /// <summary>
        /// Get demand notices
        /// </summary>
        /// <param name="demandNotice"></param>
        /// <returns></returns>
        private BillingDemandNotice GetDemandNotice(string demandNotice)
        {
            if (string.IsNullOrEmpty(demandNotice)) { return new BillingDemandNotice(); }
            return JsonConvert.DeserializeObject<BillingDemandNotice>(demandNotice);
        }


        #region Create Billing

        /// <summary>
        /// Validate billing model
        /// <para>This validates the demand notice, discounts, penalties, and due date</para>
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <param name="createBillingModel"></param>
        /// <returns>List{ErrorModel}</returns>
        /// <exception cref="AlreadyHasBillingException"></exception>
        /// <exception cref="CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException"></exception>
        /// <exception cref="NoBillingTypeSpecifiedException"></exception>
        /// <exception cref="DateTimeCouldNotBeParsedException"></exception>
        /// <exception cref="NoFrequencyTypeFoundException"></exception>
        public List<ErrorModel> ValidateBillingModel(BillingHelperModel createBillingModel)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            Logger.Information("Validating demand notice");
            //validate demand notice
            ValidatedDemandNoticeModel(createBillingModel.DemandNotice, ref errors);
            Logger.Information("Validating discount model");
            //validate discount model
            ValidateDiscountCollection(createBillingModel.DiscountCollection, ref errors);
            Logger.Information("Validating penalty model");
            //validate penalty model
            ValidatePenaltyCollection(createBillingModel.PenaltyCollection, ref errors);
            Logger.Information("Validating due date model");
            //validate due date
            ValidateDueDate(createBillingModel.DueDateModel, createBillingModel.AssessmentModel.IsRecurring, ref errors);
            return errors;
        }


        /// <summary>
        /// Save billing and schedule
        /// </summary>
        /// <param name="newBilling"></param>
        /// <param name="schedule"></param>
        private void SaveBillingAndSchedule(BillingModel newBilling, BillingSchedule schedule, APIRequest requestLog = null)
        {
            Logger.Information("Saving billing");
            if (!_billingRepository.Save(newBilling)) { throw new CouldNotSaveBillingException("Cannot save billing information"); }
            Logger.Information("Saving schedule");
            if (schedule != null) { if (!_scheduleRepository.Save(schedule)) { throw new CouldNotSaveBillingException("Cannot save billing information"); } }
            Logger.Information("saving API response");
            if (requestLog != null) { if (!_apiRequestRepository.Save(requestLog)) { throw new CouldNotSaveBillingException("Cannot save billing information"); } }
            Logger.Information("Billing Saved");
        }


        private CashFlowProduct CreateProductsOnCashflow(RevenueHead revenueHead, BillingModel billing, string SMEKey)
        {
            try
            {
                Logger.Error("Setting product on cashflow");
                var key = SMEKey;
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", key } });
                var productService = _invoicingService.ProductServices(context);
                #endregion
                if (string.IsNullOrEmpty(key))
                {
                    Logger.Information("No SMKEY found. Cannot create billing for revenue head, MDA does not exist on cashflow");
                    throw new CouldNotSaveBillingException("No SMKEY found. MDA does not have a cashflow account");
                }
                else
                {
                    var postParameters = new CashFlowCreateProduct
                    {
                        Name = revenueHead.NameAndCode(),
                        ProductCode = revenueHead.Code + "/" + revenueHead.Id,
                        Price = billing.Amount,
                        Description = string.Format("{0} Assessment", revenueHead.NameAndCode())
                    };
                    return productService.CreateProduct(postParameters);
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Error Occured : {0}", exception.Message);
                throw new CouldNotSaveBillingException(exception.Message, exception);
            }
        }

        private CashFlowProduct UpdateProductsOnCashflow(RevenueHead revenueHead, BillingModel billing, string SMEKey)
        {
            try
            {
                Logger.Error("Updating product on cashflow");
                var key = SMEKey;
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", key } });
                var productService = _invoicingService.ProductServices(context);
                #endregion
                if (string.IsNullOrEmpty(key))
                {
                    Logger.Information("No SMKEY found. Cannot update billing for revenue head, MDA does not exist on cashflow");
                    throw new CouldNotSaveBillingException("No SMKEY found. MDA does not have a cashflow account");
                }
                else
                {
                    var postParameters = new CashFlowEditProduct
                    {
                        Product = new CashFlowCreateProduct
                        {
                            Name = revenueHead.NameAndCode(),
                            ProductCode = revenueHead.Code + "/" + revenueHead.Id,
                            Price = billing.Amount,
                            Description = string.Format("{0} Assessment", revenueHead.NameAndCode())
                        },
                        ProductId = revenueHead.CashFlowProductId
                    };
                    return productService.EditProduct(postParameters);
                }
            }
            catch (Exception exception)
            {
                Logger.Error("Error Occured : {0}", exception.Message);
                throw new CouldNotSaveBillingException(exception.Message, exception);
            }
        }

        private void ValidateDueDate(DueDateModel dueDateModel, bool isRecurring, ref List<ErrorModel> errors)
        {
            if (dueDateModel == null)
            {
                errors.Add(new ErrorModel { FieldName = "DueDateModel.DueDateInterval", ErrorMessage = ErrorLang.invalidduedate().ToString() });
                return;
            }

            if (dueDateModel.DueOnNextBillingDate && !isRecurring)
            {
                errors.Add(new ErrorModel { FieldName = "DueDateModel.DueOnNextBillingDate", ErrorMessage = ErrorLang.nextbilldateforduedateisonlyvalidforrecurringbills().ToString() });
            }

            if (dueDateModel.DueDateInterval <= 0 && !dueDateModel.DueOnNextBillingDate)
            {
                errors.Add(new ErrorModel { FieldName = "DueDateModel.DueDateInterval", ErrorMessage = ErrorLang.invalidduedate().ToString() });
            }

            if (dueDateModel.DueDateAfter == DueDateAfter.None && !dueDateModel.DueOnNextBillingDate)
            {
                errors.Add(new ErrorModel { FieldName = "DueDateModel.DueDateAfter", ErrorMessage = ErrorLang.duedatetype404().ToString() });
            }
        }

        private void ValidatePenaltyCollection(ICollection<PenaltyModel> penaltyCollection, ref List<ErrorModel> errors)
        {
            if (penaltyCollection == null || penaltyCollection.Count <= 0) { return; }
            int counter = 0;
            foreach (var penalty in penaltyCollection)
            {
                if (penalty.PenaltyValueType == PenaltyValueType.None)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].PenaltyValueType", ErrorMessage = ErrorLang.penalty404().ToString() });
                }

                if (penalty.Value <= 0)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].Value", ErrorMessage = ErrorLang.penaltybadvalue().ToString() });
                }

                if (penalty.EffectiveFromType == EffectiveFromType.None)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].EffectiveFromType", ErrorMessage = ErrorLang.penalty404().ToString() });
                }

                if (penalty.EffectiveFrom <= 0)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].EffectiveFrom", ErrorMessage = ErrorLang.penaltybadvalue().ToString() });
                }
                counter++;
            }
        }

        private void ValidateDiscountCollection(ICollection<DiscountModel> discountCollection, ref List<ErrorModel> errors)
        {
            if (discountCollection == null || discountCollection.Count <= 0) { return; }

            int counter = 0;
            foreach (var discount in discountCollection)
            {
                if (discount.BillingDiscountType == BillingDiscountType.None)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].BillingDiscountType", ErrorMessage = counter + " Billing discount type could not be found." });
                }

                if (discount.EffectiveFromType == EffectiveFromType.None)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].EffectiveFrom", ErrorMessage = counter + " Range type for this discount could not be found. Select a valid range type." });
                }

                if (discount.Discount <= 0)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].Discount", ErrorMessage = counter + " Discount must be a valid number." });
                }
                if (discount.EffectiveFrom <= 0)
                {
                    errors.Add(new ErrorModel { FieldName = "[" + counter + "].EffectiveFrom", ErrorMessage = counter + " Effective from for discount must be a number greater than 0." });
                }
                counter++;
            }
        }


        /// <summary>
        /// Validate demand notice model
        /// </summary>
        /// <param name="demandNotice"></param>
        /// <param name="errors"></param>
        private void ValidatedDemandNoticeModel(BillingDemandNotice demandNotice, ref List<ErrorModel> errors)
        {
            if (demandNotice == null || !demandNotice.IsChecked) { return; }

            if (demandNotice.EffectiveFrom <= 0 && demandNotice.IsChecked)
            {
                errors.Add(new ErrorModel { FieldName = "BillingDemandNotice.EffectiveFrom", ErrorMessage = ErrorLang.demandnoticefrom().ToString() });
            }

            if (demandNotice.EffectiveFromType == EffectiveFromType.None && demandNotice.IsChecked)
            {
                errors.Add(new ErrorModel { FieldName = "BillingDemandNotice.EffectiveFromType", ErrorMessage = ErrorLang.demandnoticetype().ToString() });
            }
        }


        /// <summary>
        /// Validate billing frequency
        /// </summary>
        /// <param name="model">BillingFrequencyModel</param>
        /// <param name="billingType">BillingType</param>
        /// <exception cref="NoBillingTypeSpecifiedException"></exception>
        private ScheduleHelperModel ValidateBillingFrequency(BillingFrequencyModel model, BillingType billingType)
        {
            //validate based on billing type
            if (billingType == BillingType.Fixed)
            {
                return ValidateFixedBillingType(model.FixedBill, model.FrequencyType);
            }
            else if (billingType == BillingType.Variable)
            {
                return ValidateVariableBillingType(model.VariableBill, model.FrequencyType);
            }
            else { throw new NoBillingTypeSpecifiedException(); }
        }


        /// <summary>
        /// Validate a fixed billing type
        /// </summary>
        /// <param name="model">BillingFrequencyModel</param>
        /// <exception cref="DateTimeCouldNotBeParsedException"></exception>
        /// <exception cref="NoFrequencyTypeFoundException"></exception>
        private ScheduleHelperModel ValidateFixedBillingType(FixedBillingModel model, FrequencyType frequencyType)
        {
            ScheduleHelperModel scheduleHelper = new ScheduleHelperModel();
            try
            {
                model.StartDateAndTime = DateTime.ParseExact(model.StartFrom.Trim() + " " + model.StartTime.Trim(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                //validate start date
                if (DateTime.Now >= model.StartDateAndTime)
                {
                    throw new StartDateHasPassedException();
                }
            }
            catch (Exception exception)
            {
                if (exception.GetType() == typeof(StartDateHasPassedException))
                {
                    Logger.Error(ErrorLang.datefrompast().ToString());
                    throw new StartDateHasPassedException(ErrorLang.datefrompast().ToString());
                }
                Logger.Error(exception, string.Format("Date and time could not be parsed. Date - {0} : Time - {1}", model.StartFrom, model.StartTime));
                throw new DateTimeCouldNotBeParsedException();
            }
            scheduleHelper.StartDateAndTime = model.StartDateAndTime;
            foreach (var freq in _billingFrequencyFilter)
            {
                if (freq.Frequency() == frequencyType)
                {
                    freq.ValidateFixed(model);
                    //if no error exception thrown, get the cron expression
                    scheduleHelper.CronExpression = freq.GetCronExpression(model, model.StartDateAndTime);
                    return scheduleHelper;
                }
            }
            throw new NoFrequencyTypeFoundException();
        }

        private ScheduleHelperModel ValidateVariableBillingType(VariableBillingModel model, FrequencyType frequencyType)
        {
            foreach (var freq in _billingFrequencyFilter)
            {
                if (freq.Frequency() == frequencyType)
                {
                    freq.ValidateVariable(model);
                    return null;
                }
            }
            throw new NoFrequencyTypeFoundException();
        }

        #endregion


        /// <summary>
        /// Check if the revenue head has any sub revenue head(s).
        /// <para>If the given revenue head has any sub revenue head(s) it cannot start a form setup process</para>
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>CoreBillingService</returns>
        /// <exception cref="CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException"></exception>
        public CoreBillingService HasSubRevenueHeads(RevenueHead revenueHead)
        {
            if (revenueHead.RevenueHeads.Any()) { throw new CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException(); }
            return this;
        }

        /// <summary>
        /// Get user
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private UserPartRecord GetUser(string userEmail)
        {
            var user = _biilingModelRepository.User(userEmail);
            if (user == null) { throw new AuthorizedUserNotFoundException(string.Format("Authorized user not found {0}", userEmail)); }
            return user;
        }


        /// <summary>
        /// Check if the revenue head already has billing info
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>CoreBillingService</returns>
        /// <exception cref="AlreadyHasBillingException"></exception>
        public CoreBillingService HasBilling(RevenueHead revenueHead)
        {
            if (revenueHead.BillingModel != null)
            {
                throw new AlreadyHasBillingException("Revenue head already has billing info. Revenue head TaxEntityId " + revenueHead.Id);
            }
            return this;
        }

        /// <summary>
        /// Get revenue head with the given details
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>RevenueHead</returns>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public RevenueHead GetRevenueHead(int revenueHeadId, string revenueHeadSlug)
        {
            return _revenueHeadCoreService.GetRevenueHead(revenueHeadSlug, revenueHeadId);
        }

        /// <summary>
        /// Checks if biling is allowed.
        /// <para>Checks if the revenue head & mda are active. If these dependencies are active return the duration model</para>
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        public bool IsBillingAllowed(MDA mda, RevenueHead revenueHead)
        {
            if (!revenueHead.IsActive) { return false; }
            if (!mda.IsActive) { return false; }
            return true;
        }


        /// <summary>
        /// Get billing duration
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>DurationModel</returns>
        public DurationModel GetDuration(BillingModel billing)
        {
            try
            {
                if (string.IsNullOrEmpty(billing.Duration)) { return new DurationModel { }; }
                DurationModel value = JsonConvert.DeserializeObject<DurationModel>(billing.Duration);
                if(value.DurationType == DurationType.EndsOn) { value.EndsAtDate = value.EndsDate.Value.ToString("dd'/'MM'/'yyyy"); }
                return value;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Get due date
        /// </summary>
        /// <param name="dueDateModel">DueDateModel</param>
        /// <param name="startTime">DateTime</param>
        /// <returns>DateTime</returns>
        /// <exception cref="NoDueDateTypeFoundException"></exception>
        public DateTime GetDueDate(DueDateModel dueDateModel, DateTime startTime)
        {
            switch (dueDateModel.DueDateAfter)
            {
                case DueDateAfter.Days:
                    return startTime.AddDays(dueDateModel.DueDateInterval);
                case DueDateAfter.Weeks:
                    return startTime.AddDays(7 * dueDateModel.DueDateInterval);
                case DueDateAfter.Months:
                    return startTime.AddMonths(dueDateModel.DueDateInterval);
                case DueDateAfter.Years:
                    return startTime.AddYears(dueDateModel.DueDateInterval);
                default:
                    throw new NoDueDateTypeFoundException("No due date found for billing ");
            }
        }

        /// <summary>
        /// Get due date model
        /// </summary>
        /// <param name="billing">BillingModel</param>
        /// <returns>DueDateModel</returns>
        public DueDateModel GetDueDateModel(BillingModel billing)
        {
            if (string.IsNullOrEmpty(billing.DueDate)) { return new DueDateModel(); }
            return JsonConvert.DeserializeObject<DueDateModel>(billing.DueDate);
        }


        /// <summary>
        /// Get the Next billing date. Next date for the billing cycle
        /// </summary>
        /// <param name="billing"></param>
        /// <param name="billingType"></param>
        /// <returns>DateTime</returns>
        public DateTime GetNextBillingDate(BillingModel billing, BillingType billingType, DateTime invoiceDate)
        {
            AssessmentModel assessment = JsonConvert.DeserializeObject<AssessmentModel>(billing.Assessment);
            //check if the billing is one off
            if (BillingType.OneOff == billingType)
            {
                return billing.NextBillingDate.Value;
            }

            if (BillingType.DirectAssessment == billingType)
            {
                return new DateTime(DateTime.Now.ToLocalTime().Year, 1, 1).AddYears(1);
            }

            if (BillingType.Fixed == billingType)
            {
                return billing.NextBillingDate.Value;
            }

            if (BillingType.Variable == billingType)
            {
                return GetNextRunDateForVariableBilling(billing, invoiceDate);
            }
            throw new NoBillingTypeSpecifiedException(string.Format("No billing type specificed type found for {0}", billing.BillingType));
        }


        /// <summary>
        /// Get the next run date for this variable billing
        /// </summary>
        /// <param name="billing">billing model</param>
        /// <param name="startDate">invoice date, most likely the start date</param>
        /// <returns>DateTime</returns>
        private DateTime GetNextRunDateForVariableBilling(BillingModel billing, DateTime startDate)
        {
            BillingFrequencyModel billingFrequency = JsonConvert.DeserializeObject<BillingFrequencyModel>(billing.BillingFrequency);
            if (billingFrequency.VariableBill == null) { throw new NoFrequencyTypeFoundException(); }
            switch (billingFrequency.FrequencyType)
            {
                case FrequencyType.Daily:
                    return startDate.AddDays(billingFrequency.VariableBill.Interval);
                case FrequencyType.Weekly:
                    return startDate.AddDays(7 * billingFrequency.VariableBill.Interval);
                case FrequencyType.Monthly:
                    return startDate.AddMonths(billingFrequency.VariableBill.Interval);
                case FrequencyType.Yearly:
                    return startDate.AddYears(billingFrequency.VariableBill.Interval);
                default:
                    throw new NoFrequencyTypeFoundException("No frequency type found");
            }
            throw new NoFrequencyTypeFoundException("No frequency type found");
        }


        /// <summary>
        /// Get Billing type
        /// </summary>
        /// <param name="billingType"></param>
        /// <returns>BillingType</returns>
        /// <exception cref="NoBillingTypeSpecifiedException">If billing type is not found</exception>
        public BillingType GetBillingType(int billingType)
        {
            if (!typeof(BillingType).IsEnumDefined(billingType)) { throw new NoBillingTypeSpecifiedException(string.Format("No billing type specificed billing id {0} ", billingType)); }
            return (BillingType)billingType;
        }


        /// <summary>
        /// Has sub revenue heads
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        bool ICoreBillingService.HasSubRevenueHeads(RevenueHead revenueHead)
        {
            if (revenueHead.RevenueHeads != null && revenueHead.RevenueHeads.Count() > 0) { return true; }
            return false;
        }


        /// <summary>
        /// Has billing 
        /// </summary>
        /// <param name="revenueHead"></param>
        /// <returns>bool</returns>
        bool ICoreBillingService.HasBilling(RevenueHead revenueHead)
        {
            if (revenueHead.BillingModel != null && revenueHead.BillingModel.Id != 0) { return true; }
            return false;
        }


        #region Billing utils

        /// <summary>
        /// Get amount for this billing
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>decimal</returns>
        public decimal GetBillingAmount(BillingModel billing, decimal amount)
        {
            AssessmentModel assessment = JsonConvert.DeserializeObject<AssessmentModel>(billing.Assessment);
            if (assessment.IsDirectAssessment) { return amount != 0.00m ? amount : assessment.Amount; }
            return assessment.Amount;
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
                    discountConcat = @"<p> <b>Discounts:</b></p> <ul>";
                    foreach (var item in discounts)
                    {
                        var rate = item.BillingDiscountType == BillingDiscountType.Flat ? "Naira flat rate" : "% percent";
                        discountConcat += string.Format("<li> {0} {1} discount is applicable {2} {3} after invoice generation </li>", item.Discount, rate, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                    discountConcat += @"</ul>";
                }

            }

            if (!string.IsNullOrEmpty(billing.Penalties))
            {
                List<PenaltyModel> penalties = JsonConvert.DeserializeObject<List<PenaltyModel>>(billing.Penalties);
                if (penalties.Any())
                {
                    penaltyConcat = "<p><b>Penalties:</b></p> <ul>";
                    foreach (var item in penalties)
                    {
                        var rate = item.PenaltyValueType == PenaltyValueType.FlatRate ? "Naira flat rate" : "% percent";
                        penaltyConcat += string.Format("<li> {0} {1} penalty is applicable {2} {3} after due date </li>", item.Value, item.PenaltyValueType, item.EffectiveFrom, item.EffectiveFromType.ToString().ToLower());
                    }
                    penaltyConcat += @"</ul>";
                }
            }
            return discountConcat + penaltyConcat;
        }

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
        /// Get due date for the invoice
        /// </summary>
        /// <param name="billing">Billing model</param>
        /// <param name="invoiceDate">this is the invoice date</param>
        /// <param name="nextBillingDate">this is the next billing date. If the invoice is due on the next billing date, this value is returned</param>
        /// <returns>Datetime</returns>
        public DateTime GetDueDate(BillingModel billing, DateTime invoiceDate, DateTime nextBillingDate)
        {
            Logger.Information("Getting due date");
            if (string.IsNullOrEmpty(billing.DueDate)) { Logger.Error("Null due date found"); throw new NoDueDateTypeFoundException("No due date found for billing " + billing.Id); }

            DueDateModel dueDate = JsonConvert.DeserializeObject<DueDateModel>(billing.DueDate);
            if (dueDate == null) { Logger.Error("Null due date found"); throw new NoDueDateTypeFoundException("No due date found for billing. Due date is null for billing Id " + billing.Id); }
            AssessmentModel assessment = JsonConvert.DeserializeObject<AssessmentModel>(billing.Assessment);
            if (dueDate.DueOnNextBillingDate && assessment.IsRecurring) { return nextBillingDate; }
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
        /// Get the list of registered adapters for direct assessments
        /// </summary>
        /// <returns>List{AssessmentInterface}</returns>
        public List<AssessmentInterface> GetAssessmentAdapters()
        {
            Logger.Information("Getting tenant state");
            TenantCBSSettings tenantIdentifier = null;
            try
            { tenantIdentifier = _coreSettingsService.HasTenantStateSettings(); }
            catch (Exception exception)
            {
                Logger.Error(exception, "No tenant identifier found");
                throw new CannotFindTenantIdentifierException(exception.Message + " No tenant identifier found " + exception.StackTrace);
            }
            if (tenantIdentifier == null) { throw new CannotFindTenantIdentifierException("No tenant identifier found. Value is null."); }
            //get list of direct assessment adapters
            Logger.Information("Getting list of direct assessment adapters attached to the state");
            return GetListOfAssessmentAdapters(tenantIdentifier.Identifier);
        }


        /// <summary>
        /// Get a list of file upload templates
        /// </summary>
        /// <returns>List{Template}</returns>
        public List<Template> GetListOfFileUploadTemplates()
        {
            //get path to app xml file
            return _fileUploadConfig.Templates(Utilities.Util.GetAppXMLFilePath());
        }


        /// <summary>
        /// Get list of adapters for direct assessment
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>List{AssessmentInterface}</returns>
        private List<AssessmentInterface> GetListOfAssessmentAdapters(string identifier)
        {
            return _payeeConfig.GetAssessmentTypes(identifier);
        }

        //public DateTime GetDueDate(DueDateModel dueDateModel, DateTime startTime)
        //{
        //    switch (dueDateModel.DueDateAfter)
        //    {
        //        case DueDateAfter.Days:
        //            return startTime.AddDays(dueDateModel.DueDateInterval);
        //        case DueDateAfter.Weeks:
        //            return startTime.AddDays(7 * dueDateModel.DueDateInterval);
        //        case DueDateAfter.Months:
        //            return startTime.AddMonths(dueDateModel.DueDateInterval);
        //        case DueDateAfter.Years:
        //            return startTime.AddYears(dueDateModel.DueDateInterval);
        //        default:
        //            throw new NoDueDateTypeFoundException("No due date found for billing ");
        //    }
        //}
        #endregion

    }
}