using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAnonymous]
    public class ResetPasswordVerificationController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IAccountVerificationHandler _handler;

        public ResetPasswordVerificationController(IHandler compHandler, IAuthenticationService authenticationService, IAccountVerificationHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }


        /// <summary>
        /// Route Name: P.Resend.Password.Reset.Verification.Code
        /// 
        /// </summary>
        /// <param name="token"></param>
        [HttpPost]
        public JsonResult ResendVerificationCode(string token, bool isPasswordReset = true)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.custommessage("No token found.").ToString() });
                }
                if (_handler.TokenHasExpired(token, isPasswordReset))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = new { Refresh = true, Message = Lang.tokenexpiredredirecting.ToString() } });
                }
                QueueAVerificationCodeResend(token);
                return Json(new APIResponse { ResponseObject = Lang.codehasbeenresent.ToString() });
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                TempData = null;
                TempData.Add("Info", Lang.youraccounthasalreadybeenverified.ToString());
                return Json(new APIResponse { Error = true, ResponseObject = new { Refresh = true, Message = Lang.tokenexpiredredirecting.ToString() } });
            }
            catch (InvalidOperationException exception)
            {
                Logger.Error(exception, exception.Message + "token: " + token);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.exceededcoderetry().ToString() });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }



        /// <summary>
        /// RouteName: P.Reset.Password.Verify
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ActionResult ResetPasswordVerify(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    TempData.Add("Error", ErrorLang.pleaseconfirmemail().ToString());
                    return RedirectToRoute("P.SelectService");
                }
                //need to check that the token hasn't expired and signify that this is a password reset request
                if (_handler.TokenHasExpired(token, true))
                {
                    TempData = null;
                    TempData.Add("Info", Lang.yourprevcodeexpired.ToString());
                    return RedirectToRoute("P.Forgot.Password");
                }
                return View(new VerifyAccountModel { HeaderObj = new HeaderObj { }, Token = token });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ResetPasswordVerification {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }


        [HttpPost]
        public ActionResult ResetPasswordVerify(string token, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    TempData = null;
                    TempData.Add("Error", ErrorLang.genericexception().ToString());
                    return RedirectToRoute("P.SelectService");
                }
               _handler.DoTokenValidation(token, code, isPasswordReset: true);
                return View("AccountVerification/VerificationTransitPage", new VerifyAccountModel { HeaderObj = new HeaderObj { }, Redirecting = true, URL = Url.RouteUrl("P.Reset.Password", new { token }) });
            }
            catch (TimeoutException)
            {
                TempData = null;
                TempData.Add("Info", Lang.yourprevcodeexpired.ToString());
                return RedirectToRoute("P.Forgot.Password");
            }
            catch (NoRecordFoundException)
            {
                TempData = null;
                TempData.Add("Error", ErrorLang.nomatchingcodefound().ToString());
                return View(new VerifyAccountModel { HeaderObj = new HeaderObj { }, Token = token, FlashObj = new FlashObj { FlashType = CBS.Core.Models.Enums.FlashType.Error, Message = ErrorLang.nomatchingcodefound().ToString(), MessageTitle = ErrorLang.custommessage("Error!").ToString() } });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in ResetPasswordVerification post method {0}", exception.Message));
                TempData = null;
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }
            return RedirectToRoute("P.SelectService");
        }
    }
}