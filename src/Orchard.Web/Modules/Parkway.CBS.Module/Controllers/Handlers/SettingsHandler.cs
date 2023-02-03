using Orchard;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Orchard.Logging;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;

namespace Parkway.CBS.Module.Controllers.Handlers
{
    public class SettingsHandler : BaseHandler, ISettingsHandler
    {
        private readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        public IInvoicingService _invoicingService;
        private readonly ICoreSettingsService _coreSettingsService;

        public SettingsHandler(IOrchardServices orchardServices, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IInvoicingService invoicingService, ICoreSettingsService coreSettingsService) : base(orchardServices, settingsRepository)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _settingsRepository = settingsRepository;
            _invoicingService = invoicingService;
            _coreSettingsService = coreSettingsService;
        }

        public ChangePasswordViewModel ChangePasswordView()
        {
            return new ChangePasswordViewModel() { };
        }


        /// <summary>
        /// Get client secret
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>string | null</returns>
        public string GetClientSecret(string clientId)
        {
            return _coreSettingsService.GetClientSecret(clientId);
        }


        /// <summary>
        /// Get the view to set state
        /// </summary>
        /// <returns>SetStateViewModel</returns>
        public SetStateViewModel SetStateView()
        {
            Logger.Information("Check permissions");
            IsAuthorized<SettingsHandler>(Permissions.ManageAdminSettings);
            Logger.Information("Checking to see if the state has already been chosen");
            TenantCBSSettings tenantStateSettings = _coreSettingsService.HasTenantStateSettings();
            if (tenantStateSettings != null) { throw new TenantStateHasAlreadyBeenSetException(tenantStateSettings.StateName); }
            Logger.Information("No state chosen yet, calling cashflow");
            List<CashFlowState> states = ListOfStates(null);
            return new SetStateViewModel { States = states.Select(state => new TenantCBSSettings { StateName = state.Name, StateId = state.Id }).ToList() };
        }


        /// <summary>
        /// Try save tenant state settings
        /// </summary>
        /// <param name="settingsController"></param>
        /// <param name="stateId">state Id</param>
        public void TrySaveTenantStateSettings(SettingsController callback, string sstateId, string identifier)
        {
            Logger.Information("Check permissions to save state settings");
            IsAuthorized<SettingsHandler>(Permissions.ManageAdminSettings);
            var user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);

            int stateId = 0;
            var parsed = Int32.TryParse(sstateId, out stateId);
            if (!parsed) { throw new CouldNotParseStringValueException(string.Format("Could not parse state string value {0}", sstateId)); }
            List<CashFlowState> states = new List<CashFlowState>();
            try
            {
                states = ListOfStates(null);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error from cashflow getting states");
                throw new CannotConnectToCashFlowException();
            }
            string stateName = null;
            try
            {
                stateName = states.Where(s => s.Id == stateId).Single().Name;
            }
            catch (Exception exception)
            { Logger.Error(exception, "One or more states with the same Id or none was found for stateid " + stateId); throw new CannotFindTenantIdentifierException("Could not find state with Id " + stateId); }

            if (string.IsNullOrEmpty(stateName)) { throw new CannotFindTenantIdentifierException("Could not find state with Id " + stateId); }

            try
            {
                _coreSettingsService.TrySaveStateSettings(identifier.Trim(), stateId, stateName, user);
            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<SettingsHandler, SettingsController>(callback, new List<ErrorModel> { { new ErrorModel { ErrorMessage = "Only letters without white space allowed", FieldName = "Identifier" } } });
            }
        }


        public List<CashFlowState> ListOfStates(CashFlowRequestContext context)
        {
            Logger.Information("Getting list of states from cashflow");
            if (context == null) { context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } }); }
            var statesService = _invoicingService.StateService(context);
            return statesService.ListOfStates();
        }

        public List<CashFlowBank> ListOfBanks(CashFlowRequestContext context)
        {
            Logger.Information("Getting list of banks from cashflow");
            if (context == null) { context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } }); }
            var bankService = _invoicingService.BankService(context);
            return bankService.ListOfBanks();
        }

        public List<string> GetRegisteredRefData(string stateName)
        {
            return _coreSettingsService.GetRefData(stateName);
        }


        /// <summary>
        /// Get expert systems view model
        /// </summary>
        /// <returns>ExpertSettingsViewModel</returns>
        public ExpertSystemListViewModel GetListOfExpertSystemsView(int skip, int take)
        {
            Logger.Information("Checking permission");
            IsAuthorized<SettingsHandler>(Permissions.ManageAdminSettings);
            //check if the state setting has been added
            Logger.Information("Checking if state settings has been set");
            var tenantStateSettings = _coreSettingsService.HasTenantStateSettings();

            if (tenantStateSettings == null)
            { return new ExpertSystemListViewModel { ShowSetStateButton = true, ExpertSystemSettings = new List<ExpertSystemSettings>() }; }
            var count = _settingsRepository.Count(s => s.Id != 0);
            IEnumerable<ExpertSystemSettings> expertSystemCollection = _coreSettingsService.GetExpertSystems(take, skip);
                //_core_settingsRepository.GetCollection(s => s.Id != 0, );
            if (expertSystemCollection == null) { return new ExpertSystemListViewModel { ExpertSystemSettings = new List<ExpertSystemSettings>() }; }

            return new ExpertSystemListViewModel { ExpertSystemSettings = expertSystemCollection.ToList(), PagerSize = count };
        }


        /// <summary>
        /// Get tenant settings 
        /// </summary>
        /// <returns>TenantCBSSettings</returns>
        public TenantCBSSettings GetTenantSettings()
        {
            Logger.Information("Getting tenant state settings");
            TenantCBSSettings tenantIdentifier = null;
            try
            { tenantIdentifier = _coreSettingsService.HasTenantStateSettings(); }
            catch (Exception exception)
            { Logger.Error(exception, "No tenant identifier found"); throw new CannotFindTenantIdentifierException("No tenant identifier found"); }

            if (tenantIdentifier == null) { throw new CannotFindTenantIdentifierException("No tenant identifier found"); }
            return tenantIdentifier;
        }


        /// <summary>
        /// Get view model to create expert system
        /// </summary>
        /// <returns>ExpertSettingsViewModel</returns>
        public ExpertSettingsViewModel CreateExpertSystemView()
        {
            //check for authorization
            Logger.Information("Checking permission to create expert system");
            IsAuthorized<SettingsHandler>(Permissions.ManageAdminSettings);
            //get list of banks
            Logger.Information("Getting tenant state");
            TenantCBSSettings tenantIdentifier = null;
            try
            { tenantIdentifier = _coreSettingsService.HasTenantStateSettings(); }
            catch (Exception exception)
            { Logger.Error(exception, "No tenant identifier found"); throw new CannotFindTenantIdentifierException("No tenant identifier found"); }

            if (tenantIdentifier == null) { throw new CannotFindTenantIdentifierException("No tenant identifier found"); }
            List<CashFlowBank> listOfBanks = new List<CashFlowBank>();
            #region CASHFLOW 
            try
            {
                Logger.Information("Getting list of banks from cashflow");
                listOfBanks = ListOfBanks(null);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw new CannotConnectToCashFlowException(exception.Message + exception.StackTrace);
            }
            #endregion

            //gett list of ref data
            Logger.Information("Getting list of ref data attached to the state");
            List<string> refData = GetListOfRefData(tenantIdentifier.Identifier);

            return new ExpertSettingsViewModel
            {
                Banks = listOfBanks,
                States = new List<CashFlowState> { { new CashFlowState { Id = tenantIdentifier.StateId, Name = tenantIdentifier.StateName } } },
                ExpertSystemsSettings = new ExpertSystemSettings { },
                ListOfRefData = refData
            };
        }


        /// <summary>
        /// Get view for edit expert system
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>ExpertSettingsViewModel</returns>
        public ExpertSettingsViewModel EditExpertSystemView(string identifier)
        {
            //check for authorization
            Logger.Information("Checking permission to create expert system");
            IsAuthorized<SettingsHandler>(Permissions.ManageAdminSettings);
            //get expert system
            ExpertSystemSettings expertSystem = _coreSettingsService.GetExpertSystem(identifier);
            if (expertSystem == null) { throw new TenantNotFoundException(); }
            //get list of banks
            Logger.Information("Getting tenant state");
            TenantCBSSettings tenantIdentifier = expertSystem.TenantCBSSettings;

            if (tenantIdentifier == null) { throw new CannotFindTenantIdentifierException("No tenant identifier found"); }
            List<CashFlowBank> listOfBanks = new List<CashFlowBank>();
            #region CASHFLOW 
            try
            {
                Logger.Information("Getting list of banks from cashflow");
                listOfBanks = ListOfBanks(null);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw new CannotConnectToCashFlowException(exception.Message + exception.StackTrace);
            }
            #endregion

            //gett list of ref data
            Logger.Information("Getting list of ref data attached to the state");
            List<string> refData = GetListOfRefData(tenantIdentifier.Identifier);

            return new ExpertSettingsViewModel
            {
                Banks = listOfBanks,
                States = new List<CashFlowState> { { new CashFlowState { Id = tenantIdentifier.StateId, Name = tenantIdentifier.StateName } } },
                ExpertSystemsSettings = expertSystem,
                ListOfRefData = refData,
                IsEdit = true,
            };
        }        


        /// <summary>
        /// Try save tenant settings
        /// </summary>
        /// <param name="settingsController">SettingsController</param>
        /// <param name="model">CBSTenantSettings</param>
        /// <param name="files">HttpFileCollectionBase</param>
        /// <param name="state">string</param>
        /// <param name="bank">string</param>
        /// <returns>TenantSettingsViewModel</returns>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        /// <exception cref="CouldNotParseStringValueException"></exception>
        public ExpertSettingsViewModel TrySaveNewExpertSystem(SettingsController callback, ExpertSystemSettings model, HttpFileCollectionBase files, string sbank)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            ExpertSettingsViewModel viewModel = new ExpertSettingsViewModel();
            Logger.Information("Checking permission for create new expert system and validating model");
            IsAuthorized<SettingsHandler>(Permissions.ManageAdminSettings).IsModelValid<SettingsHandler, SettingsController>(callback);
            try
            {
                Logger.Information("Getting user data");
                var user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
                Logger.Information("Validating parsing bank id string");
                int bankId = 0;
                if (!Int32.TryParse(sbank, out bankId))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Could not find bank details", FieldName = "ExpertSystemsSettings.TSA" });
                    throw new DirtyFormDataException(string.Format("Could not parse bank string value {0}", sbank));
                }
                _coreSettingsService.TrySaveNewExpertSystem(user, files, ref errors, model, bankId);
            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<SettingsHandler, SettingsController>(callback, errors);
            }
            return viewModel;
        }


        /// <summary>
        /// Try update expert system
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="model"></param>
        /// <param name="files"></param>
        /// <param name="bank"></param>
        /// <returns></returns>
        public ExpertSettingsViewModel TryUpdateExpertSystemSettings(string identifier, SettingsController callback, ExpertSettingsViewModel model, HttpFileCollectionBase files, string sbank)
        {
            Logger.Information("Updating expert system " + model.ExpertSystemsSettings.BillingSchedulerIdentifier);
            Logger.Information("Validating model and permissions");
            IsAuthorized<SettingsHandler>(Permissions.ManageAdminSettings).IsModelValid<SettingsHandler, SettingsController>(callback);
            Logger.Information("Getting user info");
            var user = GetUser(_orchardServices.WorkContext.CurrentUser.Id);
            List<ErrorModel> errors = new List<ErrorModel>();
            ExpertSettingsViewModel viewModel = new ExpertSettingsViewModel();

            int bankId = 0;
            if (!Int32.TryParse(sbank, out bankId)) { throw new CouldNotParseStringValueException(string.Format("Could not parse bank value", sbank)); }

            try
            {
                viewModel = _coreSettingsService.TryUpdateExpertSystemSettings(identifier, model.ExpertSystemsSettings, user, files, ref errors, bankId);
            }
            catch (DirtyFormDataException)
            {
                AddValidationErrorsToCallback<SettingsHandler, SettingsController>(callback, errors);
            }
            return viewModel;
        }

        /// <summary>
        /// Get ref data collection
        /// </summary>
        /// <returns>List{string}</returns>
        public List<string> GetListOfRefData(string identifier)
        {
            return _coreSettingsService.GetRefData(identifier);
        }


        /// <summary>
        /// Get view for reference data
        /// </summary>
        /// <returns>ReferenceDataViewModel</returns>
        public ReferenceDataViewModel GetReferenceDataSettingsView()
        {
            Logger.Error("Getting reference data view");
            ExpertSystemSettings tenant = null;
            return new ReferenceDataViewModel { RefData = GetListOfRefData(tenant.TenantCBSSettings.StateName), StateName = tenant.TenantCBSSettings.StateName };
        }


        /// <summary>
        /// Try save reference data settings
        /// </summary>
        /// <param name="model"></param>
        public void TrySaveReferenceDataSettings(ReferenceDataViewModel model)
        {
            ExpertSystemSettings tenant = null;
            _coreSettingsService.TrySaveReferenceDataSettings(tenant, model);
        }
    }
}