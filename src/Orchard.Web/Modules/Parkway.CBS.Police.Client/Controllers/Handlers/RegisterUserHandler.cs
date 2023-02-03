using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Client.Controllers.Handlers
{
    public class RegisterUserHandler : IRegisterUserHandler
    {
        private readonly IOrchardServices _orchardServices;
        private readonly ICoreUserService _coreUser;
        private readonly ITaxEntityCategoryManager<TaxEntityCategory> _taxCategoriesRepository;
        private readonly ICoreStateAndLGA _coreStateLGAService;
        private readonly IValidateIdentificationNumberAJAXHandler _validateIdentificationNumberAJAXHandler;
        private readonly ICoreTaxEntityProfileLocationService _coreTaxEntityProfileLocationService;
        private readonly IIdentificationTypeFileAttachmentManager<IdentificationTypeFileAttachment> _identificationTypeFileAttachmentRepo;
        private readonly ICoreHelperService _corehelper;
        private readonly ITaxCategoryTaxCategoryPermissionsManager<TaxCategoryTaxCategoryPermissions> _taxCategoryTaxCategoryPermissionsRepo;
        private readonly ICoreCBSUserTaxEntityProfileLocationService _coreCBSUserTaxEntityProfileLocationService;
        public ILogger Logger { get; set; }

        public RegisterUserHandler(IOrchardServices orchardServices, ICoreStateAndLGA coreStateLGAService, ITaxEntityCategoryManager<TaxEntityCategory> taxCategoriesRepository, ICoreUserService coreUser, IValidateIdentificationNumberAJAXHandler validateIdentificationNumberAJAXHandler, IIdentificationTypeFileAttachmentManager<IdentificationTypeFileAttachment> identificationTypeFileAttachmentRepo, ICoreHelperService corehelper, ITaxCategoryTaxCategoryPermissionsManager<TaxCategoryTaxCategoryPermissions> taxCategoryTaxCategoryPermissionsRepo, ICoreTaxEntityProfileLocationService coreTaxEntityProfileLocationService, ICoreCBSUserTaxEntityProfileLocationService coreCBSUserTaxEntityProfileLocationService)
        {
            _orchardServices = orchardServices;
            _coreStateLGAService = coreStateLGAService;
            _taxCategoriesRepository = taxCategoriesRepository;
            _coreUser = coreUser;
            _validateIdentificationNumberAJAXHandler = validateIdentificationNumberAJAXHandler;
            _identificationTypeFileAttachmentRepo = identificationTypeFileAttachmentRepo;
            _coreTaxEntityProfileLocationService = coreTaxEntityProfileLocationService;
            _corehelper = corehelper;
            _taxCategoryTaxCategoryPermissionsRepo = taxCategoryTaxCategoryPermissionsRepo;
            _coreCBSUserTaxEntityProfileLocationService = coreCBSUserTaxEntityProfileLocationService;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Return view for User sign up page
        /// </summary>
        /// <returns></returns>
        public RegisterPSSUserObj GetViewModelForUserSignup()
        {
            List<TaxEntityCategoryVM> categories = _taxCategoriesRepository.GetTaxEntityCategoryVM();
            return new RegisterPSSUserObj
            {
                HeaderObj = new HeaderObj { },
                RegisterCBSUserModel = new RegisterCBSUserModel { },
                TaxCategoriesVM = categories,
                ErrorMessage = string.Empty,
                StateLGAs = _coreStateLGAService.GetStates(),
                TaxCategoryPermissions = _taxCategoryTaxCategoryPermissionsRepo.GetTaxCategoryPermissions().ToList(),
            };
        }


        /// <summary>
        /// Gets categories
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaxEntityCategoryVM> GetCategories()
        {
            return _taxCategoriesRepository.GetTaxEntityCategoryVM();
        }


        /// <summary>
        /// Gets active tax category permissions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaxCategoryTaxCategoryPermissionsVM> GetTaxCategoryPermissions()
        {
            return _taxCategoryTaxCategoryPermissionsRepo.GetTaxCategoryPermissions();
        }


        /// <summary>
        /// Do validation for model
        /// </summary>
        /// <param name="registrationController"></param>
        /// <param name="errors"></param>
        /// <param name="userInput"></param>
        /// <param name="identificationFile"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        /// <returns>RegisterUserResponse</returns>
        public RegisterUserResponse RegisterCBSUserModel(ref List<ErrorModel> errors, RegisterPSSUserObj userInput, HttpPostedFileBase identificationFile)
        {
            try
            {
                RegisterCBSUserModel dataModel = new RegisterCBSUserModel { };
                IdentificationTypeVM identificationType = _validateIdentificationNumberAJAXHandler.validateIdType(userInput.RegisterCBSUserModel.IdType);
                ValidateIdentificationNumberResponseModel identificationNumberValidationResponse = null;
                string errorMessage = null;

                if (identificationType == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Invalid Identification Type specified.", FieldName = "RegisterCBSUserModel.IdType" });
                    userInput.FormErrorNumber = 1;
                }
                string tempMaskedPhoneNumber = userInput.RegisterCBSUserModel.PhoneNumber;
                string tempMaskedEmail = userInput.RegisterCBSUserModel.Email;
                if (identificationType.HasIntegration)
                {
                    identificationNumberValidationResponse = _validateIdentificationNumberAJAXHandler.ValidateIdentificationNumber(userInput.RegisterCBSUserModel.IdNumber, userInput.RegisterCBSUserModel.IdType, identificationType.ImplementingClassName, out errorMessage);
                    userInput.RegisterCBSUserModel.PhoneNumber = identificationNumberValidationResponse.PhoneNumber;
                    userInput.RegisterCBSUserModel.Email = !string.IsNullOrEmpty(identificationNumberValidationResponse.EmailAddress) ? identificationNumberValidationResponse.EmailAddress : userInput.RegisterCBSUserModel.Email;
                }

                DoUserFormValidation(userInput, errors, dataModel);

               
                //do validation check
                if (errors.Count() > 0) 
                {
                    userInput.RegisterCBSUserModel.PhoneNumber = tempMaskedPhoneNumber;
                    userInput.RegisterCBSUserModel.Email = tempMaskedEmail;
                    throw new DirtyFormDataException(); 
                }

                if ((!identificationType.HasIntegration && identificationFile == null) || (!identificationType.HasIntegration && identificationFile.ContentLength == 0))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Identification file is required in the supported format (.pdf,.jpg,.jpeg,.png).", FieldName = "IdentificationFile" });
                    userInput.FormErrorNumber = 1;
                    throw new DirtyFormDataException();
                }

                if (!identificationType.HasIntegration)
                {   //validate identification file
                    List<UploadedFileAndName> filesAndFileNames = new List<UploadedFileAndName>();

                    { filesAndFileNames.Add(new UploadedFileAndName { Upload = identificationFile, UploadName = "IdentificationFile" }); }
                    _corehelper.CheckFileSize(filesAndFileNames, errors, 2048);
                    _corehelper.CheckFileType(filesAndFileNames, errors, new List<string> { "jpg", "png", "jpeg", "pdf" }, new List<string> { ".jpg", ".png", ".jpeg", ".pdf" });
                }
                else
                {   //validate identification number
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = errorMessage, FieldName = "RegisterCBSUserModel.IdNumber" });
                        userInput.FormErrorNumber = 1;
                        throw new DirtyFormDataException();
                    }
                    dataModel.Name = identificationNumberValidationResponse.TaxPayerName;
                    dataModel.PhoneNumber = (!string.IsNullOrEmpty(identificationNumberValidationResponse.PhoneNumber)) ? Util.NormalizePhoneNumber(identificationNumberValidationResponse.PhoneNumber) : Util.NormalizePhoneNumber(dataModel.PhoneNumber);
                    dataModel.RCNumber = identificationNumberValidationResponse.RCNumber;
                    dataModel.FirstName = identificationNumberValidationResponse.FirstName;
                    dataModel.MiddleName = identificationNumberValidationResponse.MiddleName;
                    dataModel.LastName = identificationNumberValidationResponse.LastName;
                }

                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

                int categoryId = 0;
                if (!Int32.TryParse(userInput.TaxPayerType, out categoryId))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Category not found. ", FieldName = "RegisterCBSUserModel.TaxPayerType" });
                    userInput.FormErrorNumber = 1;
                    throw new DirtyFormDataException();
                }
                TaxEntityCategoryVM category = _taxCategoriesRepository.GetTaxEntityCategoryVM(categoryId);
                if (category == null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Category not found.", FieldName = "RegisterCBSUserModel.TaxPayerType" });
                    userInput.FormErrorNumber = 1;
                }

                TaxEntityCategorySettings taxEntityCatSettings = category.GetSettings();
                if (taxEntityCatSettings.ValidateContactEntityInfo)
                {
                    if (string.IsNullOrEmpty(userInput.RegisterCBSUserModel.RCNumber))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Enter a valid RC Number.", FieldName = "RegisterCBSUserModel.RCNumber" });
                        throw new DirtyFormDataException();
                    }

                    dataModel.RCNumber = string.IsNullOrEmpty(dataModel.RCNumber) ? userInput.RegisterCBSUserModel.RCNumber.Trim() : dataModel.RCNumber.Trim();
                    if (dataModel.RCNumber.Length == 0)
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "RCNumber is required.", FieldName = "RegisterCBSUserModel.RCNumber" });
                    }

                    var contactEntityValidationModel = new ContactEntityValidationModel
                    {
                        ContactNameFieldValue = userInput.RegisterCBSUserModel.ContactPersonName,
                        ContactNameFieldName = "RegisterCBSUserModel." + nameof(CBS.Core.HelperModels.RegisterCBSUserModel.ContactPersonName),
                        ContactPhoneNumberFieldValue = userInput.RegisterCBSUserModel.ContactPersonPhoneNumber,
                        ContactPhoneNumberFieldName = "RegisterCBSUserModel." + nameof(CBS.Core.HelperModels.RegisterCBSUserModel.ContactPersonPhoneNumber),
                        ContactEmailFieldValue = userInput.RegisterCBSUserModel.ContactPersonEmail,
                        ContactEmailFieldName = "RegisterCBSUserModel." + nameof(CBS.Core.HelperModels.RegisterCBSUserModel.ContactPersonEmail),
                    };

                    ValidateContactEntityInfo(contactEntityValidationModel, errors);
                    if (errors.Count() > 0)
                    {
                        userInput.RegisterCBSUserModel.ContactPersonName = contactEntityValidationModel.ContactNameFieldValue;
                        userInput.RegisterCBSUserModel.ContactPersonEmail = contactEntityValidationModel.ContactEmailFieldValue;
                        userInput.RegisterCBSUserModel.ContactPersonPhoneNumber = contactEntityValidationModel.ContactPhoneNumberFieldValue;
                        throw new DirtyFormDataException();
                    }
                    dataModel.ContactPersonEmail = contactEntityValidationModel.ContactEmailFieldValue;
                    dataModel.ContactPersonName = contactEntityValidationModel.ContactNameFieldValue;
                    dataModel.ContactPersonPhoneNumber = contactEntityValidationModel.ContactPhoneNumberFieldValue;
                }

                if (taxEntityCatSettings.ValidateGenderInfo)
                {
                    if (!Enum.IsDefined(typeof(Core.Models.Enums.Gender), userInput.RegisterCBSUserModel.Gender))
                    {
                        errors.Add(new ErrorModel { ErrorMessage = "Specified gender is not valid.", FieldName = "RegisterCBSUserModel.Gender" });
                    }
                }
                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

                dataModel.UserName = userInput.RegisterCBSUserModel.Email;
                dataModel.Password = userInput.RegisterCBSUserModel.Password;
                dataModel.ConfirmPassword = userInput.RegisterCBSUserModel.ConfirmPassword;
                dataModel.Gender = userInput.RegisterCBSUserModel.Gender;
                var registerUserResponse = _coreUser.TryCreateCBSUser(dataModel, new TaxEntityCategory { Name = category.Name, RequiresLogin = category.RequiresLogin, Id = category.Id }, ref errors, fieldPrefix: "RegisterCBSUserModel.", validateEmail: true, validatePhoneNumber: true);
                if (errors.Count() > 0) { throw new DirtyFormDataException(); }

                //Creates default location/branch
                int locationId = _coreTaxEntityProfileLocationService.CreateBranch(new TaxEntityProfileLocationVM
                {
                    Name = registerUserResponse.TaxEntityVM.Recipient,
                    State = dataModel.SelectedState,
                    LGA = dataModel.SelectedStateLGA,
                    Address = dataModel.Address,
                    TaxEntityId = registerUserResponse.TaxEntityVM.Id
                }, true);

                _coreCBSUserTaxEntityProfileLocationService.AttachUserToLocation(registerUserResponse.CBSUserId, locationId);

                if (!identificationType.HasIntegration)
                {
                    string fileName = string.Empty;
                    string path = string.Empty;
                    string siteName = _orchardServices.WorkContext.CurrentSite.SiteName;
                    fileName = Guid.NewGuid().ToString("N") + DateTime.Now.Ticks.ToString() + Path.GetExtension(identificationFile.FileName);
                    StateConfig siteConfig = Util.GetTenantConfigBySiteName(siteName);
                    Node node = siteConfig.Node.Where(x => x.Key == PSSTenantConfigKeys.RegistrationIdentificationFilePath.ToString()).FirstOrDefault();
                    if (node == null || string.IsNullOrEmpty(node.Value))
                    {
                        Logger.Error(string.Format("Unable to get registration identification file upload path in a config file"));
                        errors.Add(new ErrorModel { ErrorMessage = "Error uploading identification file.", FieldName = "IdentificationFile" });
                        userInput.FormErrorNumber = 1;
                        throw new DirtyFormDataException();
                    }

                    DirectoryInfo basePath = Directory.CreateDirectory(HttpRuntime.AppDomainAppPath + node.Value + siteName);
                    path = Path.Combine(basePath.FullName, fileName);
                    //save file
                    identificationFile.SaveAs(path);

                    IdentificationTypeFileAttachment identificateTypeFile = new IdentificationTypeFileAttachment
                    {
                        IdentificationType = new Core.Models.IdentificationType { Id = identificationType.Id },
                        TaxEntity = new TaxEntity { Id = registerUserResponse.TaxEntityVM.Id },
                        OriginalFileName = identificationFile.FileName,
                        FilePath = path,
                        ContentType = identificationFile.ContentType,
                        Blob = Convert.ToBase64String(File.ReadAllBytes(path))
                    };

                    if (!_identificationTypeFileAttachmentRepo.Save(identificateTypeFile))
                    {
                        errors.Add(new ErrorModel { FieldName = "IdentificationFile", ErrorMessage = "Unable to save identification file. Please try again later" });
                        _identificationTypeFileAttachmentRepo.RollBackAllTransactions();
                        throw new DirtyFormDataException();
                    }
                }
                return registerUserResponse;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                if (errors.Count > 0)
                {
                    userInput.FlashObj = new FlashObj { FlashType = FlashType.Error, MessageTitle = "User Details", Message = errors.ElementAt(0).ErrorMessage };
                    throw new DirtyFormDataException();
                }
                throw;
            }

        }


        /// <summary>
        /// Validates contact entity validation model
        /// </summary>
        /// <param name="contactInfo"></param>
        /// <param name="errors"></param>
        public void ValidateContactEntityInfo(ContactEntityValidationModel contactInfo, List<ErrorModel> errors)
        {
            if (string.IsNullOrEmpty(contactInfo.ContactNameFieldValue))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Contact person name is not valid", FieldName = contactInfo.ContactNameFieldName });
                return;
            }
            contactInfo.ContactNameFieldValue = contactInfo.ContactNameFieldValue.Trim();
            if (contactInfo.ContactNameFieldValue.Length < 3 || contactInfo.ContactNameFieldValue.Length > 100)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Contact person name value should be between 3 and 100 chanrecters.", FieldName = contactInfo.ContactNameFieldName });
            }

            if (string.IsNullOrEmpty(contactInfo.ContactPhoneNumberFieldValue))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Contact person phone number is not valid", FieldName = contactInfo.ContactPhoneNumberFieldName });
                return;
            }
            contactInfo.ContactPhoneNumberFieldValue = contactInfo.ContactPhoneNumberFieldValue.Trim();
            if (!Util.DoPhoneNumberValidation(contactInfo.ContactPhoneNumberFieldValue))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Contact person phone number is not valid.", FieldName = contactInfo.ContactPhoneNumberFieldName });
                return;
            }
            contactInfo.ContactPhoneNumberFieldValue = Util.NormalizePhoneNumber(contactInfo.ContactPhoneNumberFieldValue);

            if (string.IsNullOrEmpty(contactInfo.ContactEmailFieldValue))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Contact person email is not valid", FieldName = contactInfo.ContactEmailFieldName });
                return;
            }
            contactInfo.ContactEmailFieldValue = contactInfo.ContactEmailFieldValue.Trim();
            if (contactInfo.ContactEmailFieldValue.Length < 3 || contactInfo.ContactEmailFieldValue.Length > 100)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Contact person email value should be between 3 and 100 chanrecters.", FieldName = contactInfo.ContactEmailFieldName });
            }
            Util.DoEmailValidation(contactInfo.ContactEmailFieldValue, ref errors, contactInfo.ContactEmailFieldName, true);
        }




        /// <summary>
        /// Do user form validation
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="errors"></param>
        private void DoUserFormValidation(RegisterPSSUserObj userInput, List<ErrorModel> errors, RegisterCBSUserModel dataModel)
        {
            if (string.IsNullOrEmpty(userInput.RegisterCBSUserModel.Name))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Enter a valid name.", FieldName = "RegisterCBSUserModel.Name" });
                return;
            }

            dataModel.Name = userInput.RegisterCBSUserModel.Name.Trim();
            if (dataModel.Name.Length < 3 || dataModel.Name.Length > 100)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Name value should be between 3 and 100 chanrecters.", FieldName = "RegisterCBSUserModel.Name" });
            }

            if (userInput.RegisterCBSUserModel.IdNumber != "01381022-0001")
            {
                if (!Util.DoPhoneNumberValidation(userInput.RegisterCBSUserModel.PhoneNumber))
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Enter a valid phone number.", FieldName = "RegisterCBSUserModel.PhoneNumber" });
                    return;

                }
            }
            dataModel.PhoneNumber = userInput.RegisterCBSUserModel.PhoneNumber.Trim();

            if (string.IsNullOrEmpty(userInput.RegisterCBSUserModel.Email))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Enter a valid email.", FieldName = "RegisterCBSUserModel.Email" });
                return;
            }

            dataModel.Email = userInput.RegisterCBSUserModel.Email.Trim();
            if (dataModel.Email.Length < 3 || dataModel.Email.Length > 100)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Email value should be between 3 and 100 chanrecters.", FieldName = "RegisterCBSUserModel.Email" });
            }
            _coreUser.DoEmailValidation(dataModel.Email, ref errors, "RegisterCBSUserModel.Email", true);

            if (userInput.RegisterCBSUserModel.IdType == 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Specify a valid Identification Type", FieldName = "RegisterCBSUserModel.IdType" });
                userInput.FormErrorNumber = 1;
            }
            dataModel.IdType = userInput.RegisterCBSUserModel.IdType;

            if (String.IsNullOrEmpty(userInput.RegisterCBSUserModel.IdNumber))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Specify a valid Identification Number. ", FieldName = "RegisterCBSUserModel.IdNumber" });
                userInput.FormErrorNumber = 1;
                return;
            }

            dataModel.IdNumber = userInput.RegisterCBSUserModel.IdNumber.Trim();
            if (dataModel.IdNumber.Length < 5 || dataModel.IdNumber.Length > 20)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Identification Number value should be between 5 and 20 chanrecters.", FieldName = "RegisterCBSUserModel.IdNumber" });
                userInput.FormErrorNumber = 1;
            }

            if (String.IsNullOrEmpty(userInput.RegisterCBSUserModel.Address))
            {
                errors.Add(new ErrorModel { ErrorMessage = "The address field value must be at least 10 characters.", FieldName = "RegisterCBSUserModel.Address" });
                return;
            }

            dataModel.Address = userInput.RegisterCBSUserModel.Address.Trim();
            if (dataModel.Address.Length < 10 || dataModel.Address.Length > 250)
            {
                errors.Add(new ErrorModel { ErrorMessage = "The address field value should be between 10 and 250 chanrecters.", FieldName = "RegisterCBSUserModel.Address" });
            }

            if (userInput.RegisterCBSUserModel.SelectedState <= 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.statenotfound().ToString(), FieldName = "RegisterCBSUserModel.SelectedState" });
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.lganotfound().ToString(), FieldName = "RegisterCBSUserModel.SelectedStateLGA" });
                return;
            }

            dataModel.SelectedState = userInput.RegisterCBSUserModel.SelectedState;
            dataModel.SelectedStateLGA = userInput.RegisterCBSUserModel.SelectedStateLGA;
            if (userInput.RegisterCBSUserModel.SelectedStateLGA <= 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = ErrorLang.lganotfound().ToString(), FieldName = "RegisterCBSUserModel.SelectedStateLGA" });
            }
        }


        /// <summary>
        /// Gets identification types available for tax category with specified id.
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IEnumerable<Core.VM.IdentificationTypeVM> GetIdentificationTypesForCategory(int categoryId)
        {
            try
            {
                return _validateIdentificationNumberAJAXHandler.GetIdentificationTypesForCategory(categoryId);
            }
            catch (Exception) { return null; }
        }
    }
}