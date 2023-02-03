using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Web;
using Orchard.Security;
using Parkway.CBS.Payee;
using System.IO;
using System.Text;
using System.Globalization;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.ViewModels;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Core.DataFilters.CollectionReport;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public class ModuleCollectionHandler : CommonBaseHandler, IModuleCollectionHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreCollectionService _coreCollectionService;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IEnumerable<IBillingImpl> _billingImpls;
        private readonly ICoreUserService _coreUserService;
        private readonly ICollectionReportFilter _collectionReportFilter;
        private readonly ICoreReceiptService _coreReceiptService;
        private readonly ICorePaymentService _corePaymentService;
        private readonly Lazy<ICoreFormService> _coreFormService;


        public ModuleCollectionHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreCollectionService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IEnumerable<IBillingImpl> billingImpls, ICollectionReportFilter collectionreportFilter, ICoreReceiptService coreReceiptService, ICorePaymentService corePaymentService, Lazy<ICoreFormService> coreFormService) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _coreCollectionService = coreCollectionService;
            _taxCategoriesRepository = taxCategoriesRepository;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _billingImpls = billingImpls;
            _collectionReportFilter = collectionreportFilter;
            _coreReceiptService = coreReceiptService;
            _corePaymentService = corePaymentService;
            _coreFormService = coreFormService;
        }


        [Obsolete("Could be better, get the revenue heads and the mdas. Needs work from the ground up on how visible revenue heads are treated and when their statues are changed")]
        /// <summary>
        /// Get the list of billable revenue heads 
        /// </summary>
        /// <param name="mdas"></param>
        /// <returns>IEnumerable{RevenueHead}</returns>
        protected virtual IEnumerable<RevenueHead> GetBillables(IEnumerable<MDA> mdas)
        {
            List<RevenueHead> billables = new List<RevenueHead>();
            foreach (var mda in mdas)
            {
                //Logger.Information(string.Format("Searching for active and visible revenue heads for mda {0}", mda.Name));
                //get all billable revenue heads under this mda
                var billableRevenueHeads = mda.RevenueHeads.Where(r => (r.BillingModel != null) && (r.IsActive) && (r.IsVisible)).Select(r => r);
                foreach (var revenueHead in billableRevenueHeads)
                {
                    billables.Add(revenueHead);
                }
            }
            return billables.OrderBy(k => k.Name);
        }

        /// <summary>
        /// get invoice URL
        /// </summary>
        /// <param name="bIN"></param>
        /// <returns></returns>
        public string GetInvoiceURL(string bin)
        {
            return _coreCollectionService.GetInvoiceURL(bin);
        }



        public InvoiceGeneratedResponseExtn SearchForInvoiceForPaymentView(string bIN)
        { return _coreCollectionService.GetInvoiceGeneratedResponseObjectForPaymentView(bIN); }


        /// <summary>
        /// get view model for register view
        /// </summary>
        /// <returns>RegisterCBSUserObj</returns>
        public RegisterCBSUserObj GetRegsiterView()
        {
            List<TaxEntityCategory> categories = _taxCategoriesRepository.GetCollection(catty => catty.Status).ToList();
            return new RegisterCBSUserObj { HeaderObj = new HeaderObj { }, RegisterCBSUserModel = new RegisterCBSUserModel { }, TaxCategories = categories, ErrorMessage = string.Empty, StateLGAs = GetStatesAndLGAs() };
        }


        /// <summary>
        /// Try register CBS user
        /// </summary>
        /// <param name="model">RegisterCBSUserObj</param>
        public void TryRegisterCBSUser(BaseCollectionController callback, RegisterCBSUserObj model)
        {
            //get the category
            Logger.Information("Getting category");
            int catId = 0;
            bool parsed = Int32.TryParse(model.TaxPayerType, out catId);
            if (!parsed) { throw new NoCategoryFoundException(); }
            TaxEntityCategory category = _coreCollectionService.GetTaxEntityCategory(catId);
            //check if the category is not null
            List<ErrorModel> errors = new List<ErrorModel>();
            try
            {
                _coreUserService.TryCreateCBSUser(model.RegisterCBSUserModel, category, ref errors);
            }
            catch (DirtyFormDataException)
            { AddValidationErrorsToCallback<ModuleCollectionHandler, BaseCollectionController>(callback, errors); throw new DirtyFormDataException(); }
        }


        public ViewToShowVM GetDirectAssessmentBillVM(TaxEntity entity, GenerateInvoiceStepsModel processStage, bool isLoggedIn = false)
        {
            //ideally we should be using the billing impl here
            //get the category
            Logger.Information("Getting category");
            TaxEntityCategory category = _coreCollectionService.GetTaxEntityCategory(processStage.CategoryId);
            //check if the category requires login
            if (category.RequiresLogin) { if (!isLoggedIn) { throw new AuthorizedUserNotFoundException(); } }

            foreach (var item in _billingImpls)
            {
                if (item.BillingType == processStage.BillingType)
                {
                    return item.InvoiceInputPage(entity, processStage, _orchardServices.WorkContext.CurrentSite.SiteName);
                }
            }
            throw new NoBillingTypeSpecifiedException();
        }


        /// <summary>
        /// Get view model for invoice confirmation
        /// </summary>
        /// <returns>dynamic</returns>
        public dynamic ConfirmingInvoiceVM(GenerateInvoiceStepsModel processStage, UserDetailsModel user)
        {
            foreach (var item in _billingImpls)
            {
                if (item.BillingType == processStage.BillingType)
                {
                    return item.ConfirmingInvoice(processStage, user);
                }
            }
            throw new NoBillingTypeSpecifiedException();
        }


        /// <summary>
        /// Get view data for self assessment page
        /// </summary>
        /// <returns>SelfAssessmentCategoryVM</returns>
        public GenerateInvoiceVM GetSelftAssessmentView()
        {
            Logger.Information("getting view model for GetSelftAssessmentView");
            //get billable mdas
            var mdas = GetMdas();
            //var selectMDAs = mdas.Select(m => new SelectListItem() { Text = m.NameAndCode(), Value = m.Slug });
            IEnumerable<RevenueHead> billables = new List<RevenueHead>();
            billables = GetBillables(mdas);
            List<TaxEntityCategoryVM> categories = _taxCategoriesRepository.GetTaxEntityCategoryVM();
            //if this request has a categoryId, lets check if the profile is needed
            return new GenerateInvoiceVM
            {
                RevenueHeads = billables,
                TaxCategories = categories,
                HeaderObj = new HeaderObj { },
                AllowCategorySelect = true,
            };
        }


        /// <summary>
        /// Create a claim token for this request
        /// </summary>
        /// <param name="email"></param>
        /// <param name="requestToken"></param>
        /// <returns>string</returns>
        public string CreateClaims(IUser ouser, string requestToken)
        {
            Logger.Debug(string.Format("creating claim {0} {1}", ouser.Email, requestToken));
            UserClaim userClaim = new UserClaim { Email = ouser.Email, UserName = ouser.UserName, TTL = DateTime.Now.ToLocalTime().AddSeconds(1800) };
            string claims = JsonConvert.SerializeObject(userClaim);
            return Util.LetsEncrypt(claims, requestToken + AppSettingsConfigurations.EncryptionSecret);
        }


        /// <summary>
        /// Check if the category requires the user to login
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns>bool</returns>
        /// <exception cref="NoCategoryFoundException"></exception>
        public bool RequiresLogin(int categoryType)
        {
            Logger.Information("Getting category");
            return _coreCollectionService.GetTaxEntityCategory(categoryType).RequiresLogin;
        }


        private IEnumerable<MDA> GetMdas()
        { return _coreCollectionService.GetMDAs(); }



        /// <summary>
        /// validate confirm invoice model
        /// </summary>
        /// <param name="collectionController"></param>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <returns>ValidateInvoiceConfirmModel</returns>
        public void ValidateConfirmInvoiceModel(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, ref List<ErrorModel> errors)
        {
            foreach (var item in _billingImpls)
            {
                if (item.BillingType == processStage.BillingType)
                { item.ValidateConfirmingInvoice(processStage, model, ref errors); return; }
            }
            throw new NoBillingTypeSpecifiedException();
        }


        /// <summary>
        /// Add the bunch of errors into the controller model, throw a DirtyFormDataException
        /// </summary>
        /// <param name="collectionController"></param>
        /// <param name="errors"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        public void AddErrorsToModel(BaseCollectionController callback, List<ErrorModel> errors)
        { AddValidationErrorsToCallback<ModuleCollectionHandler, BaseCollectionController>(callback, errors); }


        /// <summary>
        /// if the validation for invoice confirmation has errors
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns>ValidateInvoiceConfirmModel</returns>
        public ValidateInvoiceConfirmModel GetViewModelForConfirmInvoiceModelPostBack(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model, UserDetailsModel user, List<ErrorModel> errors)
        {
            foreach (var item in _billingImpls)
            {
                if (item.BillingType == processStage.BillingType)
                { return item.ConfirmingInvoiceFailed(processStage, model, user, errors); }
            }
            throw new NoBillingTypeSpecifiedException();
        }


        /// <summary>
        /// get the model for invoice generation
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="user"></param>
        /// <returns>CreateInvoiceModel</returns>
        public CreateInvoiceModel GetCreateInvoiceModel(GenerateInvoiceStepsModel processStage, TaxEntity entity)
        {
            //lets do a validation on the revenue head
            //lets validate that this revenue head is bound to an external redirect for invoice generation'
            var result = _coreCollectionService.GetRevenueHeadDetails(processStage.RevenueHeadId);
            if (!string.IsNullOrEmpty(result.InvoiceGenerationRedirectURL))
            { return new CreateInvoiceModel { ExternalRedirect = new ExternalRedirect { Redirecting = true, URL = result.InvoiceGenerationRedirectURL } }; }

            foreach (var item in _billingImpls)
            {
                if (item.BillingType == processStage.BillingType)
                { return item.GenerateInvoice(processStage, entity); }
            }
            throw new NoBillingTypeSpecifiedException();
        }


        /// <summary>
        /// when the invoice has been confirmed this model would contain the necessary information you would need to 
        /// generate the CreateInvoiceModel
        /// </summary>
        /// <param name="processStage"></param>
        /// <param name="model"></param>
        /// <returns>InvoiceConfirmedModel</returns>
        public InvoiceConfirmedModel GetInvoiceConfirmedModel(GenerateInvoiceStepsModel processStage, ConfirmInvoiceVM model)
        {
            foreach (var item in _billingImpls)
            {
                if (item.BillingType == processStage.BillingType)
                { return item.ConfirmedInvoice(processStage, model); }
            }
            throw new NoBillingTypeSpecifiedException();
        }


        /// <summary>
        /// Get invoice details for tax payer
        /// </summary>
        /// <param name="createInvoiceModel"></param>
        /// <returns>InvoiceGeneratedResponseExtn</returns>
        public InvoiceGeneratedResponseExtn GetInvoiceDetails(CreateInvoiceModel createInvoiceModel, ref List<ErrorModel> errors)
        {
            return _coreCollectionService.GenerateInvoice(createInvoiceModel, ref errors);
        }


        /// <summary>
        /// When a user is signed in, we would like to redirect the user to a page where they can progress with their invoice generation
        /// </summary>
        /// <param name="user"></param>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <returns>ProceedWithInvoiceGenerationVM</returns>
        public ProceedWithInvoiceGenerationVM GetModelWhenUserIsSignedIn(UserDetailsModel user, int revenueHeadId, int categoryId)
        {
            RevenueHeadEssentials essentials = GetRevenueHeadEssentials(user.Category, revenueHeadId);

            if (essentials.RevenueHeadDetails.Redirect) { return new ProceedWithInvoiceGenerationVM { Redirect = true, InvoiceGenerationRedirectURL = essentials.RevenueHeadDetails.InvoiceGenerationRedirectURL }; }

            return new ProceedWithInvoiceGenerationVM
            {
                BillingType = essentials.BillingType,
                CategoryName = GetCatText(essentials.TaxEntityCategory),
                RevenueHeadName = essentials.RevenueHeadDetails.RevenueHead.Name,
                MDAName = essentials.RevenueHeadDetails.Mda.Name,
                HeaderObj = new HeaderObj { ShowSignin = true, DisplayText = user.Name, IsLoggedIn = true, },
                Entity = new TaxEntity { PayerId = user.Entity.PayerId, Address = user.Entity.Address, Email = user.Entity.Email, PhoneNumber = user.Entity.PhoneNumber, TaxPayerIdentificationNumber = user.Entity.TaxPayerIdentificationNumber, Id = user.Entity.Id, TaxEntityCategory = new TaxEntityCategory { Id = user.Category.Id, }, Recipient = user.Entity.Recipient },
                Amount = essentials.RevenueHeadDetails.Billing.Amount,
                Redirect = essentials.RevenueHeadDetails.Redirect,
                InvoiceGenerationRedirectURL = essentials.RevenueHeadDetails.InvoiceGenerationRedirectURL,
                Surcharge = essentials.RevenueHeadDetails.Billing.Surcharge,
                CallBackURL = essentials.RevenueHeadDetails.RevenueHead.CallBackURL,
                RevenueHeadNameAndCode = essentials.RevenueHeadDetails.RevenueHead.NameAndCode(),
                MDANameAndCode = essentials.RevenueHeadDetails.Mda.NameAndCode(),
                BillingId = essentials.RevenueHeadDetails.Billing.Id
            };
        }


        /// <summary>
        /// For anons, we would like to get the model for invoice generation
        /// <para>For categories that users are selected from, we should get the select tax entity profile and continue
        /// with invoice generation else we redirect to profile page
        /// </para>
        /// </summary>
        /// <param name="revenueHeadId"></param>
        /// <param name="categoryId"></param>
        /// <param name="profileIdentifier"></param>
        /// <returns>ProceedWithInvoiceGenerationVM</returns>
        public ProceedWithInvoiceGenerationVM GetModelForAnonymous(int revenueHeadId, int categoryId, string profileIdentifier)
        {
            //lets check if the category has an option for drop down selection
            var category = _coreCollectionService.GetTaxEntityCategory(categoryId);
            if (!category.GetSettings().CanShowDropDown) { return null; }
            Int64 taxEntityId = 0;
            bool parsed = Int64.TryParse(profileIdentifier, out taxEntityId);

            if (string.IsNullOrEmpty(profileIdentifier) || !parsed) { throw new NoRecordFoundException(); }

            TaxEntity taxEntity = _coreUserService.GetTaxEntityById(taxEntityId);

            RevenueHeadEssentials essentials = GetRevenueHeadEssentials(category, revenueHeadId);

            if (essentials.RevenueHeadDetails.Redirect) { return new ProceedWithInvoiceGenerationVM { Redirect = true, InvoiceGenerationRedirectURL = essentials.RevenueHeadDetails.InvoiceGenerationRedirectURL }; }

            return new ProceedWithInvoiceGenerationVM
            {
                BillingType = essentials.BillingType,
                CategoryName = GetCatText(category),
                Entity = new TaxEntity { PayerId = taxEntity.PayerId, Address = taxEntity.Address, Email = taxEntity.Email, PhoneNumber = taxEntity.PhoneNumber, Recipient = taxEntity.Recipient, TaxPayerIdentificationNumber = taxEntity.TaxPayerIdentificationNumber, Id = taxEntity.Id, TaxEntityCategory = new TaxEntityCategory { Id = category.Id } },
                FromTaxProfileSetup = true,
                MDAName = essentials.RevenueHeadDetails.Mda.Name,
                RevenueHeadName = essentials.RevenueHeadDetails.RevenueHead.Name,
                Amount = essentials.RevenueHeadDetails.Billing.Amount,
                Surcharge = essentials.RevenueHeadDetails.Billing.Surcharge,
                Redirect = essentials.RevenueHeadDetails.Redirect,
                InvoiceGenerationRedirectURL = essentials.RevenueHeadDetails.InvoiceGenerationRedirectURL,
                BillingId = essentials.RevenueHeadDetails.Billing.Id
            };
        }


        /// <summary>
        /// Get the route to redirect the user to for invoice confirmation
        /// </summary>
        /// <param name="billingType"></param>
        /// <returns>dynamic</returns>
        public dynamic GetConfirmingInvoiceRoute(BillingType billingType)
        {
            foreach (var item in _billingImpls)
            {
                if (item.BillingType == billingType)
                { return item.ConfirmingInvoiceRoute(); }
            }
            throw new NoBillingTypeSpecifiedException("No billing type found " + billingType.ToString());
        }


        /// <summary>
        /// Get revenue head details you would need for invoice generation for a signed in user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="category"></param>
        /// <returns>RevenueHeadEssentials</returns>
        private RevenueHeadEssentials GetRevenueHeadEssentials(TaxEntityCategory category, int revenueHeadId)
        {
            RevenueHeadDetails revenueHeadDetails = _coreCollectionService.GetRevenueHeadDetails(revenueHeadId);
            if (revenueHeadDetails.Mda == null) { throw new MDARecordNotFoundException(); }

            //check if this revenue head has been configured to generate invoices on another platform
            var checkVal = CheckForInvoiceGenerationRedirect(revenueHeadDetails);
            if (checkVal != null) { return checkVal; }

            //getting billing info
            BillingModel billing = revenueHeadDetails.Billing;
            if (billing == null) { throw new NoBillingInformationFoundException(); }
            Logger.Information("Gotten billing and mda record");

            //getting billing info
            if (!typeof(BillingType).IsEnumDefined(revenueHeadDetails.Billing.BillingType))
            { throw new NoBillingTypeSpecifiedException(string.Format("No billing type specificed billing type {0}  id {1} ", revenueHeadDetails.Billing.BillingType, revenueHeadDetails.Billing.Id)); }

            BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;
            IEnumerable<FormControl> controls = new List<FormControl>();
            //we have got the essentials, lets get the form details
            Logger.Information("Getting form controls");

            return new RevenueHeadEssentials
            {
                TaxEntityCategory = category,
                RevenueHeadDetails = revenueHeadDetails,
                Controls = controls,
                BillingType = billingType,
            };
        }


        /// <summary>
        /// Get collection report model
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <param name="searchParams"></param>
        /// <returns>CollectionReportViewModel</returns>
        public PaymentsVM GetCollectionReport(long entityId, PaymentsVM model, string datefilter, int skip)
        {
            int take = 10;
            DateTime startDate = DateTime.Now.AddMonths(-3);
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(datefilter))
            {
                var dateFilterSplit = datefilter.Split(new[] { '-' }, 2);
                if (dateFilterSplit.Length == 2)
                {
                    try
                    {
                        startDate = DateTime.ParseExact(dateFilterSplit[0].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(dateFilterSplit[1].Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        startDate = DateTime.Now.AddMonths(-3);
                        endDate = DateTime.Now;
                    }
                }
            }
            endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

            CollectionSearchParams searchParams = new CollectionSearchParams
            {
                FromRange = startDate,
                EndRange = endDate,
                //InvoiceNumber = invoiceNumber,
                //PaymentRef = paymentRef,
                //SRevenueHeadId = revenueheadId,
                //SelectedPaymentMethod = selectedPaymentMethod ?? PaymentChannel.None,
                ReceiptNumber = model.ReceiptNumber,
                //SelectedMDA = mda,
                SelectedBankCode = model.SelectedBank,
                PaymentDirection = CollectionPaymentDirection.PaymentDate,
                Take = take,
                Skip = skip,
                TaxEntityId = entityId,
            };

            PaymentsVM returnModel = new PaymentsVM();
            returnModel.TaxEntityId = entityId;

            var collectionResult = _coreCollectionService.GetReportForCollection(searchParams);
            returnModel.ReportRecords = collectionResult.ReportRecords;
            returnModel.TotalAmountPaid = collectionResult.TotalAmountPaid;
            returnModel.TotalNumberOfPayment = collectionResult.TotalNumberOfPayment;

            var dataSize = returnModel.TotalNumberOfPayment;
            double pageSize = ((double)dataSize / (double)take);
            int pages = 0;

            if (pageSize < 1 && dataSize >= 1) { pages = 1; }
            else { pages = (int)Math.Ceiling(pageSize); }

            returnModel.ReceiptNumber = model.ReceiptNumber;
            returnModel.HeaderObj = new HeaderObj { IsLoggedIn = true, ShowSignin = true };
            returnModel.DataSize = pages;
            returnModel.DateFilter = string.Format("{0} - {1}", startDate.ToString("dd'/'MM'/'yyyy"), endDate.ToString("dd'/'MM'/'yyyy"));
            returnModel.Token = Util.LetsEncrypt(JsonConvert.SerializeObject(new ReceiptDataModel { DateFilter = string.Format("{0} - {1}", startDate.ToString("dd'/'MM'/'yyyy"), endDate.ToString("dd'/'MM'/'yyyy")), ChunkSize = 10, Page = 1 }), System.Configuration.ConfigurationManager.AppSettings["EncryptionSecret"]);

            return returnModel;
        }

        /// <summary>
        /// Get the next paged data for cell sites for operator page display
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPagedPaymentList(long operatorId, int page, string datefilter)
        {
            try
            {
                int take = 10;
                int skip = page == 1 ? 0 : (take * page) - take;
                PaymentsVM record = GetCollectionReport(operatorId, new PaymentsVM { }, datefilter, skip);
                return new APIResponse { ResponseObject = record };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message + " .Error getting data for scheduleRef " + operatorId);
            }
            return new APIResponse { ResponseObject = ErrorLang.genericexception().ToString(), Error = true };
        }


        public ReceiptViewModel GetReceiptVM(string receiptNumber)
        {
            try
            {
                var result = _coreReceiptService.GetReceiptVMByReceiptNumber(receiptNumber);
                if (result == null) { throw new NoRecordFoundException("No record found for receipt number " + receiptNumber); }
                return result;
            }
            catch (NoRecordFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// create PDF file for receipt download
        /// </summary>
        /// <param name="reportVM"></param>
        /// <returns>CreateReceiptDocumentVM</returns>
        /// <exception cref="NoRecordFoundException"></exception>
        public CreateReceiptDocumentVM CreateReceiptFile(string receiptNumber)
        {
            ReceiptViewModel vm = GetReceiptVM(receiptNumber);
            return _coreReceiptService.CreateReceiptDocument(vm);
        }


        /// <summary>
        /// Save netpay payment details
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<InvoiceValidationResponseModel> SavePayment(PaymentAcknowledgeMentModel model)
        {
            try
            {
                return await _corePaymentService.SaveNetpayPayment(model);
            }
            catch (PaymentNoficationAlreadyExistsException)
            {
                TransactionLogGroup res = _corePaymentService.GetByPaymentRef(model.PaymentRequestRef, PaymentProvider.Bank3D);
                model.PaymentStatus = PaymentStatus.Successful;
                return new InvoiceValidationResponseModel { ReceiptNumber = res.ReceiptNumber };
            }
            catch (CannotVerifyNetPayTransaction)
            {
                throw;
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns>PaymentReferenceVM</returns>
        public PaymentReferenceVM GetPaymentReferenceDetail(string referenceNumber)
        {
           return _corePaymentService.GetPaymentReferenceDetail(referenceNumber);
        }


        /// <summary>
        /// Validate form fields
        /// </summary>
        public void ValidateForms(IEnumerable<FormControlViewModel> userControlInputs, IEnumerable<FormControlViewModel> expectedControlInputs, ref List<ErrorModel> errors)
        {
            errors = _coreFormService.Value.AddAndValidateFormValueFromUserToCorrespondingDBFormControl(userControlInputs, expectedControlInputs);
        }


        /// <summary>
        /// Get expected forms for this revenue head and categoryId
        /// </summary>
        /// <param name="processStage"></param>
        /// <returns>IEnumerable{FormControlViewModel}</returns>
        public IEnumerable<FormControlViewModel> GetDBForms(GenerateInvoiceStepsModel processStage)
        {
            return _coreFormService.Value.GetRevenueHeadFormFields(processStage.RevenueHeadId, processStage.CategoryId);
        }


        /// <summary>
        /// Get the state and LGAs
        /// </summary>
        /// <returns>List{StateModel}</returns>
        public List<StateModel> GetStatesAndLGAs()
        {
            return _handlerHelper.GetAllStates();
        }

    }
}