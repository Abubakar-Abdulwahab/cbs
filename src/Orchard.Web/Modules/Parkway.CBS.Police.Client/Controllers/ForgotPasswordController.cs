using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAnonymous]
    public class ForgotPasswordController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IForgotPasswordHandler _handler;

        public ForgotPasswordController(IHandler compHandler, IAuthenticationService authenticationService, IForgotPasswordHandler handler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// RouteName: P.Forgot.Password
        /// </summary>
        public ActionResult ForgotPassword()
        {
            try
            {
                FlashObj flashObj = null;
                try
                {
                    flashObj = GetTextFromTempData("Info", FlashType.Info, "Info!");
                }
                catch (Exception exception)
                { Logger.Error(exception, string.Format("Exception getting removing PSSRequestStage from session value {0}", exception.Message)); }

                return View(new ForgotPasswordVM { HeaderObj = new HeaderObj { }, Email = null, FlashObj = flashObj });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in ForgotPassword {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute("P.SelectService");
            }
        }



        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordVM userInput)
        {
            string errorMessage = string.Empty;
            List<ErrorModel> errors = new List<ErrorModel> { };
            FlashObj flash = null;
            try
            {
                if (!this.ModelState.IsValid) { throw new DirtyFormDataException(); }

                _handler.ValidateEmail(userInput.Email.Trim(), ref errors, null);
                RegisterUserResponse result = _handler.GetRegisteredUserResponseObjByEmail(userInput.Email.Trim(), ref errors, null);
                return RedirectToRoute("P.Reset.Password.Verify", new { token = _compHandler.ProviderVerificationToken(result.CBSUser, VerificationType.ForgotPassword) });
            }
            catch (DirtyFormDataException exception)
            {
                if (errors.Count > 0)
                {
                    foreach (var item in errors)
                    { this.ModelState.AddModelError(item.FieldName, item.ErrorMessage.ToString()); }
                }
                Logger.Error(exception.Message, exception);
                flash = new FlashObj { FlashType = FlashType.Error, Message = ErrorLang.validationerror().ToString(), MessageTitle = ErrorLang.custommessage("Error").ToString() };
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in ForgotPassword {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute("P.SelectService");
            }
            return View(new ForgotPasswordVM { HeaderObj = new HeaderObj { }, Email = userInput.Email, FlashObj = flash });
        }


    }
}