using Orchard;
using Orchard.FileSystems.Media;
using Orchard.Logging;
using Orchard.MediaLibrary.Services;
using Parkway.CBS.Core.HTTP.Handlers.Contracts;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Parkway.CBS.Core.HelperModels;
using Orchard.Security;
using Orchard.Users.Services;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using System.ComponentModel.DataAnnotations;
using Parkway.CBS.Core.Utilities;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Parkway.CBS.Core.HTTP.Handlers
{
    public class CoreUserService : ICoreUserService
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IMediaLibraryService _mediaLibraryService;
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly ICoreTaxPayerService _taxPayerService;
        private readonly ICBSUserManager<CBSUser> _cbsUserService;
        private readonly IAPIRequestManager<APIRequest> _apiRequestRepository;
        private readonly Lazy<IStateModelManager<StateModel>> _stateRepo;

        public ILogger Logger { get; set; }


        public CoreUserService(IOrchardServices orchardServices,
            IMediaLibraryService mediaManagerService, IMimeTypeProvider mimeTypeProvider,
            IInvoicingService invoicingService, IMediaLibraryService mediaLibraryService, IMembershipService membershipService, IUserService userService, ICoreTaxPayerService taxPayerService, ICBSUserManager<CBSUser> cbsUserService, IAPIRequestManager<APIRequest> apiRequestRepository, Lazy<IStateModelManager<StateModel>> stateRepo)
        {
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
            _mediaLibraryService = mediaLibraryService;
            _membershipService = membershipService;
            _userService = userService;
            _taxPayerService = taxPayerService;
            _cbsUserService = cbsUserService;
            _apiRequestRepository = apiRequestRepository;
            _stateRepo = stateRepo;
        }


        /// <summary>
        /// Create a front end user on central billing
        /// <para>
        /// This would create a CBSUser, tax profile and activate the tax profile account. 
        /// Safe from null checks. Would return a valid object, or throw an exception
        /// </para>
        /// </summary>
        /// <param name="registerCBSUserModel"></param>
        /// <param name="category"></param>
        /// <param name="errors"></param>
        /// <param name="expertSystem"></param>
        /// <param name="requestRef"></param>
        /// <returns>RegisterUserResponse</returns>
        /// <exception cref="CBSUserNotFoundException">for api call when </exception>
        /// <exception cref="CannotSaveTaxEntityException">for when the tax profile could not be saved</exception>
        public RegisterUserResponse TryCreateCBSUser(RegisterCBSUserModel validatedRegisterCBSUserModel, TaxEntityCategory category, ref List<ErrorModel> errors, ExpertSystemSettings expertSystem = null, string requestRef = null, string fieldPrefix = null, bool validateEmail = false, bool validatePhoneNumber = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(requestRef))
                {
                    Logger.Information("Searching for TryCreateCBSUser reference");
                    Int64 refResult = _apiRequestRepository.GetResourseIdentifier(requestRef, expertSystem, CallTypeEnum.Register);
                    if (refResult > 0)
                    {
                        Logger.Information(string.Format("Request reference found for client ID: {0} Ref: {1} CBSUser: {2}", expertSystem.ClientId, requestRef, refResult));
                        var persistedCBSUser = _cbsUserService.Get(x => x.Id == refResult);
                        if (persistedCBSUser == null)
                        {
                            Logger.Error("Cannot find CBS user request with Id " + refResult);
                            throw new CBSUserNotFoundException("Cannot find CBS user request with Id " + refResult);
                        }
                        Logger.Information(string.Format("Returning ref TryCreateCBSUser {0}", requestRef));
                        return new RegisterUserResponse { CBSUserId = persistedCBSUser.Id };
                    }
                }

                DoModelCheck(validatedRegisterCBSUserModel, ref errors);
                //do validation check if email
                DoEmailValidation(validatedRegisterCBSUserModel.Email, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "Email" : "Email", validateEmail);
                //validate phonenumber
                if (validatedRegisterCBSUserModel.IdNumber != "01381022-0001")
                {
                    DoPhoneNumberValidation(validatedRegisterCBSUserModel.PhoneNumber, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "PhoneNumber" : "PhoneNumber", validatePhoneNumber);
                }
                //check if category requires the TIN to be unique
                PerformTenantSpecificUserChecks(validatedRegisterCBSUserModel); 
                //validate state and LGA
                int count = _stateRepo.Value.CountStateIdForLGAId(validatedRegisterCBSUserModel.SelectedStateLGA, validatedRegisterCBSUserModel.SelectedState);
                if (count != 1)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected LGA value is not valid", FieldName = !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "SelectedStateLGA" : "SelectedStateLGA" });
                    errors.Add(new ErrorModel { ErrorMessage = "Selected State value is not valid", FieldName = !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "SelectedState" : "SelectedState" });
                }

                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                //check for email duplicates
                if (validateEmail)
                {
                    HasDuplicateEmail(validatedRegisterCBSUserModel.Email, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "Email" : "Email");
                }
                //check for phoneNumber duplicate
                if (validatePhoneNumber)
                {
                    HasDuplicatePhoneNumber(validatedRegisterCBSUserModel.PhoneNumber, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "PhoneNumber" : "PhoneNumber");
                }

                CBSUser savedCBSUser = null;
                //firstly lets get the tax entity, we validate and save
                TaxEntity taxEntity = new TaxEntity
                {
                    Email = string.IsNullOrEmpty(validatedRegisterCBSUserModel.Email) ? null : validatedRegisterCBSUserModel.Email.Trim(),
                    PhoneNumber = string.IsNullOrEmpty(validatedRegisterCBSUserModel.PhoneNumber) ? null : Util.NormalizePhoneNumber(validatedRegisterCBSUserModel.PhoneNumber).Trim(),
                    Address = validatedRegisterCBSUserModel.Address,
                    Recipient = validatedRegisterCBSUserModel.Name.Trim(),
                    TaxEntityType = category.Id,
                    TaxEntityCategory = category,
                    TaxPayerIdentificationNumber = string.IsNullOrEmpty(validatedRegisterCBSUserModel.TIN) ? validatedRegisterCBSUserModel.TIN : validatedRegisterCBSUserModel.TIN.Trim(),
                    RCNumber = string.IsNullOrEmpty(validatedRegisterCBSUserModel.RCNumber) ? null : validatedRegisterCBSUserModel.RCNumber.Trim(),
                    ContactPersonName = string.IsNullOrEmpty(validatedRegisterCBSUserModel.ContactPersonName) ? null : validatedRegisterCBSUserModel.ContactPersonName.Trim(),
                    ContactPersonEmail = string.IsNullOrEmpty(validatedRegisterCBSUserModel.ContactPersonEmail) ? null : validatedRegisterCBSUserModel.ContactPersonEmail.Trim(),
                    ContactPersonPhoneNumber = string.IsNullOrEmpty(validatedRegisterCBSUserModel.ContactPersonPhoneNumber) ? null : Util.NormalizePhoneNumber(validatedRegisterCBSUserModel.ContactPersonPhoneNumber).Trim(),
                    ShortName = validatedRegisterCBSUserModel.ShortName,
                    StateLGA = new LGA { Id = validatedRegisterCBSUserModel.SelectedStateLGA },
                    BVN = string.IsNullOrEmpty(validatedRegisterCBSUserModel.BVN) ? null : validatedRegisterCBSUserModel.BVN.Trim(),
                    IdentificationType = (validatedRegisterCBSUserModel.IdType == 0) ? 0 : validatedRegisterCBSUserModel.IdType,
                    IdentificationNumber = string.IsNullOrEmpty(validatedRegisterCBSUserModel.IdNumber) ? null : validatedRegisterCBSUserModel.IdNumber,
                    AddedByExternalExpertSystem = expertSystem == null ? null : new ExpertSystemSettings { Id = expertSystem.Id },
                    BusinessType = (validatedRegisterCBSUserModel.BusinessTypeId > 0) ? validatedRegisterCBSUserModel.BusinessTypeId : 0,
                    Gender = (Gender) validatedRegisterCBSUserModel.Gender,
                    FirstName = string.IsNullOrEmpty(validatedRegisterCBSUserModel.FirstName) ? null : validatedRegisterCBSUserModel.FirstName.Trim(),
                    MiddleName = string.IsNullOrEmpty(validatedRegisterCBSUserModel.MiddleName) ? null : validatedRegisterCBSUserModel.MiddleName.Trim(),
                    LastName = string.IsNullOrEmpty(validatedRegisterCBSUserModel.LastName) ? null : validatedRegisterCBSUserModel.LastName.Trim(),
                };

                taxEntity = _taxPayerService.ValidateAndSaveTaxEntity(taxEntity, category).TaxEntity;

                //let check if the user has a login profile
                savedCBSUser = _cbsUserService.Get(x => x.TaxEntity == taxEntity);
                //if cbs user is not null, that means that this tax entity alreay has a login profile
                if (savedCBSUser != null)
                { _cbsUserService.RollBackAllTransactions(); throw new CBSUserAlreadyExistsException(); }

                //Business requirement is that the user can register without an email            
                //check if the username already exists
                var userNameExists = _membershipService.GetUser(validatedRegisterCBSUserModel.UserName.Trim());
                if (userNameExists != null)
                {
                    _cbsUserService.RollBackAllTransactions();
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.usernamealreadyexists().ToString(), FieldName = !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "UserName" : "UserName" });
                    throw new DirtyFormDataException();
                }

                var email = validatedRegisterCBSUserModel.Email != null ? validatedRegisterCBSUserModel.Email.Trim() : validatedRegisterCBSUserModel.Email;

                var user = _membershipService.CreateUser(new CreateUserParams(validatedRegisterCBSUserModel.UserName.Trim(), validatedRegisterCBSUserModel.Password, email, null, null, true));

                CBSUser cbsUser = new CBSUser
                {
                    Name = validatedRegisterCBSUserModel.Name.Trim(),
                    TaxEntity = taxEntity,
                    UserPartRecord = new Orchard.Users.Models.UserPartRecord { Id = user.Id },
                    PhoneNumber = taxEntity.PhoneNumber,
                    Email = taxEntity.Email,
                    Address = taxEntity.Address,
                    IsAdministrator = true,
                    IsActive = true
                };

                APIRequest requestLog = null;
                if (!string.IsNullOrEmpty(requestRef))
                {
                    requestLog = new APIRequest
                    { ResourceIdentifier = cbsUser.Id, RequestIdentifier = requestRef, ExpertSystemSettings = expertSystem, CallType = (short)CallTypeEnum.Register };
                }
                try
                {
                    SaveCBSUserRegistration(cbsUser, requestLog);
                    return new RegisterUserResponse { CBSUserId = cbsUser.Id, TaxEntityVM = new TaxEntityViewModel { Id = taxEntity.Id, Email = taxEntity.Email, Recipient = taxEntity.Recipient, PhoneNumber = taxEntity.PhoneNumber, PayerId = taxEntity.PayerId }, CBSUser = new CBSUserVM { Id = cbsUser.Id, Name = cbsUser.Name, Email = cbsUser.Email, PhoneNumber = cbsUser.PhoneNumber, TaxEntity = new TaxEntityViewModel { Id = taxEntity.Id } } };
                }
                catch (Exception)
                { _cbsUserService.RollBackAllTransactions(); throw; }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _cbsUserService.RollBackAllTransactions();
                throw;
            }
        }


        /// <summary>
        /// Creates a front end user on central billing
        /// </summary>
        /// <para>
        /// This would create a CBSUser linked to the tax profile account with the specified tax entity id.
        /// Safe from null checks. Would return a valid object, or throw an exception
        /// </para>
        /// <param name="validatedRegisterCBSUserModel"></param>
        /// <param name="taxEntity"></param>
        /// <param name="category"></param>
        /// <param name="errors"></param>
        /// <param name="expertSystem"></param>
        /// <param name="requestRef"></param>
        /// <param name="fieldPrefix"></param>
        /// <param name="validateEmail"></param>
        /// <param name="validatePhoneNumber"></param>
        /// <returns></returns>
        public RegisterUserResponse TryCreateCBSSubUser(RegisterCBSUserModel validatedRegisterCBSUserModel, TaxEntity taxEntity, TaxEntityCategory category, ref List<ErrorModel> errors, ExpertSystemSettings expertSystem = null, string requestRef = null, string fieldPrefix = null, bool validateEmail = false, bool validatePhoneNumber = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(requestRef))
                {
                    Logger.Information("Searching for TryCreateCBSSubUser reference");
                    Int64 refResult = _apiRequestRepository.GetResourseIdentifier(requestRef, expertSystem, CallTypeEnum.Register);
                    if (refResult > 0)
                    {
                        Logger.Information(string.Format("Request reference found for client ID: {0} Ref: {1} CBSUser: {2}", expertSystem.ClientId, requestRef, refResult));
                        var persistedCBSUser = _cbsUserService.Get(x => x.Id == refResult);
                        if (persistedCBSUser == null)
                        {
                            Logger.Error("Cannot find CBS user request with Id " + refResult);
                            throw new CBSUserNotFoundException("Cannot find CBS user request with Id " + refResult);
                        }
                        Logger.Information(string.Format("Returning ref TryCreateCBSSubUser {0}", requestRef));
                        return new RegisterUserResponse { CBSUserId = persistedCBSUser.Id };
                    }
                }

                DoModelCheck(validatedRegisterCBSUserModel, ref errors);
                //do validation check if email
                DoEmailValidation(validatedRegisterCBSUserModel.Email, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "Email" : "Email", validateEmail);
                //validate phonenumber
                DoPhoneNumberValidation(validatedRegisterCBSUserModel.PhoneNumber, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "PhoneNumber" : "PhoneNumber", validatePhoneNumber);
                //check if category requires the TIN to be unique
                PerformTenantSpecificUserChecks(validatedRegisterCBSUserModel);
                //validate state and LGA
                int count = _stateRepo.Value.CountStateIdForLGAId(validatedRegisterCBSUserModel.SelectedStateLGA, validatedRegisterCBSUserModel.SelectedState);
                if (count != 1)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Selected LGA value is not valid", FieldName = !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "SelectedStateLGA" : "SelectedStateLGA" });
                    errors.Add(new ErrorModel { ErrorMessage = "Selected State value is not valid", FieldName = !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "SelectedState" : "SelectedState" });
                }

                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                //check for email duplicates
                if (validateEmail)
                {
                    HasDuplicateEmailForCBSUser(validatedRegisterCBSUserModel.Email, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "Email" : "Email");
                }
                //check for phoneNumber duplicate
                if (validatePhoneNumber)
                {
                    HasDuplicatePhoneNumberForCBSUser(validatedRegisterCBSUserModel.PhoneNumber, ref errors, !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "PhoneNumber" : "PhoneNumber");
                }

                //Business requirement is that the user can register without an email            
                //check if the username already exists
                var userNameExists = _membershipService.GetUser(validatedRegisterCBSUserModel.UserName.Trim());
                if (userNameExists != null)
                {
                    errors.Add(new ErrorModel { ErrorMessage = ErrorLang.usernamealreadyexists().ToString(), FieldName = !string.IsNullOrEmpty(fieldPrefix) ? fieldPrefix + "UserName" : "UserName" });
                    _cbsUserService.RollBackAllTransactions();
                    throw new DirtyFormDataException();
                }

                var email = validatedRegisterCBSUserModel.Email != null ? validatedRegisterCBSUserModel.Email.Trim() : validatedRegisterCBSUserModel.Email;

                var user = _membershipService.CreateUser(new CreateUserParams(validatedRegisterCBSUserModel.UserName.Trim(), validatedRegisterCBSUserModel.Password, email, null, null, true));

                CBSUser cbsUser = new CBSUser
                {
                    Name = validatedRegisterCBSUserModel.Name.Trim(),
                    TaxEntity = taxEntity,
                    UserPartRecord = new Orchard.Users.Models.UserPartRecord { Id = user.Id },
                    PhoneNumber = Util.NormalizePhoneNumber(validatedRegisterCBSUserModel.PhoneNumber).Trim(),
                    Email = validatedRegisterCBSUserModel.Email.Trim(),
                    Address = validatedRegisterCBSUserModel.Address,
                    IsActive = true
                };

                APIRequest requestLog = null;
                if (!string.IsNullOrEmpty(requestRef))
                {
                    requestLog = new APIRequest
                    { ResourceIdentifier = cbsUser.Id, RequestIdentifier = requestRef, ExpertSystemSettings = expertSystem, CallType = (short)CallTypeEnum.Register };
                }
                try
                {
                    SaveCBSUserRegistration(cbsUser, requestLog);
                    return new RegisterUserResponse { CBSUserId = cbsUser.Id, TaxEntityVM = new TaxEntityViewModel { Id = taxEntity.Id } };
                }
                catch (Exception)
                { _cbsUserService.RollBackAllTransactions(); throw; }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _cbsUserService.RollBackAllTransactions();
                throw;
            }
        }


        private void HasDuplicateEmail(string email, ref List<ErrorModel> errors, string fieldName)
        {
            int count = _taxPayerService.CheckCountCount(t => t.Email == email);
            if (count > 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Another user with this email already exists.", FieldName = fieldName });
                throw new DirtyFormDataException();
            }
        }


        private void HasDuplicateEmailForCBSUser(string email, ref List<ErrorModel> errors, string fieldName)
        {
            int count = _cbsUserService.Count(t => t.Email == email);
            if (count > 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Another user with this email already exists.", FieldName = fieldName });
                throw new DirtyFormDataException();
            }
        }


        private void HasDuplicatePhoneNumber(string phoneNumber, ref List<ErrorModel> errors, string fieldName)
        {
            int count = _taxPayerService.CheckCountCount(t => t.PhoneNumber == phoneNumber);
            if (count > 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Another user with this phone number already exists.", FieldName = fieldName });
                throw new PhoneNumberHasBeenTakenException();
            }
        }


        private void HasDuplicatePhoneNumberForCBSUser(string phoneNumber, ref List<ErrorModel> errors, string fieldName)
        {
            int count = _cbsUserService.Count(t => t.PhoneNumber == phoneNumber);
            if (count > 0)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Another user with this phone number already exists.", FieldName = fieldName });
                throw new PhoneNumberHasBeenTakenException();
            }
        }


        /// <summary>
        /// Do email validation
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        public void DoEmailValidation(string email, ref List<ErrorModel> errors, string fieldName, bool compulsory)
        {
            Util.DoEmailValidation(email, ref errors, fieldName, compulsory);            
        }


        /// <summary>
        /// Do validation for phone number
        /// </summary>
        /// <param name="sPhoneNumber"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        public void DoPhoneNumberValidation(string sPhoneNumber, ref List<ErrorModel> errors, string fieldName, bool compulsory)
        {
            if (string.IsNullOrEmpty(sPhoneNumber))
            {
                if (!compulsory) { return; }
                errors.Add(new ErrorModel { ErrorMessage = "Enter a valid phonenumber.", FieldName = fieldName });
                return;
            }

            if (!Util.DoPhoneNumberValidation(sPhoneNumber))
            {
                errors.Add(new ErrorModel { ErrorMessage = "Add a valid mobile phone number.", FieldName = fieldName });
            }
        }



        /// <summary>
        /// do model check
        /// </summary>
        /// <param name="validatedRegisterCBSUserModel"></param>
        /// <param name="errors"></param>
        public void DoModelCheck(RegisterCBSUserModel validatedRegisterCBSUserModel, ref List<ErrorModel> errors)
        {
            ICollection<ValidationResult> results = new List<ValidationResult>();
            Validator.TryValidateObject(validatedRegisterCBSUserModel, new ValidationContext(validatedRegisterCBSUserModel), results);
            foreach (var item in results)
            {
                errors.Add(new ErrorModel { ErrorMessage = item.ErrorMessage, FieldName = item.MemberNames.ElementAt(0) });
            }
        }


        private void PerformTenantSpecificUserChecks(RegisterCBSUserModel validatedRegisterCBSUserModel)
        {
            var tenantConfig = Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
            var value = tenantConfig.Node.Where(s => s.Key == TenantConfigKeys.IsTINUnique.ToString());
            if (value == null) { return; }
            //IEnumerable<TaxEntity> collection = _taxPayerService.GetTaxEntities(t => t.TaxPayerIdentificationNumber == validatedRegisterCBSUserModel.TIN);
        }


        /// <summary>
        /// Get tax entity by id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <returns>TaxEntity</returns>
        /// <exception cref="NoRecordFoundException">If entity record 404 </exception>
        public TaxEntity GetTaxEntityById(long taxEntityId)
        {
            TaxEntity value = _taxPayerService.GetTaxEntity(t => t.Id == taxEntityId);
            if (value == null) { throw new NoRecordFoundException(); }
            return value;
        }


        /// <summary>
        /// Save details for user registration
        /// </summary>
        /// <param name="user"></param>
        /// <param name="request"></param>
        /// <exception cref="CannotSaveTaxEntityException"></exception>
        /// <exception cref="CannotSaveTaxEntityException"></exception>
        /// <exception cref="CouldNotSaveCBSUserException"></exception>
        private long SaveCBSUserRegistration(CBSUser user, APIRequest request)
        {
            Logger.Information("Saving CBS user");
            if (!_cbsUserService.Save(user))
            {
                Logger.Error("Could not save cbs user info");
                throw new CouldNotSaveCBSUserException("Cannot save cbs user information");
            }

            Logger.Information("Saving API response");
            if (request != null && !_apiRequestRepository.Save(request))
            {
                Logger.Error("could not save api request info");
                throw new CouldNotSaveCBSUserException("Cannot save api request information");
            }
            return user.Id;
        }


        /// <summary>
        /// Check the count for email
        /// <para>throw error iff email count != 1</para>
        /// </summary>
        /// <param name="email"></param>
        /// <param name="errors"></param>
        /// <param name="fieldName"></param>
        /// <exception cref="DirtyFormDataException"></exception>
        public void CheckIfEmailExists(string email, ref List<ErrorModel> errors, string fieldName)
        {
            if (_taxPayerService.CheckCountCount(t => t.Email == email) != 1)
            {
                errors.Add(new ErrorModel { ErrorMessage = "Email address does not exist.", FieldName = fieldName });
                throw new DirtyFormDataException();
            }
        }
    }
}