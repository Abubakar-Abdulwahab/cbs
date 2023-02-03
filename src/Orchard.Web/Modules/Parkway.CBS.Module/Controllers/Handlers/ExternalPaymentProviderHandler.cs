using Newtonsoft.Json;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.DataFilters.PaymentProvider;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class ExternalPaymentProviderHandler : BaseHandler, IExternalPaymentProviderHandler
    {
        private readonly ICoreExternalPaymentProviderService _extPaymentProviderService;
        private readonly IOrchardServices _orchardServices;
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IRevenueHeadManager<RevenueHead> _revHeadRepo;
        private readonly IPaymentProviderFilter _paymentProviderFilter;


        public ExternalPaymentProviderHandler(ICoreExternalPaymentProviderService extPaymentProviderService, IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IRevenueHeadManager<RevenueHead> revHeadRepo, IPaymentProviderFilter paymentProviderFilter) 
            : base(orchardServices, settingsRepository)
        {
            _extPaymentProviderService = extPaymentProviderService;
            _orchardServices = orchardServices;
            _settingsRepository = settingsRepository;
            _revHeadRepo = revHeadRepo;
            _paymentProviderFilter = paymentProviderFilter;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="createMDA"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        public void CheckForPermission(Orchard.Security.Permissions.Permission permission)
        {
            IsAuthorized<ExternalPaymentProviderHandler>(permission);
        }


        /// <summary>
        /// Creates an external payment provider
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="userInput"></param>
        public void TryCreateExtPaymentProvider(ExternalPaymentProviderController callBack,ExternalPaymentProviderVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            Logger.Information("Creating External Payment Provider");
            try
            {
                IsAuthorized<ExternalPaymentProviderHandler>(Permissions.CreateExternalPaymentProvider).IsModelValid<ExternalPaymentProviderHandler, ExternalPaymentProviderController>(callBack);
                if (!String.IsNullOrEmpty(userInput.Name) && !Regex.Match(userInput.Name,@"[^a-zA-Z0-9\s\.\(\)]").Success)
                {
                    userInput.Name = userInput.Name.Trim();
                    _extPaymentProviderService.TrySaveExtPaymentProvider(userInput, GetUser(_orchardServices.WorkContext.CurrentUser.Id), errors);
                }
                else { errors.Add(new ErrorModel { FieldName = nameof(userInput.Name), ErrorMessage = "Payment provider must have a valid name. i.e only alpha numeric characters" }); throw new DirtyFormDataException(); }

            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<ExternalPaymentProviderHandler, ExternalPaymentProviderController>(callBack,errors);
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Get payment provider list vm
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        public PaymentProviderListVM GetPaymentProviderListVM(PaymentProviderSearchParams searchParams)
        {
            try
            {
                IsAuthorized<ExternalPaymentProviderHandler>(Permissions.ViewExternalPaymentProvider);
                PaymentProviderListVM providers = new PaymentProviderListVM { };

                dynamic recordsAndAggregate = _paymentProviderFilter.GetPaymentProvidersViewModel(searchParams);

                providers.PaymentProviders = ((IEnumerable<ExternalPaymentProviderVM>) recordsAndAggregate.ProviderRecords).ToList();
                providers.TotalProviders = (int) ((IEnumerable<ReportStatsVM>)recordsAndAggregate.Aggregate).First().TotalRecordCount;

                return providers;
            }
            catch (Exception) { throw; }
        }


        /// <summary>
        /// Get client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>APIResponse | null</returns>
        public APIResponse GetClientSecret(string clientId)
        {
            var errorMessage = "";
            try
            {
                return new APIResponse { Error = false, ResponseObject = _extPaymentProviderService.GetClientSecretByClientId(clientId) };
            }
            catch (Exception exception)
            {
                errorMessage = "Error getting client secret for client Id";
                Logger.Error(exception, exception.Message + " Error getting client secret for client Id " + clientId);
            }
            return new APIResponse { Error = true, ResponseObject = errorMessage };
        }


    }
}