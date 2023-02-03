using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.Cashflow.Ng.Models;
using System.Security.Cryptography;
using Parkway.CBS.ReferenceData.Configuration;
using System.Text.RegularExpressions;
using Parkway.CBS.Core.Validations;
using Parkway.CBS.Core.Validations.Contracts;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreSettingsService : CoreBaseService, ICoreSettingsService
    {
        private readonly IOrchardServices _orchardServices;
        private IInvoicingService _invoicingService;
        private readonly IMediaLibraryService _mediaLibraryService;
        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IRefDataConfiguration _refDataConfig;
        private readonly ITenantStateSettings<TenantCBSSettings> _stateSettingsRepo;
        private readonly IRevenueHeadPermissionManager<RevenueHeadPermission> _permissionRepo;
        private readonly IRevenueHeadPermissionConstraintsManager<RevenueHeadPermissionConstraints> _permissionConstraintRepo;
        private readonly IValidator _validator;

        public CoreSettingsService(IOrchardServices orchardServices,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider,
            IInvoicingService invoicingService, IMediaLibraryService mediaLibraryService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, ITenantStateSettings<TenantCBSSettings> stateSettingsRepo, IValidator validator, IRevenueHeadPermissionManager<RevenueHeadPermission> permissionRepo, IRevenueHeadPermissionConstraintsManager<RevenueHeadPermissionConstraints> permissionConstraintRepo) : base(orchardServices, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            _invoicingService = invoicingService;
            Logger = NullLogger.Instance;
            _mediaLibraryService = mediaLibraryService;
            _settingsRepository = settingsRepository;
            _refDataConfig = new RefDataConfiguration();
            _stateSettingsRepo = stateSettingsRepo;
            _validator = validator;
            _permissionRepo = permissionRepo;
            _permissionConstraintRepo = permissionConstraintRepo;
        }



        public void DoExpertSystemAccessListMigration()
        {
            try
            {
                List<ExpertSystemSettings> expertSystems = _settingsRepository.GetCollection().ToList();
                List<RevenueHeadPermissionConstraints> permissions = new List<RevenueHeadPermissionConstraints> { };
                RevenueHeadPermission permission = _permissionRepo.GetCollection(r => r.Name == Models.Enums.EnumExpertSystemPermissions.GenerateInvoice.ToString()).Single();
                foreach (var expertSystem in expertSystems)
                {
                    if (expertSystem.ListOfRevenueHeadAccessList() == null || expertSystem.ListOfRevenueHeadAccessList().Count <= 0) { continue; }
                    foreach (var val in expertSystem.ListOfRevenueHeadAccessList())
                    {
                        permissions.Add(new RevenueHeadPermissionConstraints { RevenueHeadPermission = permission, ExpertSystem = expertSystem, MDA = new MDA { Id = 1 }, RevenueHead = new RevenueHead { Id = val }, LastUpdatedBy = new UserPartRecord { Id = 2 } });
                    }
                }
                //delete all records so we don't have duplicates
                _permissionConstraintRepo.ClearTable();
                //
                _permissionConstraintRepo.SaveBundleUnCommitStatelessWithErrors(permissions);
                _permissionConstraintRepo.UpdateMDAId();
            }
            catch (Exception)
            {
                _permissionConstraintRepo.RollBackAllTransactions();
                throw;
            }
            //clear table
        }


        /// <summary>
        /// Has tenant state settings
        /// </summary>
        /// <returns>bool</returns>
        public TenantCBSSettings HasTenantStateSettings()
        {
            return _stateSettingsRepo.GetState();
        }

        public string GetClientSecret(string clientId)
        {
            return _settingsRepository.GetClientSecretByClientId(clientId);
        }


        public void TrySaveStateSettings(string identifier, int stateId, string stateName, UserPartRecord user)
        {
            Logger.Information("Checking tenant state settings exists");
            var stateSettings = HasTenantStateSettings();
            if (stateSettings != null) { throw new TenantStateHasAlreadyBeenSetException(stateSettings.StateName); }
            Logger.Information("Saving state settings");
            if (!Regex.IsMatch(identifier, @"^[a-zA-Z0-9]+$"))
            { Logger.Error("Identifier did not fulfil validation " + identifier); throw new DirtyFormDataException("Only letters and numbers allowed " + identifier); };

            identifier.Trim();
            if (!_stateSettingsRepo.Save(new TenantCBSSettings { CountryName = "Nigeria", CountryId = 1, Identifier = (identifier.Split(' ')[0] + "_" + stateName.Split(' ')[0]).ToUpper(), AddedBy = user, LastUpdatedBy = user, StateId = stateId, StateName = stateName }))
            { throw new CannotSaveTenantSettings(); }
        }


        /// <summary>
        /// Try save tenant settings
        /// </summary>
        /// <param name="user"></param>
        /// <param name="files"></param>
        /// <param name="errors"></param>
        /// <param name="model"></param>
        /// <param name="stateId"></param>
        /// <param name="bankId"></param>
        /// <returns>TenantSettingsViewModel</returns>
        public void TrySaveNewExpertSystem(UserPartRecord user, HttpFileCollectionBase files, ref List<ErrorModel> errors, ExpertSystemSettings model, int bankId)
        {
            Logger.Information("Getting tenant settings");
            TenantCBSSettings tenantSettings = null;
            //https://stackoverflow.com/questions/2950989/net-regex-for-letters-and-spaces
            if (!Regex.IsMatch(model.BillingSchedulerIdentifier, @"^[a-zA-Z0-9]+$"))
            {
                Logger.Error("Only alphabets are allowed");
                errors.Add(new ErrorModel { ErrorMessage = "Only alphabets without white spaces are allowed", FieldName = "ExpertSystemsSettings.BillingSchedulerIdentifier" });
                throw new DirtyFormDataException("Only alphabets are allowed");
            }
            try
            {
                tenantSettings = HasTenantStateSettings();
                if (tenantSettings == null) { Logger.Information("Cannot find tenant setting"); throw new CannotFindTenantIdentifierException(); }
            }
            catch (Exception exception)
            { Logger.Error(exception, "Error getting tenant state settings"); throw new CannotFindTenantIdentifierException(); }

            Logger.Information("Validating expert system name");
            List<UniqueValidationModel> settingDataValue = new List<UniqueValidationModel>();
            settingDataValue.Add(new UniqueValidationModel { ErrorMessage = "Another Expert system already has this value", Identifier = "ExpertSystemsSettings.BillingSchedulerIdentifier", Name = "BillingSchedulerIdentifier", SelectDataValue = "BillingSchedulerIdentifier:" + model.BillingSchedulerIdentifier.Trim(), InclusiveClauses = new string[] { } });
            errors = ValidateUniqueness<ExpertSystemSettings>(settingDataValue);
            if (errors.Any()) { throw new DirtyFormDataException("Error validating expert system scheduler identifier"); }

            Logger.Information("Getting the list of expert systems");
            ExpertSystemSettings defaultExpertSystem = _settingsRepository.HasRootExpertSystem();

            if (defaultExpertSystem == null) { model.IsRoot = true; }

            var listOfBanks = new List<CashFlowBank>();
            try
            {
                Logger.Information("Getting bank details from cashflow");
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
                var bankService = _invoicingService.BankService(context);
                listOfBanks = bankService.ListOfBanks();
                #endregion
            }
            catch (Exception exception) { Logger.Error(exception, exception.Message); throw new CannotConnectToCashFlowException(exception.Message); }

            var bankObj = new CashFlowBank();
            try { bankObj = listOfBanks.Where(b => b.Id == bankId).Single(); }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Bank ID not found. Id {0}", bankId));
                errors.Add(new ErrorModel { ErrorMessage = "Could not find bank details", FieldName = "ExpertSystemsSettings.TSA" });
                throw new DirtyFormDataException(string.Format("Bank ID not found. Id {0}", bankId));
            }
            //sort ref data
            Logger.Information("Getting ref data");
            List<RefData> listOfRefData = _refDataConfig.GetCollection(tenantSettings.Identifier);
            try
            {
                RefData refData = listOfRefData.Where(rfd => rfd.Name == model.ReferenceDataSourceName).SingleOrDefault();
                if (refData == null)
                {
                    Logger.Error("No reference data with found " + model.ReferenceDataSourceName);
                    errors.Add(new ErrorModel { FieldName = "ExpertSystemsSettings.ReferenceDataSourceName", ErrorMessage = ErrorLang.couldnotfindmatchingrefdata().ToString() });
                    throw new DirtyFormDataException("No reference data with found " + model.ReferenceDataSourceName);
                }

                switch (refData.Type)
                {
                    case "Parkway":
                        model.ReferenceDataSourceType = "Parkway";
                        break;
                    case "Adapter":
                        model.ReferenceDataSourceType = "Adapter";
                        model.AdapterClassName = refData.ClassName;
                        break;
                    case "Mock":
                        model.ReferenceDataSourceType = "Mock";
                        break;
                    default:
                        errors.Add(new ErrorModel { FieldName = "ExpertSystemsSettings.ReferenceDataSourceName", ErrorMessage = ErrorLang.couldnotfindmatchingrefdata().ToString() });
                        throw new DirtyFormDataException("No reference data with found " + model.ReferenceDataSourceName);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "ExpertSystemsSettings.ReferenceDataSourceName", ErrorMessage = ErrorLang.couldnotfindmatchingrefdata().ToString() });
                throw new DirtyFormDataException("No reference data with found " + model.ReferenceDataSourceName);
            }

            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();

            for (var i = 0; i < files.Count; i++)
            { filesAndFileNames.Add(new UploadedFileAndName { Upload = files[i], UploadName = files.GetKey(i) }); }

            Logger.Information("Validating files");
            CheckFileType(filesAndFileNames, errors);
            CheckFileSize(filesAndFileNames, errors);

            if (errors.Count > 0) { throw new DirtyFormDataException(); }
            Logger.Information("Creating folder and storing files");
            CreateFolders("CashFlow", filesAndFileNames.Select(f => f.UploadName).ToArray());

            Orchard.MediaLibrary.Models.MediaPart mediaPartLogo = SaveMedia("CashFlow\\DefaultLogoPath", files[0], true);
            model.LogoPath = _mediaLibraryService.GetMediaPublicUrl(mediaPartLogo.FolderPath, mediaPartLogo.FileName);

            Orchard.MediaLibrary.Models.MediaPart mediaPartSignature = SaveMedia("CashFlow\\DefaultSignaturePath", files[1], true);
            model.SignaturePath = _mediaLibraryService.GetMediaPublicUrl(mediaPartSignature.FolderPath, mediaPartSignature.FileName);

            Logger.Information("Loading up persist model");
            //get file collection
            model.TSA = bankId; model.LastUpdatedBy = user;
            model.AddedBy = user;

            model.BillingSchedulerIdentifier = model.BillingSchedulerIdentifier.ToUpper();
            model.BankCode = bankObj.Code;
            model.CompanyEmail.Trim(); model.CompanyAddress.Trim();
            model.BusinessNature.Trim(); model.CompanyMobile.Trim();
            model.TenantCBSSettings = tenantSettings; model.IsActive = true;
            model.CompanyName.Trim().ToUpper();
            GenerateAuthCredentials(tenantSettings, model).SaveRecord(model);
        }


        /// <summary>
        /// try update expert system
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <param name="files"></param>
        /// <param name="errors"></param>
        /// <param name="bankId"></param>
        /// <returns>ExpertSettingsViewModel</returns>
        public ExpertSettingsViewModel TryUpdateExpertSystemSettings(string expertSystemIentifier, ExpertSystemSettings updatedModel, UserPartRecord user, HttpFileCollectionBase files, ref List<ErrorModel> errors, int bankId)
        {
            Logger.Information("Updating expert system");
            //get expert system
            Logger.Information("Validating new billing scheduler identifier");
            //https://stackoverflow.com/questions/2950989/net-regex-for-letters-and-spaces
            if (!Regex.IsMatch(updatedModel.BillingSchedulerIdentifier, @"^[a-zA-Z0-9]+$"))
            {
                Logger.Error("Only alphabets are allowed");
                errors.Add(new ErrorModel { ErrorMessage = "Only alphabets without white spaces are allowed", FieldName = "ExpertSystemsSettings.BillingSchedulerIdentifier" });
                throw new DirtyFormDataException("Only alphabets are allowed");
            }

            ExpertSystemSettings expertSystem = GetExpertSystem(expertSystemIentifier);
            if (expertSystem == null) { throw new TenantNotFoundException(); }
            //get list of banks
            Logger.Information("Getting tenant state");
            TenantCBSSettings tenantIdentifier = expertSystem.TenantCBSSettings;

            if (tenantIdentifier == null) { { Logger.Information("Cannot find tenant setting for identifier " + expertSystemIentifier); throw new CannotFindTenantIdentifierException(); } }

            //validating that the new name has not been take
            Logger.Information("Validating that the neww billing scheduler name has not been taken");
            List<UniqueValidationModel> settingDataValue = new List<UniqueValidationModel>();
            settingDataValue.Add(new UniqueValidationModel { ErrorMessage = "Another Expert system already has this value", Identifier = "ExpertSystemsSettings.BillingSchedulerIdentifier", Name = "BillingSchedulerIdentifier", SelectDataValue = "BillingSchedulerIdentifier:" + updatedModel.BillingSchedulerIdentifier.Trim(), InclusiveClauses = new string[] { "Id:true:" + expertSystem.Id } });
            errors = ValidateUniqueness<ExpertSystemSettings>(settingDataValue);
            if (errors.Any()) { throw new DirtyFormDataException("Error validating expert system scheduler identifier. Anothher expert system has the value"); }

            var listOfBanks = new List<CashFlowBank>();
            try
            {
                Logger.Information("Getting bank details from cashflow");
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
                var bankService = _invoicingService.BankService(context);
                listOfBanks = bankService.ListOfBanks();
                #endregion
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                throw new CannotConnectToCashFlowException("Error connecting to cashflow");
            }
            CashFlowBank bankObj = null;
            try
            {
                bankObj = listOfBanks.Where(b => b.Id == bankId).Single();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Bank ID not found. Id {0}", bankId));
                errors.Add(new ErrorModel { ErrorMessage = "Could not find bank details", FieldName = "ExpertSystemsSettings.TSA" });
                throw new DirtyFormDataException(string.Format("Bank ID not found. Id {0}", bankId));
            }

            updatedModel.TSA = bankId;
            updatedModel.BankCode = bankObj.Code; updatedModel.LastUpdatedBy = user;
            //
            Logger.Information("Getting ref data");
            List<RefData> listOfRefData = _refDataConfig.GetCollection(tenantIdentifier.Identifier);
            try
            {
                RefData refData = listOfRefData.Where(rfd => rfd.Name == updatedModel.ReferenceDataSourceName).SingleOrDefault();
                if (refData == null)
                {
                    Logger.Error("No reference data with found " + updatedModel.ReferenceDataSourceName);
                    errors.Add(new ErrorModel { FieldName = "ExpertSystemsSettings.ReferenceDataSourceName", ErrorMessage = ErrorLang.couldnotfindmatchingrefdata().ToString() });
                    throw new DirtyFormDataException("No reference data with found " + updatedModel.ReferenceDataSourceName);
                }

                switch (refData.Type)
                {
                    case "Parkway":
                        updatedModel.ReferenceDataSourceType = "Parkway";
                        break;
                    case "Adapter":
                        updatedModel.ReferenceDataSourceType = "Adapter";
                        updatedModel.AdapterClassName = refData.ClassName;
                        break;
                    case "Mock":
                        updatedModel.ReferenceDataSourceType = "Mock";
                        break;
                    default:
                        errors.Add(new ErrorModel { FieldName = "ExpertSystemsSettings.ReferenceDataSourceName", ErrorMessage = ErrorLang.couldnotfindmatchingrefdata().ToString() });
                        throw new DirtyFormDataException("No reference data with found " + updatedModel.ReferenceDataSourceName);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errors.Add(new ErrorModel { FieldName = "ExpertSystemsSettings.ReferenceDataSourceName", ErrorMessage = ErrorLang.couldnotfindmatchingrefdata().ToString() });
                throw new DirtyFormDataException("No reference data with found " + updatedModel.ReferenceDataSourceName);
            }

            //
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();
            for (var i = 0; i < files.Count; i++)
            {
                filesAndFileNames.Add(new UploadedFileAndName { Upload = files[i], UploadName = files.GetKey(i) });
            }

            if (files[0].ContentLength > 0)
            {
                CheckFileType(new List<UploadedFileAndName> { { filesAndFileNames.ElementAt(0) } }, errors);
                CheckFileSize(new List<UploadedFileAndName> { { filesAndFileNames.ElementAt(0) } }, errors);
                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                CreateFolders("CashFlow", new string[] { files.GetKey(0) });
                Orchard.MediaLibrary.Models.MediaPart mediaPartLogo = SaveMedia("CashFlow\\DefaultLogoPath", files[0], true);
                expertSystem.LogoPath = _mediaLibraryService.GetMediaPublicUrl(mediaPartLogo.FolderPath, mediaPartLogo.FileName);
            }

            if (files[1].ContentLength > 0)
            {
                CheckFileType(new List<UploadedFileAndName> { { filesAndFileNames.ElementAt(1) } }, errors);
                CheckFileSize(new List<UploadedFileAndName> { { filesAndFileNames.ElementAt(1) } }, errors);
                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                CreateFolders("CashFlow", new string[] { files.GetKey(1) });
                Orchard.MediaLibrary.Models.MediaPart mediaPartSignature = SaveMedia("CashFlow\\DefaultSignaturePath", files[1], true);
                expertSystem.SignaturePath = _mediaLibraryService.GetMediaPublicUrl(mediaPartSignature.FolderPath, mediaPartSignature.FileName);
            }
            UpdateRecord(updatedModel, expertSystem);
            ExpertSettingsViewModel viewModel = new ExpertSettingsViewModel
            {
                ExpertSystemsSettings = expertSystem,
                States = new List<CashFlowState> { { new CashFlowState { Id = tenantIdentifier.StateId, Name = tenantIdentifier.StateName } } },
                Banks = listOfBanks
            };
            return viewModel;
        }


        /// <summary>
        /// Validate that the records in datavalues are unique
        /// </summary>
        /// <typeparam name="M">MDARevenueHead</typeparam>
        /// <param name="dataValues">List{UniqueValidationModel}</param>
        /// <returns>List{ErrorModel}</returns>
        private List<ErrorModel> ValidateUniqueness<T>(List<UniqueValidationModel> settingDataValue)
        {
            if (!settingDataValue.Any()) return new List<ErrorModel>();
            var validator = _validator.BundleUnique<ExpertSystemSettings>(settingDataValue);
            return validator.HasValidationErrors();
        }


        /// <summary>
        /// Get ref data options
        /// </summary>
        /// <returns></returns>
        public List<string> GetRefData(string identifier)
        {
            //get available ref data types from config
            return _refDataConfig.GetCollection(identifier).Select(rf => rf.Name).ToList();
        }


        /// <summary>
        /// Save tenant settings record
        /// </summary>
        /// <param name="model"></param>
        private void SaveRecord(ExpertSystemSettings model)
        {
            if (!_settingsRepository.Save(model)) { throw new CannotSaveTenantSettings(); }
        }


        /// <summary>
        /// Generate insert the client id and secret into a CBSTenantSettings model
        /// </summary>
        /// <param name="model">CBSTenantSettings</param>
        /// <returns>CoreSettingsService</returns>
        private CoreSettingsService GenerateAuthCredentials(TenantCBSSettings tenantSettings, ExpertSystemSettings model)
        {
            model.ClientId = OnewayHashThis(tenantSettings.Identifier + model.CompanyName + DateTime.UtcNow.ToString(), tenantSettings.StateName);

            RandomNumberGenerator cryptoRandomDataGenerator = new RNGCryptoServiceProvider();
            byte[] buffer = new byte[45];
            cryptoRandomDataGenerator.GetBytes(buffer);
            model.ClientSecret = Convert.ToBase64String(buffer);
            return this;
        }

        
        /// <summary>
        /// Persist tenant record
        /// </summary>
        /// <param name="model">updated tenant model</param>
        /// <param name="persistedModel">persisted tenant model</param>
        private void UpdateRecord(ExpertSystemSettings model, ExpertSystemSettings persistedModel)
        {
            persistedModel.CompanyName = model.CompanyName;
            persistedModel.CompanyMobile = model.CompanyMobile;
            persistedModel.CompanyEmail = model.CompanyEmail;
            persistedModel.CompanyAddress = model.CompanyAddress;
            persistedModel.BusinessNature = model.BusinessNature;
            persistedModel.TSA = model.TSA;
            persistedModel.TSABankNumber = model.TSABankNumber;
            persistedModel.BankCode = model.BankCode;
            if (!_settingsRepository.Update(persistedModel)) { throw new CannotUpdateTenantSettingsException(); }
        }


        /// <summary>
        /// Try save reference data settings
        /// </summary>
        /// <param name="model">ReferenceDataViewModel</param>
        public void TrySaveReferenceDataSettings(ExpertSystemSettings tenant, ReferenceDataViewModel model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ExpertSystemSettings> GetExpertSystems(int take, int skip)
        {
            return _settingsRepository.GetExpertSystemList(take, skip);
        }


        /// <summary>
        /// Get expert system
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns>ExpertSystemSettings</returns>
        public ExpertSystemSettings GetExpertSystem(string identifier)
        {
            try
            { return _settingsRepository.GetCollection(xpt => xpt.BillingSchedulerIdentifier == identifier).SingleOrDefault(); }
            catch (Exception exception)
            { Logger.Error(exception, string.Format("Error getting expert system {0} {1}", identifier, exception.Message)); return null; }
        }       
    }
}