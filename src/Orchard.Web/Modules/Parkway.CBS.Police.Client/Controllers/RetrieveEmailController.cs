using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAnonymous]
    public class RetrieveEmailController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IRetrieveEmailHandler _handler;
        public RetrieveEmailController(IHandler compHandler, IAuthenticationService authenticationService, IRetrieveEmailHandler handler) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        [HttpGet]
        public ActionResult RetrieveEmail()
        {
            try
            {
                return View(new RetrieveEmailVM { HeaderObj = new HeaderObj { }, FlashObj = GetTextFromTempData("Info", FlashType.Info, "Info!") });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in RetrieveEmail {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().Text);
                return RedirectToRoute(RouteName.SelectService.ShowSelectService);
            }
        }


        [HttpPost]
        public ActionResult RetrieveEmail(RetrieveEmailVM userInput)
        {
            List<ErrorModel> errors = new List<ErrorModel> { };
            FlashObj flash = null;
            try
            {
                if (!this.ModelState.IsValid) { throw new DirtyFormDataException(); }
                RegisterUserResponse result = _handler.ValidatePhoneNumber(userInput.PhoneNumber.Trim(), ref errors);
                if (errors.Count > 0) { throw new DirtyFormDataException(); }

                if (result == null)
                {
                    flash = new FlashObj { FlashType = FlashType.Error, Message = PoliceErrorLang.ToLocalizeString("User with specified phone number does not exist").Text, MessageTitle = PoliceErrorLang.ToLocalizeString("Error").Text };
                    return View(new RetrieveEmailVM { HeaderObj = new HeaderObj { }, PhoneNumber = userInput.PhoneNumber, FlashObj = flash });
                }

                if (_handler.CheckIfCBSUserExceededResendCount(result.CBSUser.Id))
                {
                    flash = new FlashObj { FlashType = FlashType.Error, Message = PoliceErrorLang.ToLocalizeString("User with specified phone number has exceeded the resend limit for today.").Text, MessageTitle = PoliceErrorLang.ToLocalizeString("Error").Text };
                    return View(new RetrieveEmailVM { HeaderObj = new HeaderObj { }, PhoneNumber = userInput.PhoneNumber, FlashObj = flash });
                }

                return RedirectToRoute(RouteName.RetrieveEmailVerification.RetrieveEmailVerify, new { token = _compHandler.ProviderVerificationTokenSMS(result.CBSUser, VerificationType.RetrieveEmail) });
            }
            catch (DirtyFormDataException exception)
            {
                if (errors.Count > 0)
                {
                    foreach (var item in errors)
                    { this.ModelState.AddModelError(item.FieldName, item.ErrorMessage); }
                }
                Logger.Error(exception.Message, exception);
                flash = new FlashObj { FlashType = FlashType.Error, Message = ErrorLang.validationerror().Text, MessageTitle = PoliceErrorLang.ToLocalizeString("Error").Text };
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in RetrieveEmail {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().Text);
                return RedirectToRoute(RouteName.SelectService.ShowSelectService);
            }
            return View(new RetrieveEmailVM { HeaderObj = new HeaderObj { }, PhoneNumber = userInput.PhoneNumber, FlashObj = flash });
        }
    }
}