using Orchard;
using Orchard.Autoroute.Services;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Orchard.Security;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.Validations;
using Parkway.CBS.Core.Validations.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using Orchard.Users.Models;
using Parkway.CBS.Core.HTTP.RemoteClient.Contracts;
using Orchard.Modules.Services;
using Parkway.CBS.Core.Models.Enums;
using Newtonsoft.Json;
using System.Collections.Specialized;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Core.StateConfig;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreMDAService : CoreMDARevenueHeadService, ICoreMDAService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthorizer _authorizer;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IMimeTypeProvider _mimeTypeProvider;
        public IInvoicingService _invoicingService;
        private readonly IMDAManager<MDA> _mdaRepository;

        private readonly IValidator _validator;
        private readonly ISlugService _slugService;

        public IAdminSettingManager<ExpertSystemSettings> _settingsRepository;
        private readonly IAPIRequestManager<APIRequest> _apiRequest;

        public CoreMDAService(IOrchardServices orchardServices, IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider, IInvoicingService invoicingService, IMDAManager<MDA> mdaRepository, IValidator validator, ISlugService slugService, IAdminSettingManager<ExpertSystemSettings> settingsRepository, IAPIRequestManager<APIRequest> apiRequest) : base(orchardServices, slugService, validator, mediaManagerService, mimeTypeProvider)
        {
            _orchardServices = orchardServices;
            _authorizer = orchardServices.Authorizer;
            _mediaLibraryService = mediaManagerService;
            _mimeTypeProvider = mimeTypeProvider;
            Logger = NullLogger.Instance;
            _invoicingService = invoicingService;
            _mdaRepository = mdaRepository;
            _validator = validator;
            _slugService = slugService;
            _settingsRepository = settingsRepository;
            _apiRequest = apiRequest;
        }



        #region Create

        /// <summary>
        /// Try save MDA record
        /// </summary>
        /// <param name="expertSystem"></param>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <param name="errors"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public MDACreatedModel TrySaveMDA(ExpertSystemSettings expertSystem, MDA model, UserPartRecord user, ref List<ErrorModel> errors, HttpFileCollectionBase files, string requestReference = null)
        {
            if (!string.IsNullOrEmpty(requestReference))
            {
                Logger.Error(string.Format("Checking request reference if any client ID: {0} Ref: {1}", expertSystem.ClientId, requestReference ?? ""));
                Int64 refResult = _apiRequest.GetResourseIdentifier(requestReference, expertSystem, CallTypeEnum.MDA);
                if (refResult != 0)
                {
                    Logger.Error(string.Format("Request reference found for client ID: {0} Ref: {1} MDA: {2}", expertSystem.ClientId, requestReference, refResult));
                    var persistedMDA = _mdaRepository.Get((int)refResult);
                    if (persistedMDA != null)
                    { Logger.Error(string.Format("Returing ref {0}", requestReference)); return new MDACreatedModel { CashFlowCompanyKey = persistedMDA.SMEKey, CBSId = persistedMDA.Id, MDASlug = persistedMDA.Slug }; }
                }
            }
            model.AddedBy = user;
            model.LastUpdatedBy = user;
            var savedMDA = SaveMDA(model, expertSystem, files, ref errors, requestReference);
            return new MDACreatedModel { CashFlowCompanyKey = savedMDA.SMEKey, CBSId = savedMDA.Id, MDASlug = savedMDA.Slug };
        }

        #endregion

              

        #region update


        public MDAEditedModel TryUpdate(ExpertSystemSettings tenant, MDA updatedMDA, int mdaId, UserPartRecord user, ref List<ErrorModel> errors, HttpFileCollectionBase files, string mdaSlug = "")
        {
            Logger.Information("model " + JsonConvert.SerializeObject(updatedMDA));
            MDA mda = new MDA();
            if (mdaId != 0) { mda = GetMDA(mdaId); }
            else { mda = GetMDA(mdaSlug); }

            UpdateMDARecord(updatedMDA, tenant, mda, files, user, ref errors);
            return new MDAEditedModel { CashFlowCompanyKey = mda.SMEKey, CBSId = mda.Id, MDASlug = mda.Slug };
        }

        #endregion



        /// <summary>
        /// save the mda record
        /// </summary>
        /// <typeparam name="H"></typeparam>
        /// <param name="mda"></param>
        /// <param name="expertSystemSettings"></param>
        /// <param name="files"></param>
        /// <param name="errors"></param>
        /// <returns>MDA</returns>
        private MDA SaveMDA(MDA mda, ExpertSystemSettings expertSystemSettings, HttpFileCollectionBase files, ref List<ErrorModel> errors, string requestReference = null)
        {
            Logger.Information(string.Format("Saving record"));
            Logger.Information("Getting state settings");
            TenantCBSSettings stateSettings = expertSystemSettings.TenantCBSSettings;
            if(stateSettings == null) { throw new TenantNotFoundException("No state settings found"); }
            //trim mda settings name and code fields
            TrimString<MDA>(new List<MDA>(1) { mda });
            TrimData(mda.MDASettings);
            try
            {
                Logger.Information("Validating uniqueness");
                //perform uniqueness test
                List<UniqueValidationModel> dataValues = new List<UniqueValidationModel>();
                //get the validation model
                dataValues = GetValidationModel(new List<MDA>() { mda });
                //validate that the mda record given is unique
                errors = ValidateUniqueness<MDA>(dataValues);                
                //validate that the mda email is unique
                List<UniqueValidationModel> settingDataValue = new List<UniqueValidationModel>();
                settingDataValue.Add(new UniqueValidationModel { ErrorMessage = "Another MDA already has this email", Identifier = "MDA.MDASettings.CompanyEmail", Name = "CompanyEmail", SelectDataValue = "CompanyEmail:" + mda.MDASettings.CompanyEmail, InclusiveClauses = new string[] { } });
                errors = ValidateUniqueness<MDASettings>(settingDataValue);
            }
            catch (MissingFieldException)
            { Logger.Error("Missing field exception"); errors.Add(new ErrorModel { ErrorMessage = "Missing validation field", FieldName = "MDA.Name" }); }

            //check if the action performed above chucked out errors
            if (errors.Count > 0) { throw new DirtyFormDataException("Error validating uniqueness"); }
            //set MDA slug
            SetSlug<MDA>(new List<MDA>(1) { mda });
            //process media files
            errors = ValidateFiles(expertSystemSettings, files, mda, null);
            //check if the action performed above chucked out errors
            if (errors.Count > 0) { throw new DirtyFormDataException("Error validating files"); }
            mda.MDASettings.BusinessNature = expertSystemSettings.BusinessNature;
            mda.ExpertSystemSettings = expertSystemSettings;

            GetCashFlowKeys(mda, expertSystemSettings, stateSettings);
            APIRequest apiRef = SaveMDA(mda, requestReference, expertSystemSettings);

            return mda;
        }


        /// <summary>
        /// Get MDA record
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns>MDA</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        private MDA GetMDA(int mdaId)
        {
            MDA mda = _mdaRepository.Get(mdaId);
            if (mda == null) { throw new MDARecordNotFoundException(string.Format("Could not find MDA record with MDAId {0}", mdaId)); }
            return mda;
        }


        /// <summary>
        /// Get MDA record
        /// </summary>
        /// <param name="mdaId"></param>
        /// <returns>MDA</returns>
        /// <exception cref="MDARecordNotFoundException"></exception>
        private MDA GetMDA(string mdaSlug)
        {
            MDA mda = _mdaRepository.Get("Slug", mdaSlug);
            if (mda == null) { throw new MDARecordNotFoundException(string.Format("Could not find MDA record with MDAId {0}", mdaSlug)); }
            return mda;
        }

        private void UpdateMDARecord(MDA updatedMDA, ExpertSystemSettings expertSystem, MDA mda, HttpFileCollectionBase files, UserPartRecord user, ref List<ErrorModel> errors)
        {
            Logger.Information("Updating mda record");
            
            if (mda == null) { throw new MDARecordNotFoundException(ErrorLang.mdacouldnotbefound().ToString()); }
            MDASettings mdaSettings = mda.MDASettings;
            if (mdaSettings == null) { throw new MDARecordNotFoundException(ErrorLang.mdasetting404(mda.Name, mda.Code).ToString()); }
            BankDetails bankDetails = mda.BankDetails;
            if (bankDetails == null) { throw new MDARecordNotFoundException(ErrorLang.bankdetails404(mda.Name, mda.Code).ToString()); }

            List<MDA> collection = new List<MDA>(1) { updatedMDA };
            Logger.Information("Validating unqueness of name and code");
            List<UniqueValidationModel> dataValues = new List<UniqueValidationModel>();
            dataValues = GetValidationModel(collection, new string[] { "Id:true:" + mda.Id });

            errors = ValidateUniqueness<MDA>(dataValues);

            Logger.Information("Validating unqueness of email");
            //validate that the mda email is unique
            List<UniqueValidationModel> settingDataValue = new List<UniqueValidationModel>();
            settingDataValue.Add(new UniqueValidationModel { ErrorMessage = "Another MDA already has this email", Identifier = "MDASettings.CompanyEmail", Name = "CompanyEmail", SelectDataValue = "CompanyEmail:" + updatedMDA.MDASettings.CompanyEmail, InclusiveClauses = new string[] { "Id:true:" + mdaSettings.Id } });

            errors = ValidateUniqueness<MDASettings>(settingDataValue);

            if (errors.Count > 0) { throw new DirtyFormDataException(); }

            Logger.Information("Editing logo and signature paths");
            updatedMDA.MDASettings.LogoPath = mda.MDASettings.LogoPath;
            updatedMDA.MDASettings.SignaturePath = mda.MDASettings.SignaturePath;

            if (files != null && files.Count > 0)
            {
                string folderBase = !string.IsNullOrEmpty(mda.MDASettings.LogoPath) ? mda.MDASettings.LogoPath : mda.MDASettings.SignaturePath;
                //if folderbase is not null or empty, get the base folder name
                if (!string.IsNullOrEmpty(folderBase))
                {
                    string[] segs = folderBase.Split('/');
                    folderBase = segs[segs.Length - 3];
                }
                errors = ValidateFiles(expertSystem, files, updatedMDA, folderBase);
                if (errors.Count > 0) { throw new DirtyFormDataException(); }
            }

            TrimString<MDA>(collection); TrimData(updatedMDA.MDASettings); SetSlug(collection);
            var hasLogoChanged = false;
            var hasSignatureChanged = false;

            string newLogoPath = (mda.MDASettings.LogoPath == updatedMDA.MDASettings.LogoPath) ? null : updatedMDA.MDASettings.LogoPath;
            string newSignaturePath = (mda.MDASettings.SignaturePath == updatedMDA.MDASettings.SignaturePath) ? null : updatedMDA.MDASettings.SignaturePath;

            if (!string.IsNullOrEmpty(newLogoPath)) { mda.MDASettings.LogoPath = newLogoPath; hasLogoChanged = true; }
            if (!string.IsNullOrEmpty(newSignaturePath)) { mda.MDASettings.SignaturePath = newSignaturePath; hasSignatureChanged = true; }

            bool hasBankDetailsChanged = false;
            bool hasNameChanged = updatedMDA.Name != mda.Name;
            bool hasEmailChanged = updatedMDA.MDASettings.CompanyEmail != mda.MDASettings.CompanyEmail;
            bool hasAddressChanged = updatedMDA.MDASettings.CompanyAddress != mda.MDASettings.CompanyAddress;
            bool hasMobileChanged = updatedMDA.MDASettings.CompanyMobile != mda.MDASettings.CompanyMobile;


            if (updatedMDA.BankDetails != null && updatedMDA.BankDetails.BankId != 0)
            {
                bool acctNumChanged = mda.BankDetails.BankAccountNumber != updatedMDA.BankDetails.BankAccountNumber;
                bool bankCodeChanged = mda.BankDetails.BankCode != updatedMDA.BankDetails.BankCode;
                hasBankDetailsChanged = acctNumChanged || bankCodeChanged;

                bankDetails.BankAccountNumber = updatedMDA.BankDetails.BankAccountNumber;
                bankDetails.BankId = updatedMDA.BankDetails.BankId;
                bankDetails.BankCode = updatedMDA.BankDetails.BankCode;
            }

            mda.Name = updatedMDA.Name; mda.Code = updatedMDA.Code; mda.Slug = updatedMDA.Slug;
            mdaSettings.CompanyAddress = updatedMDA.MDASettings.CompanyAddress;
            mdaSettings.CompanyMobile = updatedMDA.MDASettings.CompanyMobile;
            mdaSettings.CompanyEmail = updatedMDA.MDASettings.CompanyEmail;
            mdaSettings.BusinessNature = string.IsNullOrEmpty(updatedMDA.MDASettings.BusinessNature) ? mdaSettings.BusinessNature : updatedMDA.MDASettings.BusinessNature;
            mda.LastUpdatedBy = user; mda.UsesTSA = updatedMDA.UsesTSA;

            //if any of these values have changed call cashflow, if not no need to update the cashflow record
            if (hasNameChanged || hasEmailChanged || hasAddressChanged || hasMobileChanged || hasBankDetailsChanged || hasSignatureChanged || hasLogoChanged)
            {
                Logger.Information("Updating cashflow");
                UpdateRecordOnInvoicingService(mda, newLogoPath, newSignaturePath, hasBankDetailsChanged);
            }
            UpdateRecord(mda);
        }

        private CoreMDAService UpdateRecordOnInvoicingService(MDA mda, string logoPath, string signaturePath, bool hasBankDetailsChanged = false, bool makeNewBankAccountPrimaryAccountIfItHasIndeedChanged = true)
        {
            Logger.Information("Calling cashflow");
            var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", mda.SMEKey } });
            var companyService = _invoicingService.CompanyServices(context);
            CashFlowEditCompany edit = new CashFlowEditCompany();
            edit.CompanyName = mda.Name;
            edit.CompanyEmail = mda.MDASettings.CompanyEmail;
            edit.BusinessNature = mda.MDASettings.BusinessNature;
            edit.CompanyAddress = mda.MDASettings.CompanyAddress;
            edit.CompanyMobile = mda.MDASettings.CompanyMobile;


            if (hasBankDetailsChanged)
            {
                edit.AdditionalBankAccount = new CashFlowBankAccount()
                {
                    Bank = new CashFlowBank
                    {
                        Id = mda.BankDetails.BankId,
                        Code = mda.BankDetails.BankCode
                    },
                    Number = mda.BankDetails.BankAccountNumber,
                    Name = mda.NameAndCode(),
                };

                if (makeNewBankAccountPrimaryAccountIfItHasIndeedChanged) { edit.MakeNewBankAccountPrimaryBankAccount = true; }
            }
            else { edit.AdditionalBankAccount = new CashFlowBankAccount { Bank = new CashFlowBank { Id = 0 } }; }
            //little hack make it cleaner on cashflow end
            bool result;
            var baseFilePath = AppDomain.CurrentDomain.BaseDirectory;
            string[] separators = new string[] { "Media" };

            if (!string.IsNullOrEmpty(logoPath)) { logoPath = HttpUtility.UrlDecode(baseFilePath + "Media" + logoPath.Split(separators, 2, StringSplitOptions.None)[1]); }
            if (!string.IsNullOrEmpty(signaturePath)) { signaturePath = HttpUtility.UrlDecode(baseFilePath + "Media" + signaturePath.Split(separators, 2, StringSplitOptions.None)[1]); }

            //if (string.IsNullOrEmpty(logoPath) && string.IsNullOrEmpty(signaturePath)) { result = companyService.EditCompany(edit); }
            //else {  }
            result = companyService.EditCompany(edit, logoPath, signaturePath);
            if (!result) { throw new CannotConnectToCashFlowException("Error connecting to invoicing service"); };
            return this;
        }

        /// <summary>
        /// Update MDA record
        /// </summary>
        /// <param name="mda"></param>
        /// <exception cref="MDARecordCouldNotBeUpdatedException"></exception>
        private void UpdateRecord(MDA mda)
        {
            if (!_mdaRepository.Update(mda)) { throw new MDARecordCouldNotBeUpdatedException("Could not update MDA record"); }
        }

        /// <summary>
        /// Validate files
        /// </summary>
        /// <typeparam name="H"></typeparam>
        /// <param name="expertSystem"></param>
        /// <param name="files"></param>
        /// <param name="mda"></param>
        /// <param name="folderBase"></param>
        /// <returns>List{ErrorModel}</returns>
        private List<ErrorModel> ValidateFiles(ExpertSystemSettings expertSystem, HttpFileCollectionBase files, MDA mda, string folderBase)
        {
            Logger.Information("Validating files");
            List<ErrorModel> errors = new List<ErrorModel>();
            if (expertSystem == null) { throw new TenantNotFoundException("No tenant found when validating files"); }
            if (mda == null) { throw new MDARecordNotFoundException("Null mda reference when validating files"); }

            folderBase = string.IsNullOrEmpty(folderBase) ? DateTime.Now.Ticks + Guid.NewGuid().ToString().Substring(0, 8) : folderBase;
            List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();

            if (files != null)
            {
                //IMPRV
                //we should stop at two iteration or simply get the file by their file names
                for (var i = 0; i < files.Count; i++)
                {
                    filesAndFileNames.Add(new UploadedFileAndName { Upload = files[i], UploadName = files.GetKey(i) });
                }
            }


            foreach (var item in filesAndFileNames)
            {
                if (item.UploadName == "LogoPath" && item.Upload.ContentLength > 0)
                {
                    CheckFileType(new List<UploadedFileAndName> { { item } }, errors);
                    CheckFileSize(new List<UploadedFileAndName> { { item } }, errors);

                    if (errors.Count > 0) { return errors; }

                    CreateFolders(_orchardServices.WorkContext.CurrentSite.SiteName + "\\MDAMedia", new string[] { folderBase });
                    Orchard.MediaLibrary.Models.MediaPart mediaPartLogo = SaveMedia(_orchardServices.WorkContext.CurrentSite.SiteName + "\\MDAMedia\\" + folderBase + "\\LogoPath", item.Upload, true);

                    mda.MDASettings.LogoPath = _mediaLibraryService.GetMediaPublicUrl(mediaPartLogo.FolderPath, mediaPartLogo.FileName);
                    continue;
                }

                if (item.UploadName == "SignaturePath" && item.Upload.ContentLength > 0)
                {
                    CheckFileType(new List<UploadedFileAndName> { { item } }, errors);
                    CheckFileSize(new List<UploadedFileAndName> { { item } }, errors);

                    if (errors.Count > 0) { return errors; }

                    CreateFolders(_orchardServices.WorkContext.CurrentSite.SiteName + "\\MDAMedia", new string[] { folderBase });
                    Orchard.MediaLibrary.Models.MediaPart mediaPartSignature = SaveMedia(_orchardServices.WorkContext.CurrentSite.SiteName + "\\MDAMedia\\" + folderBase + "\\SignaturePath", item.Upload, true);
                    mda.MDASettings.SignaturePath = _mediaLibraryService.GetMediaPublicUrl(mediaPartSignature.FolderPath, mediaPartSignature.FileName);
                    continue;
                }
            }

            //if the logo was not added
            //check if the there is a default logo present            
            mda.MDASettings.LogoPath = string.IsNullOrEmpty(mda.MDASettings.LogoPath) ? expertSystem.LogoPath : mda.MDASettings.LogoPath;
            mda.MDASettings.SignaturePath = string.IsNullOrEmpty(mda.MDASettings.SignaturePath) ? expertSystem.SignaturePath : mda.MDASettings.SignaturePath;
            return errors;
        }


        /// <summary>
        /// Prepare the collection data for validation
        /// </summary>
        /// <param name="collection">Collection of MDAs</param>
        /// <returns>List{UniqueValidationModel}</returns>
        public List<UniqueValidationModel> GetValidationModel(ICollection<MDA> collection, string[] inclusiveClauses = null)
        {
            ICollection<UniqueValidationModel> dataValues = new List<UniqueValidationModel>();
            foreach (var model in collection)
            {
                dataValues.Add(new UniqueValidationModel()
                {
                    Identifier = "MDA.Name",
                    Name = "Name",
                    SelectDataValue = "Name:" + model.Name,
                    InclusiveClauses = inclusiveClauses == null ? new string[] { } : inclusiveClauses,
                    ErrorMessage = "Another MDA record already has this value"
                });

                dataValues.Add(new UniqueValidationModel()
                {
                    Identifier = "MDA.Code",
                    Name = "Code",
                    SelectDataValue = "Code:" + model.Code,
                    InclusiveClauses = inclusiveClauses == null ? new string[] { } : inclusiveClauses,
                    ErrorMessage = "Another MDA record already has this value"
                });
            }
            return dataValues.ToList();
        }


        /// <summary>
        /// Trim MDASettings data
        /// </summary>
        /// <param name="mdaSettings"></param>
        public void TrimData(MDASettings mdaSettings)
        {
            mdaSettings.CompanyMobile = mdaSettings.CompanyMobile.Trim();
            mdaSettings.CompanyEmail = mdaSettings.CompanyEmail.Trim();
        }

        /// <summary>
        /// Get Cash flow company key for this MDA
        /// </summary>
        /// <typeparam name="M">MDA type <see cref="MDA"/></typeparam>
        /// <param name="model">MDA <see cref="MDA"/></param>
        /// <param name="expertSystemSettings">CBSTenantSettings <see cref="ExpertSystemSettings"/></param>
        /// <returns>CoreMDAService</returns>
        public void GetCashFlowKeys(MDA mda, ExpertSystemSettings expertSystemSettings, TenantCBSSettings tenantCBSSettings)
        {
            try
            {
                Logger.Error("Getting cashflow keys");
                #region CASHFLOW 
                var context = _invoicingService.StartInvoicingService(new Dictionary<string, dynamic> { { "companyKeyCode", "" } });
                var companyService = _invoicingService.CompanyServices(context);
                #endregion

                var companyModel = new CashFlowCreateCompany
                {
                    CallBackURL = System.Configuration.ConfigurationManager.AppSettings["CallBackURL"],
                    CompanyName = mda.Name,
                    CompanyAddress = mda.MDASettings.CompanyAddress,
                    CompanyEmail = mda.MDASettings.CompanyEmail,
                    CompanyMobile = mda.MDASettings.CompanyMobile,
                    BusinessNature = expertSystemSettings.BusinessNature,
                    StateID = tenantCBSSettings.StateId,
                    CountryID = tenantCBSSettings.CountryId,
                    Admin = new CashFlowCompanyUser
                    {
                        Permissions = new List<CashFlowUserPermission>() { new CashFlowUserPermission { Permission = new CashFlowPermission { Name = "ADD_PMT" } }, new CashFlowUserPermission { Permission = new CashFlowPermission { Name = "MDF_COY" } }, new CashFlowUserPermission { Permission = new CashFlowPermission { Name = "MNG_INV" } }, new CashFlowUserPermission { Permission = new CashFlowPermission { Name = "MNG_USR" } }, new CashFlowUserPermission { Permission = new CashFlowPermission { Name = "WRT_OFF" } } },
                        Email = mda.MDASettings.CompanyEmail,
                        //Password = Path.GetRandomFileName().Replace("f", "?").Substring(0, 8),
                        Password = "password_" + mda.Code,
                        FirstName = mda.Name,
                        LastName = mda.Code
                    },
                    BankAccount = new CashFlowBankAccount
                    {
                        Bank = new CashFlowBank
                        {
                            Id = mda.BankDetails.BankId,
                            Code = mda.BankDetails.BankCode
                        },
                        Number = mda.BankDetails.BankAccountNumber,
                        Name = mda.NameAndCode(),
                        BankID = mda.BankDetails.BankId,
                    },
                };
                var remotePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
                var logoPath = string.IsNullOrEmpty(mda.MDASettings.LogoPath) ? "" : mda.MDASettings.LogoPath;
                var logoFullServerPath = remotePath + logoPath;
                var logoFileName = string.IsNullOrEmpty(mda.MDASettings.LogoPath) ? "" : Path.GetFileName(mda.MDASettings.LogoPath);
                logoPath = logoFullServerPath.Substring(0, logoFullServerPath.Length - logoFileName.Length);

                var signaturePath = string.IsNullOrEmpty(mda.MDASettings.SignaturePath) ? "" : mda.MDASettings.SignaturePath;
                var signatureFullServerPath = remotePath + signaturePath;
                var signatureFileName = string.IsNullOrEmpty(mda.MDASettings.SignaturePath) ? "" : Path.GetFileName(mda.MDASettings.SignaturePath);
                signaturePath = signatureFullServerPath.Substring(0, signatureFullServerPath.Length - signatureFileName.Length);
                //var n = HttpUtility.UrlDecode();
                logoPath = string.IsNullOrEmpty(mda.MDASettings.LogoPath) ? null : HttpUtility.UrlDecode(_mediaLibraryService.GetMediaPublicUrl(logoPath, logoFileName));
                signaturePath = string.IsNullOrEmpty(mda.MDASettings.SignaturePath) ? null : HttpUtility.UrlDecode(_mediaLibraryService.GetMediaPublicUrl(signaturePath, signatureFileName));
               
                var cashflowSection = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection("cashFlowSettings");
                string thirdPartyKey = cashflowSection["ThirdPartyRequestSecret"];
                string thirdPartyId = cashflowSection["ThirdPartyRequestId"];
                string thirdPartyAppName = cashflowSection["ThirdPartyAppName"];
                ThirdPartyAppIdentity identity = new ThirdPartyAppIdentity { Name = thirdPartyAppName, ClientId = thirdPartyId, Title = thirdPartyAppName, EncryptionKey = thirdPartyKey };

                //request ref
                var requestReference = OnewayHashThis($"{companyModel.Admin.Email}{_orchardServices.WorkContext.CurrentSite.SiteName}{Guid.NewGuid().ToString()}", thirdPartyKey);
                Logger.Error(string.Format("Get cashflow keys with Request ID for {0}", requestReference));
                companyModel.RequestReference = requestReference;

                //add vendor code
                StateConfig.StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.CashflowVendorCode.ToString()).FirstOrDefault();
                Node companyCodeNode = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.CashflowCompanyCode.ToString()).FirstOrDefault();

                companyModel.CashFlowCompanyCode = new CashFlowCompanyCode { Id = Int32.Parse(companyCodeNode.Value) };

                mda.SMEKey = companyService.CreateCompany(companyModel, identity, logoPath: logoPath, signaturePath: signaturePath, vendorCode: node == null? "parkway": node.Value);
                Logger.Error(string.Format("Gotten Cashflow keys"));
            }
            catch (Exception exception)
            {
                //if for some reason we could not connect to cash flow
                //lets delete the stored records
                Logger.Error(exception, "Failed to get keys. Deleting records..");
                //DeleteMDARecords(mda, apiRef);
                Logger.Error("Records deleted ");
                throw new CannotConnectToCashFlowException("Cannot connect to CashFlow", exception);
            }
        }


        /// <summary>
        /// delete mda records
        /// </summary>
        /// <param name="mda"></param>
        private void DeleteMDARecords(MDA mda, APIRequest apiRef)
        {
            var sess = _orchardServices.TransactionManager.GetSession();
            using (var tranx = sess.BeginTransaction())
            {
                try
                {
                    sess.Delete(mda.MDASettings);
                    sess.Delete(mda.BankDetails);
                    if(apiRef != null) { sess.Delete(apiRef); }
                    sess.Delete(mda);
                    tranx.Commit();
                }
                catch (Exception exception)
                {
                    Logger.Error("Error deleting mda ", exception);
                    tranx.Rollback();
                    throw;
                }
            }
        }


        /// <summary>
        /// save mda setting, bank details and mda in one transaction
        /// </summary>
        /// <param name="model"></param>
        private APIRequest SaveMDA(MDA model, string requestIdentifier, ExpertSystemSettings expertSystem)
        {
            Logger.Information(string.Format("Persisting MDA models"));
            MDA mda = model;
            MDASettings settings = model.MDASettings;
            BankDetails details = model.BankDetails;
            APIRequest apiRef = null;

            if (!string.IsNullOrEmpty(requestIdentifier))
            { apiRef = new APIRequest { RequestIdentifier = requestIdentifier, CallType = (short)CallTypeEnum.MDA, ExpertSystemSettings = expertSystem }; }
           

            using (var sess = _orchardServices.TransactionManager.GetSession().SessionFactory.OpenSession())
            using (var tranx = sess.BeginTransaction())
            {
                try
                {
                    var savedSettings = sess.Save(settings);
                    var savedBankDetails = sess.Save(details);
                    mda.MDASettings = settings;
                    mda.BankDetails = details;
                    _orchardServices.TransactionManager.GetSession().Save(mda);
                    if (apiRef != null) { apiRef.ResourceIdentifier = mda.Id; sess.Save(apiRef); }
                    tranx.Commit();
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("Could not save MDA settings. Exception - {0} ", exception.Message));
                    tranx.Rollback();
                }
            }
            Logger.Error("Saved models");
            return apiRef;
        }
    }
}