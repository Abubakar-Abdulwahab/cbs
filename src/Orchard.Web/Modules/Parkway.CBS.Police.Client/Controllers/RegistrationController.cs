using System;
using System.Linq;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Client.Controllers
{

    public class RegistrationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IRegisterUserHandler _handler;
        private readonly IHandler _compHandler;

        public RegistrationController(IHandler compHandler, IAuthenticationService authenticationService, IRegisterUserHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [PSSAnonymous]
        public ActionResult RegisterUser()
        {
            try
            {
                UserDetailsModel userDetails = GetLoggedInUserDetails(false);
                if (userDetails == null)
                {
                    RegisterPSSUserObj model = _handler.GetViewModelForUserSignup();
                    model.FlashObj = GetTextFromTempData();
                    return View(model);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in RegisterUser {0}", exception.Message));
            }
            return RedirectToRoute("P.SelectService");
        }



        [PSSAnonymous]
        [HttpPost, ActionName("RegisterUser")]
        public ActionResult RegisterUser(RegisterPSSUserObj userInput)
        {
            int taxPayerTypeParsed = 0;
            string errorMessage = string.Empty;
            List<ErrorModel> errors = new List<ErrorModel> { };
            try
            {
                if (!int.TryParse(userInput.TaxPayerType, out taxPayerTypeParsed))
                {
                    errorMessage = "invalid tax payer type specified.";
                    throw new Exception();
                }
                if (userInput.RegisterCBSUserModel.Password != userInput.RegisterCBSUserModel.ConfirmPassword)
                {
                    errors.Add(new ErrorModel { ErrorMessage = "Passwords do not match", FieldName = "RegisterCBSUserModel.Password" });
                }
                userInput.RegisterCBSUserModel.UserName = null;
                RegisterUserResponse result = _handler.RegisterCBSUserModel(ref errors, userInput, HttpContext.Request.Files.Get("identificationfile"));
                return RedirectToRoute("P.Verify.Account", new { token = _compHandler.ProviderVerificationToken(result.CBSUser, CBS.Core.Models.Enums.VerificationType.AccountVerification) });
            }
            #region catch clauses
            catch (DirtyFormDataException exception)
            {
                if (errors.Count > 0)
                {
                    if(userInput.FormErrorNumber > 1) { userInput.FlashObj = new FlashObj { FlashType = CBS.Core.Models.Enums.FlashType.Error, MessageTitle = "User Details", Message = errors.ElementAt(0).ErrorMessage }; }
                    foreach (var item in errors)
                    { this.ModelState.AddModelError(item.FieldName, item.ErrorMessage.ToString()); }
                }
                Logger.Error(exception.Message, exception);
            }
            catch (NoCategoryFoundException exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.categorynotfound().ToString();
            }
            catch (CBSUserAlreadyExistsException)
            {
                errorMessage = ErrorLang.profilealreadyexists().ToString();
            }
            catch (PhoneNumberHasBeenTakenException)
            {
                errorMessage = ErrorLang.phonenumberalreadyexists().ToString();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                errorMessage = ErrorLang.genericexception().ToString();
            }
            #endregion

            var viewModel = _handler.GetViewModelForUserSignup();
            if (userInput.RegisterCBSUserModel.SelectedState > 0)
            {
                var state = viewModel.StateLGAs.Where(s => s.Id == userInput.RegisterCBSUserModel.SelectedState).SingleOrDefault();
                userInput.ListLGAs = state != null ? state.LGAs.ToList() : null;
            }
            
            userInput.TaxCategoriesVM = viewModel.TaxCategoriesVM;
            userInput.HeaderObj = new HeaderObj { };
            userInput.Error = true;
            userInput.ErrorMessage = errorMessage;
            userInput.StateLGAs = viewModel.StateLGAs;
            userInput.PSSIdentificationTypes = _handler.GetIdentificationTypesForCategory(taxPayerTypeParsed);
            userInput.TaxCategoryPermissions = viewModel.TaxCategoryPermissions;
            userInput.FormErrorNumber = 1;
            return View(userInput);
        }


    }
}