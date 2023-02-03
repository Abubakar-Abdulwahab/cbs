using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAnonymous]
    public class ResetPasswordController : BaseController
    {

        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountVerificationHandler _accountVerHandler;
        private readonly IResetPasswordHandler _handler;

        public ResetPasswordController(IHandler compHandler, IAuthenticationService authenticationService, IAccountVerificationHandler accountVerHandler, IResetPasswordHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _accountVerHandler = accountVerHandler;
            _handler = handler;
        }


        // GET: ResetPassword
        public ActionResult ResetPassword(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    TempData.Add("Error", ErrorLang.notokenfound().ToString());
                    return RedirectToRoute("P.Sign.In");
                }
                //need to check that the token hasn't expired and signify that this is a password reset request
                if (_accountVerHandler.TokenHasExpired(token, true))
                {
                    TempData = null;
                    TempData.Add("Info", Lang.yourprevcodeexpired.ToString());
                    return RedirectToRoute("P.Forgot.Password");
                }
                return View(new ResetPasswordVM { HeaderObj = new HeaderObj { }, Token = token });
            }
            catch (Exception exception)
            {
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                Logger.Error(exception, string.Format("Exception in Reset Password {0}", exception.Message));
            }

            return RedirectToRoute("P.SelectService");
        }



        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordVM userInput, string token)
        {
            FlashObj flash = null;
            try
            {
                if (!this.ModelState.IsValid) { throw new DirtyFormDataException(); }

                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    TempData.Add("Error", ErrorLang.genericexception().ToString());
                    return RedirectToRoute("P.SelectService");
                }

                //need to check that the token hasn't expired and signify that this is a reset password request
                if (_accountVerHandler.TokenHasExpired(token, true))
                {
                    TempData = null;
                    TempData.Add("Info", Lang.yourprevcodeexpired.ToString());
                    return RedirectToRoute("P.Forgot.Password");
                }

                //Reset password
                _handler.ResetPassword(token, userInput.NewPassword);
                TempData = null;
                TempData.Add("Info", Lang.passwordresetsuccessful.ToString());
                return RedirectToRoute("P.Sign.In");
            }
            catch (DirtyFormDataException exception)
            {
                Logger.Error(exception.Message, exception);
                flash = new FlashObj { FlashType = FlashType.Error, Message = ErrorLang.validationerror().ToString(), MessageTitle = ErrorLang.custommessage("Error").ToString() };
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in Reset Password post method {0}", exception.Message));
                flash = new FlashObj { FlashType = FlashType.Error, Message = ErrorLang.genericexception().ToString(), MessageTitle = ErrorLang.custommessage("Error").ToString() };
            }

            return View(new ResetPasswordVM { HeaderObj = new HeaderObj { }, Token = userInput.Token, FlashObj = flash });
        }
    }
}