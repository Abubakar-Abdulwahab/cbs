using Orchard;
using System.Linq;
using Orchard.Logging;
using Newtonsoft.Json;
using Orchard.Users.Models;
using Orchard.Localization;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Payee;
using Parkway.CBS.FileUpload;
using Parkway.CBS.Core.Models;
using Orchard.Modules.Services;
using Orchard.FileSystems.Media;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Module.ViewModels;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;



namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class BillingHandler : BaseHandler, IBillingHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IRevenueHeadHandler _revenueHeadHandler;
        private readonly ICoreRevenueHeadService _corerevenueHeadHandler;
        private readonly ICoreBillingService _billingService;
        private readonly ICoreRevenueHeadService _coreRevenueHeadService;
        public IInvoicingService _invoicingService;
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        public Localizer T { get; set; }
        public readonly IPayeeAssessmentConfiguration _payeeConfig;
        private readonly ICoreSettingsService _coreSettingsService;

        public BillingHandler(IOrchardServices orchardServices, IRevenueHeadHandler revenueHeadHandler, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IInvoicingService invoicingService, ICoreRevenueHeadService coreRevenueHeadHandler, IModuleService moduleService, ICoreBillingService billingService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ICoreRevenueHeadService coreRevenueHeadService, IPayeeAssessmentConfiguration payeeConfig, ICoreSettingsService coreSettingsService) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            T = NullLocalizer.Instance;
            _revenueHeadHandler = revenueHeadHandler;
            _invoicingService = invoicingService;
            _corerevenueHeadHandler = coreRevenueHeadHandler;
            _billingService = billingService;
            _settingsRepository = settingsRepository;
            _coreRevenueHeadService = coreRevenueHeadService;
            _payeeConfig = payeeConfig;
            _coreSettingsService = coreSettingsService;
        }

        #region create billing ops

        /// <summary>
        /// Get view to create a billing record
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>BillingViewModel</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CannotFindRevenueHeadException"></exception>
        public BillingViewModel GetCreateBillingView(int revenueHeadId, string revenueHeadSlug)
        {
            IsAuthorized<BillingHandler>(Permissions.CreateBilling);
            var revenueHead = _billingService.GetRevenueHead(revenueHeadId, revenueHeadSlug);

            if (_billingService.HasSubRevenueHeads(revenueHead)) { throw new CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException("This revenue head already has sub-revenue heads " + revenueHead.Id); }
            if (_billingService.HasBilling(revenueHead)) { throw new AlreadyHasBillingException("Cannot create billing for Revenue head because it already has billing info " + revenueHead.Id); }
            //get tenant
            Logger.Information("Getting list of ");
            List<Core.Payee.AssessmentInterface> assessmentAdapters = GetAssessmentInterfaces();
            //get collection of direct assessments
            // O_O
            return new BillingViewModel
            {
                RHName = revenueHead.NameAndCode(),
                FrequencyModel = new BillingFrequencyModel { Duration = new DurationModel { }, FixedBill = new FixedBillingModel { DailyBill = new DailyBillingModel { }, MonthlyBill = new MonthlyBillingModel { Months = new List<Months> { } }, WeeklyBill = new WeeklyBillingModel { Days = new List<Days> { } }, YearlyBill = new YearlyBillingModel { MonthlyBill = new MonthlyBillingModel { Months = new List<Months> { } } } } },
                DiscountModel = new DiscountModel(),
                Indexer = 0,
                DiscountModelPostBackData = new List<DiscountModel>(),
                BillingDemandNotice = new BillingDemandNotice { },
                PenaltyModel = new PenaltyModel(),
                PenaltyModelPostBackData = new List<PenaltyModel>(),
                DueDateModel = new DueDateModel { },
                CallBackObject = new CallBackObject { },
                DirectAssessment = new DirectAssessmentVM { AllowFileUpload = false, ListOfAssessmentInterface = assessmentAdapters },
                FileUploadBillingModel = GetFileUploadTemplatesVM(),
            };
        }


        /// <summary>
        /// Get file upload templates view model
        /// <para>Returns a list full or an empty list never a null value</para>
        /// </summary>
        /// <returns>FileUploadTemplatesVM</returns>
        public FileUploadTemplatesVM GetFileUploadTemplatesVM()
        {
            var templates = GetFileUploadTemplates();
            FileUploadTemplatesVM fileuploadVM = new FileUploadTemplatesVM { ListOfTemplates = new List<TemplateVM> { } };

            if (templates != null && templates.Any())
            {
                fileuploadVM = new FileUploadTemplatesVM { ListOfTemplates = templates.Select(templ => new TemplateVM { Name = templ.Name, ListOfUploadImplementations = templ.ListOfUploadImplementations.Select(impl => new UploadImplInterfaceVM { Name = impl.Name, Value = impl.Value }).ToList() }).ToList() };
            }
            return fileuploadVM;
        }


        protected List<Template> GetFileUploadTemplates()
        {
            return _billingService.GetListOfFileUploadTemplates();
        }


        /// <summary>
        /// Get list of assessment interfaces
        /// </summary>
        /// <returns>List{AssessmentInterface}</returns>
        public List<AssessmentInterface> GetAssessmentInterfaces()
        {
            return _billingService.GetAssessmentAdapters();
        }


        /// <summary>
        /// create billing info
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="frequencyModel"></param>
        /// <param name="discountModel"></param>
        /// <param name="penaltyModel"></param>
        /// <exception cref="AlreadyHasBillingException"></exception>
        /// <exception cref="CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException"></exception>
        /// <exception cref="NoBillingTypeSpecifiedException"></exception>
        /// <exception cref="DateTimeCouldNotBeParsedException"></exception>
        /// <exception cref="NoFrequencyTypeFoundException"></exception>
        public void TryPostBillingDetails(BillingController callback, BillingViewModel model, string revenueHeadSlug, int revenueHeadId, BillingFrequencyModel frequencyModel, ICollection<DiscountModel> discountModel, ICollection<PenaltyModel> penaltyModel, List<AssessmentInterface> directAssessmentAdapters, bool isEdit)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            Logger.Information("Process billing record (is edit)" + isEdit.ToString());
            Logger.Information("Model " + JsonConvert.SerializeObject(model));
            try
            {
                Logger.Information("Checking permissions");
                IsAuthorized<BillingHandler>(Permissions.CreateBilling);
                Logger.Information("getting revenue head");
                var revenueHead = _billingService.GetRevenueHead(revenueHeadId, revenueHeadSlug);

                Logger.Information("Validating amount");
                //validate amount
                decimal amount = 0.0m;
                string amountValueInString = model.SAmount != null ? model.SAmount.Replace(",", "") : "0";
                bool parsed = decimal.TryParse(amountValueInString, out amount);
                if (!parsed)
                {
                    callback.ModelState.AddModelError("SAmount", "Only numerical values are accepted for the amount");
                    throw new DirtyFormDataException();
                }

                decimal surcharge = 0.0m;
                string surchargeValueInString = !string.IsNullOrEmpty(model.Surcharge) ? model.Surcharge.Replace(",", "") : "0";
                parsed = decimal.TryParse(surchargeValueInString, out surcharge);
                if (!parsed)
                {
                    callback.ModelState.AddModelError("Surcharge", "Only numerical values are accepted for the surcharge");
                    throw new DirtyFormDataException();
                }

                //based off the pre-historic implementation
                //lets check what type of direct assessment this billing info is for
                //if the model.IsDirectAssessment and requires file upload, lets leave the IsDirectAssessment
                //if the direct assessment does not require file upload lets set the billing type to self assessment and set the isDirectAssessment to false
                DirectAssessmentModel directAssessmentModel = new DirectAssessmentModel { };
                if(model.BillingType == BillingType.None)
                {
                    throw new NoBillingTypeSpecifiedException { };
                }

                if (model.BillingType == BillingType.DirectAssessment)
                {
                    if (model.DirectAssessment == null || !model.DirectAssessment.AllowFileUpload)
                    {
                        model.BillingType = BillingType.SelfAssessment;
                    }
                    else
                    {
                        directAssessmentModel.AllowFileUpload = model.DirectAssessment.AllowFileUpload;
                        directAssessmentModel.AdapterValue = model.DirectAssessment.AdapterValue;
                        directAssessmentModel.DirectAssessmentAdapters = directAssessmentAdapters;
                        model.IsDirectAssessment = true;
                    }
                }

                //lets create assessment
                AssessmentModel assessmentModel = new AssessmentModel
                {
                    Amount = amount,
                    IsDirectAssessment = model.IsDirectAssessment,
                    IsRecurring = model.IsRecurring,
                };

                BillingFrequencyModel billingFrequencyModel = new BillingFrequencyModel();
                FileUploadTemplates fileUploadModel = new FileUploadTemplates { };

                if (model.BillingType == BillingType.FileUpload)
                {
                    fileUploadModel = new FileUploadTemplates { SelectedTemplate = model.FileUploadBillingModel.SelectedTemplate, SelectedImplementation = model.FileUploadBillingModel.SelectedImplementation };
                    fileUploadModel.ListOfTemplates = GetFileUploadTemplates();
                }

                if (assessmentModel.IsRecurring)
                {
                    billingFrequencyModel = GetBillingFrequencyModel(model.BillingType, frequencyModel);
                }


                BillingHelperModel createBillingModel = new BillingHelperModel
                {
                    AssessmentModel = assessmentModel,
                    Surcharge = surcharge,
                    BillingType = model.BillingType,
                    BillingFrequencyModel = billingFrequencyModel,
                    DiscountCollection = discountModel,
                    PenaltyCollection = penaltyModel,
                    DemandNotice = model.BillingDemandNotice,
                    DueDateModel = model.DueDateModel,
                    DirectAssessment = directAssessmentModel,
                    FileUploadTemplates = fileUploadModel,
                    RevenueHeadID = revenueHead.Id,
                };

                Logger.Information("Model loaded up, getting mda and user");
                MDA mda = revenueHead.Mda;
                if (mda == null) { throw new MDARecordNotFoundException(); }
                UserPartRecord user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information("Validating model");

                BillingCreatedModel response = _billingService.TryPostBillingForCollection(mda, revenueHead, user, ref errors, createBillingModel, isEdit);

            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<BillingHandler, BillingController>(callback, errors);
            }
            catch (NoBillingTypeSpecifiedException)
            {
                AddValidationErrorsToCallback<BillingHandler, BillingController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = "No Billing type found.", FieldName = "IsRecurring" } } });
            }
            catch (NoFrequencyTypeFoundException exception)
            {
                Logger.Error(exception, exception.Message);
                AddValidationErrorsToCallback<BillingHandler, BillingController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.notfrequencytypefound().ToString(), FieldName = "FrequencyType" } } });
            }
            catch (BillingDurationException exception)
            {
                Logger.Error(exception, exception.Message);
                AddValidationErrorsToCallback<BillingHandler, BillingController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.billingdurationexception(exception.Message).ToString(), FieldName = "IsRecurring" } } });
            }
            catch (BillingFrequencyException exception)
            {
                errors.Add(new ErrorModel { ErrorMessage = exception.Message, FieldName = "BillingType" });
                AddValidationErrorsToCallback<BillingHandler, BillingController>(callback, errors);
            }
        }

        //Edit billing update method
        public bool TryUpdateBillingDetails(BillingController callback, BillingViewModel model, string revenueHeadSlug, int revenueHeadId, BillingFrequencyModel frequencyModel, ICollection<DiscountModel> discountModel, ICollection<PenaltyModel> penaltyModel, List<AssessmentInterface> directAssessmentAdapters, bool isEdit, out bool isSuccess)
        {
            TryPostBillingDetails(callback, model, revenueHeadSlug, revenueHeadId, frequencyModel, discountModel, penaltyModel, directAssessmentAdapters, isEdit);
            isSuccess = false;
            return isSuccess;
        }


        private BillingFrequencyModel GetBillingFrequencyModel(BillingType billingType, BillingFrequencyModel frequencyModel)
        {
            if (billingType == BillingType.Fixed)
            {
                return new BillingFrequencyModel { FrequencyType = frequencyModel.FrequencyType, Duration = frequencyModel.Duration, FixedBill = GetFixedModel(frequencyModel) };
            }
            else if (billingType == BillingType.Variable)
            {
                return new BillingFrequencyModel { FrequencyType = frequencyModel.FrequencyType, Duration = frequencyModel.Duration, VariableBill = GetVaribaleModel(frequencyModel) };
            }
            throw new NoBillingTypeSpecifiedException("No billing type found");
        }


        /// <summary>
        /// Get variable billing model
        /// </summary>
        /// <param name="frequencyModel"></param>
        /// <returns>VariableBillingModel</returns>
        private VariableBillingModel GetVaribaleModel(BillingFrequencyModel frequencyModel)
        {
            return new VariableBillingModel { Interval = frequencyModel.VariableBill != null ? frequencyModel.VariableBill.Interval : 0 };
        }


        /// <summary>
        /// Get fixed billing model
        /// </summary>
        /// <param name="frequencyModel"></param>
        /// <returns>FixedBillingModel</returns>
        private FixedBillingModel GetFixedModel(BillingFrequencyModel frequencyModel)
        {
            if (frequencyModel.FrequencyType == FrequencyType.Daily)
            {
                return new FixedBillingModel { DailyBill = frequencyModel.FixedBill.DailyBill, StartFrom = frequencyModel.FixedBill.StartFrom, StartTime = frequencyModel.FixedBill.StartTime };
            }
            else if (frequencyModel.FrequencyType == FrequencyType.Weekly)
            {
                return new FixedBillingModel { WeeklyBill = frequencyModel.FixedBill.WeeklyBill, StartFrom = frequencyModel.FixedBill.StartFrom, StartTime = frequencyModel.FixedBill.StartTime };
            }
            else if (frequencyModel.FrequencyType == FrequencyType.Monthly)
            {
                return new FixedBillingModel { MonthlyBill = frequencyModel.FixedBill.MonthlyBill, StartFrom = frequencyModel.FixedBill.StartFrom, StartTime = frequencyModel.FixedBill.StartTime };
            }
            else if (frequencyModel.FrequencyType == FrequencyType.Yearly)
            {
                return new FixedBillingModel { YearlyBill = new YearlyBillingModel { MonthlyBill = frequencyModel.FixedBill.MonthlyBill, NumberOfYears = frequencyModel.FixedBill.YearlyBill.NumberOfYears }, StartFrom = frequencyModel.FixedBill.StartFrom, StartTime = frequencyModel.FixedBill.StartTime };
            }
            throw new NoFrequencyTypeFoundException("No billing type found");
        }

        #endregion

        #region Edit ops

        /// <summary>
        /// Check if the billing is valid
        /// </summary>
        /// <param name="billing"></param>
        /// <returns>bool</returns>
        private bool HasBilling(BillingModel billing)
        {
            if (billing != null && billing.Id != 0) { return true; }
            return false;
        }

        private bool HasSubRevenueHeads(RevenueHead revenueHead)
        {
            return _coreRevenueHeadService.HasSubRevenueHeads(revenueHead);
        }


        private RevenueHead GetRevenueHead(int revenueHeadId, string revenueHeadSlug)
        {
            return _coreRevenueHeadService.GetRevenueHead(revenueHeadSlug, revenueHeadId);
        }


        /// <summary>
        /// Get view for edit billing
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="revenueHeadSlug"></param>
        /// <returns>BillingViewModel</returns>
        public BillingViewModel GetBillingView(int revenueHeadId, string revenueHeadSlug, bool isEdit)
        {
            if (!isEdit) { return GetCreateBillingView(revenueHeadId, revenueHeadSlug); }

            IsAuthorized<BillingHandler>(Permissions.UpdateBillingAmount);
            RevenueHead revenueHead = GetRevenueHead(revenueHeadId, revenueHeadSlug);
            //inorder to edit we have to first of all check if this guy has billing info
            var billing = revenueHead.BillingModel;
            if (!HasBilling(billing)) { throw new HasNoBillingException(); }
            //check if rev has subrevenue heads
            if (HasSubRevenueHeads(revenueHead)) { throw new CannotCreateStartSetupProcessBecauseRevenueHeadHasSubRevenueHeadsException(revenueHead.NameAndCode()); }
            //transform billing model to view compact model
            BillingHelperModel billingHelper = _billingService.ConvertBillingToHelperModel(billing);

            DirectAssessmentVM directAssessment = (billingHelper.DirectAssessment == null) ? new DirectAssessmentVM { ListOfAssessmentInterface = GetAssessmentInterfaces() } : new DirectAssessmentVM
            {
                ListOfAssessmentInterface = GetAssessmentInterfaces(),
                AdapterValue = billingHelper.DirectAssessment.AdapterValue,
                AllowFileUpload = billingHelper.DirectAssessment.AllowFileUpload
            };


            FileUploadTemplatesVM fileUploadBillingModel = (billingHelper.FileUploadTemplates == null) ? GetFileUploadTemplatesVM() : new FileUploadTemplatesVM
            {
                ListOfTemplates = GetFileUploadTemplatesVM().ListOfTemplates,
                SelectedTemplate = billingHelper.FileUploadTemplates.SelectedTemplate,
                SelectedImplementation = billingHelper.FileUploadTemplates.SelectedImplementation
            };

            return new BillingViewModel
            {
                RHName = revenueHead.NameAndCode(),
                FrequencyModel = billingHelper.BillingFrequencyModel,
                BillingType = billingHelper.BillingType,
                Indexer = 0,
                DiscountModelPostBackData = billingHelper.DiscountCollection,
                Surcharge = billingHelper.Surcharge.ToString("N"),
                BillingDemandNotice = billingHelper.DemandNotice,
                PenaltyModelPostBackData = billingHelper.PenaltyCollection,
                SAmount = billing.Amount.ToString("N"),
                DueDateModel = billingHelper.DueDateModel,
                CallBackObject = new CallBackObject { HasFrequencyValue = billingHelper.AssessmentModel.IsRecurring },
                IsRecurring = billingHelper.AssessmentModel.IsRecurring,
                IsPrepaid = billingHelper.AssessmentModel.IsPrepaid,
                IsDirectAssessment = billingHelper.AssessmentModel.IsDirectAssessment,
                DirectAssessment = directAssessment,
                FileUploadBillingModel = fileUploadBillingModel,
                IsEdit = true,
                
            };
        }


        #endregion

    }
}