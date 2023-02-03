using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Core.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAnonymous]
    public class RetrieveEmailVerificationController : BaseController
    {
        private readonly IRetrieveEmailVerificationHandler _handler;

        public RetrieveEmailVerificationController(IHandler compHandler, IAuthenticationService authenticationService, IRetrieveEmailVerificationHandler handler)
            : base(authenticationService, compHandler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        /// <summary>
        /// Route Name: P.Resend.Retrieve.Email.Verification.Code
        /// </summary>
        /// <param name="token"></param>
        [HttpPost]
        public JsonResult ResendVerificationCode(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = PoliceErrorLang.ToLocalizeString("No token found.").Text });
                }
                if (_handler.TokenHasExpired(token))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = new { Refresh = true, Message = Lang.tokenexpiredredirecting.Text } });
                }
                _handler.ResendVerificationCode(token);
                return Json(new APIResponse { ResponseObject = Lang.codehasbeenresent.Text });
            }
            catch (InvalidOperationException exception)
            {
                Logger.Error(exception, exception.Message + "token: " + token);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.exceededcoderetry().Text });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text});
        }


        /// <summary>
        /// RouteName: P.Retrieve.Email.Verify
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult RetrieveEmailVerify(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    TempData.Add("Error", PoliceErrorLang.ToLocalizeString("Please confirm phone number before retrieving email").Text);
                    return RedirectToRoute(RouteName.SelectService.ShowSelectService);
                }
                //need to check that the token hasn't expired
                if (_handler.TokenHasExpired(token))
                {
                    TempData = null;
                    TempData.Add("Info", Lang.yourprevcodeexpiredphonenumber.Text);
                    return RedirectToRoute(RouteName.RetrieveEmail.RetrieveEmailAction);
                }
                return View(new VerifyAccountModel { HeaderObj = new HeaderObj { }, Token = token });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in RetrieveEmailVerify {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().Text);
            }
            return RedirectToRoute(RouteName.SelectService.ShowSelectService);
        }


        [HttpPost]
        public ActionResult RetrieveEmailVerify(string token, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    TempData.Add("Error", ErrorLang.genericexception().Text);
                    return RedirectToRoute(RouteName.SelectService.ShowSelectService);
                }
                ValidateVerReturnValue result = _handler.DoTokenValidation(token, code);
                //Send email address to user
                _handler.SendEmailAddressAsSMSToUser(result.CBSUserId);
                TempData = null;
                TempData.Add("Info", PoliceLang.ToLocalizeString("Your email has been sent to your specified phone number.").Text);

                return RedirectToRoute(RouteName.Login.SignIn);
            }
            catch (TimeoutException)
            {
                TempData = null;
                TempData.Add("Info", Lang.yourprevcodeexpiredphonenumber.Text);
                return RedirectToRoute(RouteName.RetrieveEmail.RetrieveEmailAction);
            }
            catch (NoRecordFoundException)
            {
                TempData = null;
                TempData.Add("Error", ErrorLang.nomatchingcodefound().Text);
                return View(new VerifyAccountModel { HeaderObj = new HeaderObj { }, Token = token, FlashObj = new FlashObj { FlashType = CBS.Core.Models.Enums.FlashType.Error, Message = ErrorLang.nomatchingcodefound().Text, MessageTitle = ErrorLang.custommessage("Error!").Text } });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in RetrieveEmailVerify post method {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().Text);
            }
            return RedirectToRoute(RouteName.SelectService.ShowSelectService);
        }
    }
}