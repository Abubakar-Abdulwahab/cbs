using Orchard;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using System;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using Orchard.Logging;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public class TaxProfileHandler : CommonBaseHandler, ITaxProfileHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreCollectionService _coreCollectionService;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly IFormControlsManager<FormControl> _formcontrolsRepository;
        private readonly IEnumerable<IBillingImpl> _billingImpls;
        private readonly ICoreTaxPayerService _coreTaxPayerService;
        private TaxProfileFormVM TaxProfileVM;
        private readonly ICoreUserService _coreUserService;

        public TaxProfileHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreCollectionService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IFormControlsManager<FormControl> formcontrolsRepository, IEnumerable<IBillingImpl> billingImpls, ICoreTaxPayerService coreTaxPayerService) : base(orchardServices, settingsRepository, handlerHelper)
        {
            _coreCollectionService = coreCollectionService;
            _taxCategoriesRepository = taxCategoriesRepository;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _handlerHelper = handlerHelper;
            _coreUserService = coreUserService;
            _formRevenueHeadRepository = formRevenueHeadRepository;
            _formcontrolsRepository = formcontrolsRepository;
            _billingImpls = billingImpls;
            _coreTaxPayerService = coreTaxPayerService;
        }


        public ProceedWithInvoiceGenerationVM TrySaveTaxEntityProfile<C>(C callback, int revenueHeadId, int categoryId, TaxEntityViewModel model, TaxProfileFormVM persistedModel, ICollection<CollectionFormVM> additionalFormFields) where C : BaseTaxProfileController
        {
            var category = _coreCollectionService.GetTaxEntityCategory(categoryId);
            List<ErrorModel> errors = new List<ErrorModel> { };
            //check if the selected state LGA exists
            if (!_handlerHelper.CheckLgaExists(model.SelectedStateLGA)) {
                errors.Add(new ErrorModel { ErrorMessage = "Selected LGA does not exist.", FieldName = "TaxEntityViewModel.SelectedStateLGA" });
            }

            if (errors.Count > 0)
            {
                AddValidationErrorsToCallback<TaxProfileHandler, BaseTaxProfileController>(callback, errors);
            }

            TaxEntityProfileHelper result = _coreTaxPayerService.ValidateAndSaveTaxEntity(new TaxEntity
            {
                Address = model.Address,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Recipient = model.Recipient,
                TaxPayerIdentificationNumber = model.TaxPayerIdentificationNumber,
                StateLGA = new LGA { Id = model.SelectedStateLGA }
            }, 
            category);
            //now that we have potentially registered the user or retrieved an already created profile, lets see if that profile requires the payer to be logged in

            if (result.RequiresLogin) { throw new AuthorizedUserNotFoundException(result.Message); }

            return new ProceedWithInvoiceGenerationVM { CategoryName = GetCatText(result.Category), Entity = new TaxEntity { PayerId = result.TaxEntity.PayerId, Address = result.TaxEntity.Address, Email = result.TaxEntity.Email, PhoneNumber = result.TaxEntity.PhoneNumber, Recipient = result.TaxEntity.Recipient, TaxPayerIdentificationNumber = result.TaxEntity.TaxPayerIdentificationNumber, Id = result.TaxEntity.Id, TaxEntityCategory = new TaxEntityCategory { Id = result.Category.Id } }, FromTaxProfileSetup = true, MDAName = persistedModel.MDAName, RevenueHeadName = persistedModel.RevenueHeadName, Message = result.Message, HasMessage = true, Amount = persistedModel.Amount, Surcharge = persistedModel.Surcharge };

        }


        /// <summary>
        /// Get view model for payer profile view
        /// </summary>
        /// <param name="processStage">GenerateInvoiceStepsModel</param>
        /// <returns>TaxProfileFormVM</returns>
        public TaxProfileFormVM GetTaxPayerProfileVM(GenerateInvoiceStepsModel processStage)
        {
            //get the category
            Logger.Information("Getting category");
            TaxEntityCategory category = _coreCollectionService.GetTaxEntityCategory(processStage.CategoryId);
            //check if the category requires login
            if (category.RequiresLogin) { throw new AuthorizedUserNotFoundException(); }

            //if all clear lets get the revenue head
            var revenueHeadDetails = _coreCollectionService.GetRevenueHeadDetails(processStage.RevenueHeadId);

            //check if this revenue head requires invoice generation to be done by an external system
            if (!string.IsNullOrEmpty(revenueHeadDetails.InvoiceGenerationRedirectURL))
            {
                return new TaxProfileFormVM { ExternalRedirect = new ExternalRedirect { URL = revenueHeadDetails.InvoiceGenerationRedirectURL, Redirecting = true } };
            }

            //getting billing info
            if (!typeof(BillingType).IsEnumDefined(revenueHeadDetails.Billing.BillingType))
            { throw new NoBillingTypeSpecifiedException(string.Format("No billing type specificed billing id {0} ", revenueHeadDetails.Billing.BillingType)); }

            BillingType billingType = (BillingType)revenueHeadDetails.Billing.BillingType;


            //we have got the essentials, lets get the form details
            Logger.Information("Getting form controls");

            TenantCBSSettings tenant = _handlerHelper.GetTenant(); //Gets current tenant settings
            
            foreach (var item in _billingImpls)
            {
                if (item.BillingType == billingType)
                {
                    TaxProfileVM = item.GetModelForFormInput(revenueHeadDetails, null, category, processStage, tenant); 
                    TaxProfileVM.AllStates = _handlerHelper.GetAllStates(); //Gets all states
                    TaxProfileVM.AllLgas = _handlerHelper.GetStateLgas(tenant.StateId); //Gets all the LGAs for the state with the specified state Id
                    TaxProfileVM.TenantState = tenant.StateId;
                    return TaxProfileVM;
                }
            }
            throw new NotImplementedException("Billing type has no implementation " + billingType.ToString());
        }


        /// <summary>
        /// Get the route name for this billing type
        /// </summary>
        /// <param name="billingType"></param>
        /// <returns>string</returns>
        /// <exception cref="NoBillingTypeSpecifiedException">No billing type found</exception>
        public RouteToVM GetPageToRouteTo(BillingType billingType)
        {
            foreach (var item in _billingImpls)
            {
                if (item.BillingType == billingType)
                { return item.ConfirmingInvoiceRoute(); }
            }
            throw new NoBillingTypeSpecifiedException("No billing type found " + billingType.ToString());
        }


    }
}