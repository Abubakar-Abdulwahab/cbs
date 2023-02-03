using System;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class UserProfileController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly Lazy<IUserProfileHandler> _handler;
        private readonly Lazy<IRegisterUserHandler> _regHandler;


        public UserProfileController(IHandler compHandler, IAuthenticationService authenticationService, Lazy<IUserProfileHandler> handler, Lazy<IRegisterUserHandler> regHandler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            _regHandler = regHandler;
        }


        /// <summary>
        /// Route name: P.ConfirmUserProfile
        /// </summary>
        [Authorize]
        public ActionResult ConfirmUserProfile()
        {
            string errorMessage = string.Empty;
            string categoryType = string.Empty;
            RequestUserProfileVM viewModel = new RequestUserProfileVM();
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.ConfirmUserProfile, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in RequestUserProfile. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }
                UserDetailsModel userDetails = GetLoggedInUserDetails();

                RegisterPSSUserObj obj = _handler.Value.GetVMToConfirmUserProfile(userDetails);
                obj.HeaderObj = HeaderFiller(userDetails);
                obj.TaxPayerType = processStage.CategoryId.ToString();
                obj.PSSServiceName = processStage.ServiceName;
                obj.PSSServiceNote = processStage.ServiceNote;
                obj.PSSIdentificationTypes = _regHandler.Value.GetIdentificationTypesForCategory(processStage.CategoryId);
                obj.TaxCategoriesVM = _regHandler.Value.GetCategories().ToList();
                obj.TaxCategoryPermissions = _regHandler.Value.GetTaxCategoryPermissions().ToList();
                return View("RequestUserProfile", obj);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ConfirmUserProfile get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        /// <summary>
        /// Route name: P.ConfirmUserProfile
        /// </summary>
        [Authorize]
        [HttpPost, ActionName("ConfirmUserProfile")]
        public ActionResult ConfirmUserProfile(string placeHolder, RegisterPSSUserObj contactInfo)
        {
            string errorMessage = string.Empty;
            PSSRequestStageModel processStage = null;
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);

                processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.ConfirmUserProfile, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in RequestUserProfile. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                if (contactInfo.HasAlternativeContactInfo)
                {
                    var dataModel = new ContactEntityValidationModel
                    {
                        ContactNameFieldValue = contactInfo.AlternativeContactPersonName,
                        ContactNameFieldName = nameof(RegisterPSSUserObj.AlternativeContactPersonName),
                        ContactPhoneNumberFieldValue = contactInfo.AlternativeContactPersonPhoneNumber,
                        ContactPhoneNumberFieldName = nameof(RegisterPSSUserObj.AlternativeContactPersonPhoneNumber),
                        ContactEmailFieldValue = contactInfo.AlternativeContactPersonEmail,
                        ContactEmailFieldName = nameof(RegisterPSSUserObj.AlternativeContactPersonEmail),
                    };
                    _regHandler.Value.ValidateContactEntityInfo(dataModel, errors);
                    if (errors.Count() > 0)
                    {
                        contactInfo.AlternativeContactPersonName = dataModel.ContactNameFieldValue;
                        contactInfo.AlternativeContactPersonEmail = dataModel.ContactEmailFieldValue;
                        contactInfo.AlternativeContactPersonPhoneNumber = dataModel.ContactPhoneNumberFieldValue;
                        throw new DirtyFormDataException();
                    }
                    processStage.AlternativeContactPersonName = dataModel.ContactNameFieldValue;
                    processStage.AlternativeContactPersonEmail = dataModel.ContactEmailFieldValue;
                    processStage.AlternativeContactPersonPhoneNumber = dataModel.ContactPhoneNumberFieldValue;
                }

                RouteNameAndStage routeAndStage = _handler.Value.GetNextDirectionForRequest(processStage.ServiceType);

                processStage.Stage = routeAndStage.Stage;

                Session.Add("PSSRequestStage", JsonConvert.SerializeObject(processStage));

                return RedirectToRoute(routeAndStage.RouteName);
            }
            catch (DirtyFormDataException)
            {
                if (errors.Count() > 0)
                {
                    foreach (var error in errors)
                    {
                        this.ModelState.AddModelError(error.FieldName, error.ErrorMessage);
                    }
                }
                UserDetailsModel userDetails = GetLoggedInUserDetails();
                RegisterPSSUserObj obj = _handler.Value.GetVMToConfirmUserProfile(userDetails);
                obj.HeaderObj = HeaderFiller(userDetails);
                obj.TaxPayerType = processStage.CategoryId.ToString();
                obj.PSSServiceName = processStage.ServiceName;
                obj.PSSServiceNote = processStage.ServiceNote;
                obj.PSSIdentificationTypes = _regHandler.Value.GetIdentificationTypesForCategory(processStage.CategoryId);
                obj.TaxCategoriesVM = _regHandler.Value.GetCategories().ToList();
                obj.TaxCategoryPermissions = _regHandler.Value.GetTaxCategoryPermissions().ToList();
                obj.AlternativeContactPersonName = contactInfo.AlternativeContactPersonName;
                obj.AlternativeContactPersonEmail = contactInfo.AlternativeContactPersonEmail;
                obj.AlternativeContactPersonPhoneNumber = contactInfo.AlternativeContactPersonPhoneNumber;
                obj.HasAlternativeContactInfo = contactInfo.HasAlternativeContactInfo;

                return View("RequestUserProfile", obj);

            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ConfirmUserProfile get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }


        /// <summary>
        /// Route name: P.RequestUserProfile
        /// </summary>
        [PSSAnonymous]
        public ActionResult RequestUserProfile()
        {
            string errorMessage = string.Empty;
            string categoryType = string.Empty;
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);
                PSSRequestStageModel processStage = GetDeserializedSessionObj(ref errorMessage);

                if (!IsStageCorrect(PSSUserRequestGenerationStage.RequestUserFormStage, processStage.Stage))
                {
                    Logger.Information(string.Format("Stage mismatch in RequestUserProfile. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                RegisterPSSUserObj userObj = _regHandler.Value.GetViewModelForUserSignup();
                userObj.TaxPayerType = processStage.CategoryId.ToString();
                userObj.PSSServiceName = processStage.ServiceName;
                userObj.PSSServiceNote = processStage.ServiceNote;
                userObj.PSSIdentificationTypes = _regHandler.Value.GetIdentificationTypesForCategory(processStage.CategoryId);
                return View(userObj);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in user profile get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage)) { errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session."; }
                TempData.Add("Error", errorMessage);
            }
            return RedirectToRoute("P.SelectService");
        }



        /// <summary>
        /// Route name: P.RequestUserProfile
        /// </summary>
        [HttpPost, ActionName("RequestUserProfile")]
        [PSSAnonymous]
        public ActionResult RequestUserProfile(RegisterPSSUserObj userInput)
        {
            string errorMessage = string.Empty;
            List<ErrorModel> errors = new List<ErrorModel> { };
            FlashObj flashObj = null;
            userInput.ShowModal = false;
            PSSRequestStageModel processStage = null;
            try
            {
                TempData = null;
                CheckStageSessionValue(ref errorMessage);
                processStage = GetDeserializedSessionObj(ref errorMessage);
                userInput.PSSServiceName = processStage.ServiceName;
                userInput.PSSServiceNote = processStage.ServiceNote;

                if (!IsStageCorrect(PSSUserRequestGenerationStage.RequestUserFormStage, processStage.Stage))
                {
                    Logger.Error(string.Format("Stage mismatch in RequestUserProfile. Stage is sess: " + processStage.Stage));
                    errorMessage = "Your session could not be continued. Please fill in your details and proceed to start new session.";
                    throw new Exception();
                }

                userInput.RegisterCBSUserModel.UserName = null;
                userInput.TaxPayerType = processStage.CategoryId.ToString();
                if (userInput.RegisterCBSUserModel.Password != userInput.RegisterCBSUserModel.ConfirmPassword)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Passwords do not match", FieldName = "RegisterCBSUserModel.Password" });
                    userInput.ShowModal = true;
                }
                RegisterUserResponse result = _regHandler.Value.RegisterCBSUserModel(ref errors, userInput, HttpContext.Request.Files.Get("identificationfile"));

                RouteNameAndStage routeAndStage = _handler.Value.GetNextDirectionForRequest(processStage.ServiceType);
                processStage.Stage = routeAndStage.Stage;
                Session.Remove("PSSRequestStage");
                return RedirectToRoute("P.Verify.Account", new
                {
                    token = SendVerificationCode(result.CBSUser, CBS.Core.Models.Enums.VerificationType.AccountVerification, new RedirectReturnObject
                    {
                        RouteName = routeAndStage.RouteName,
                        RedirectObject = new
                        {
                            processStage.CategoryId,
                            processStage.ServiceId,
                            processStage.ServiceType,
                            processStage.ServicePrefix,
                            processStage.ServiceName,
                            processStage.ServiceNote,
                            processStage.SubCategoryId,
                            processStage.SubSubCategoryId,
                            routeAndStage.Stage,
                            userInput.RegisterCBSUserModel.Email,
                            userInput.RegisterCBSUserModel.Password,
                        }
                    })
                });
            }
            #region catch clauses
            catch (DirtyFormDataException exception)
            {
                if (errors.Count > 0)
                {
                    foreach (var item in errors)
                    { this.ModelState.AddModelError(item.FieldName, item.ErrorMessage.ToString()); }
                }
                Logger.Error(exception.Message, exception);
            }
            catch (CBSUserAlreadyExistsException)
            {
                flashObj = new FlashObj { FlashType = CBS.Core.Models.Enums.FlashType.Error, Message = ErrorLang.profilealreadyexists().ToString(), MessageTitle = "Validation Error!", RedirectToLogin = true };
            }
            catch (PhoneNumberHasBeenTakenException) { }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.genericexception().ToString();

                Logger.Error(exception, string.Format("Exception in user profile get {0}", exception.Message));
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = ErrorLang.genericexception().ToString();
                }
                TempData.Add("Error", errorMessage);
                return RedirectToRoute("P.SelectService");
            }
            #endregion

            var viewModel = _regHandler.Value.GetViewModelForUserSignup();
            if (userInput.RegisterCBSUserModel.SelectedState > 0)
            {
                var state = viewModel.StateLGAs.Where(s => s.Id == userInput.RegisterCBSUserModel.SelectedState).SingleOrDefault();
                userInput.ListLGAs = state != null ? state.LGAs.ToList() : null;
            }
            userInput.TaxCategoriesVM = viewModel.TaxCategoriesVM;
            userInput.HeaderObj = new HeaderObj { };
            userInput.StateLGAs = viewModel.StateLGAs;
            userInput.FlashObj = flashObj;
            userInput.PSSIdentificationTypes = (processStage.CategoryId != 0) ? _regHandler.Value.GetIdentificationTypesForCategory(processStage.CategoryId) : null;
            userInput.TaxCategoryPermissions = viewModel.TaxCategoryPermissions;
            return View(userInput);
        }


    }
}