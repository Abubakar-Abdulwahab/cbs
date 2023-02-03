using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Notifications.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.Web.Controllers.CommonHandlers.HelperHandlers.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.BillingImpl.Contracts;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.Web.Controllers.Handlers
{
    public class ModuleCollectionAjaxHandler : CommonBaseHandler, IModuleCollectionAjaxHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreCollectionService _coreCollectionService;

        private readonly IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IHandlerHelper _handlerHelper;
        private readonly IFormControlRevenueHeadManager<FormControlRevenueHead> _formRevenueHeadRepository;
        private readonly IFormControlsManager<FormControl> _formcontrolsRepository;
        private readonly IEnumerable<IBillingImpl> _billingImpls;
        private readonly ICorePaymentService _corePaymentService;
        private readonly ICoreUserService _coreUserService;
        private readonly ICoreTaxPayerService _coreTaxPayerService;

        public ModuleCollectionAjaxHandler(IOrchardServices orchardServices, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreCollectionService coreCollectionService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IHandlerHelper handlerHelper, ICoreUserService coreUserService, IFormControlRevenueHeadManager<FormControlRevenueHead> formRevenueHeadRepository, IFormControlsManager<FormControl> formcontrolsRepository, IEnumerable<IBillingImpl> billingImpls, ICoreTaxPayerService coreTaxPayerService, ICorePaymentService corePaymentService) : base(orchardServices, settingsRepository, handlerHelper)
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
            _corePaymentService = corePaymentService;
        }


        /// <summary>
        /// Get the tax payer profiles by category id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetTaxProfilesByCategory(string sCategoryId)
        {
            int categoryId = 0;
            bool parsed = Int32.TryParse(sCategoryId, out categoryId);
            if (!parsed) { return new APIResponse { Error = true, ResponseObject = ErrorLang.categorynotfound().ToString() }; }

            List<TaxPayerWithDetails> profiles = _coreTaxPayerService.GetTaxEntitiesByCategory(categoryId);
            if (profiles == null || profiles.Count <= 0) { return new APIResponse { Error = true, ResponseObject = Lang.noprofiles404.ToString() }; }
            return new APIResponse { ResponseObject = profiles };
        }


        /// <summary>
        /// Get Payment Reference Number
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="InvoiceId"></param>
        /// <param name="provider"></param>
        /// <returns>APIResponse</returns>
        public APIResponse GetPaymentReferenceNumber(int InvoiceId, string InvoiceNumber, PaymentProvider provider)
        {
            try
            {
                PaymentReference paymentReference = _corePaymentService.SavePaymentReference(new PaymentReference
                {
                    PaymentProvider = (int)provider,
                    Invoice = new Invoice { Id = InvoiceId },
                    InvoiceNumber = InvoiceNumber
                });

                //Evict Payment Reference object
                _corePaymentService.EvictPaymentReferenceObject(paymentReference);

                //Call the payment reference table to get the computed reference number
                return _corePaymentService.GetPaymentReference(paymentReference.Id);
            }
            catch (CouldNotSaveRecord ex)
            {
                Logger.Error(ex, $"Unable save Payment Reference for Invoice Id {InvoiceId}");
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
            catch (InvoiceAlreadyPaidForException exception)
            {
                Logger.Error(exception, exception.Message);
                return new APIResponse { Error = true, StatusCode = System.Net.HttpStatusCode.ResetContent, ResponseObject = ErrorLang.invoiceFullyPaid(InvoiceNumber).ToString() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, $"Unable to get Payment Reference for Invoice Id {InvoiceId}");
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }


        /// <summary>
        /// Gets all the LGAs of the state with the specified ID
        /// </summary>
        /// <param name="sStateId"> Id of the state which the LGAs belong to</param>
        /// <returns>APIResponse Object filled with LGAs if successful, if not it returns null and throws an exception</returns>
        public APIResponse GetLgasByStates(string sStateId)
        {
            try
            {
                int stateId = 0;
                if (!Int32.TryParse(sStateId, out stateId))
                { return new APIResponse { Error = true, ResponseObject = ErrorLang.statenotfound().ToString() }; }

                List<LGA> lgas = _handlerHelper.GetStateLgas(stateId);
                if (lgas == null || lgas.Count < 1)
                { return new APIResponse { Error = true, ResponseObject = "No LGA" }; }

                return new APIResponse { ResponseObject = lgas };
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                return new APIResponse { Error = true, ResponseObject = ErrorLang.statenotfound().ToString() };
            }

        }

        /// <summary>
        /// Send notification for the specified payment reference and provider
        /// </summary>
        /// <param name="paymentReference"></param>
        /// <param name="provider"></param>
        /// <returns>APIResponse</returns>
        public APIResponse SendNotification(string paymentReference, PaymentProvider provider)
        {
            try
            {
                return _corePaymentService.SendNotifications(paymentReference, provider);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Exception in sending notification " + paymentReference);
                return new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() };
            }
        }
    }

}