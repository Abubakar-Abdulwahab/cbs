using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class AccountWalletPaymentApprovalAJAXController : Controller
    {
        public readonly IAccountWalletPaymentApprovalAJAXHandler _handler;
        public readonly IOrchardServices _orchardServices;
        public ILogger Logger { get; set; }

        public AccountWalletPaymentApprovalAJAXController(IAccountWalletPaymentApprovalAJAXHandler handler, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _handler = handler;
            _orchardServices = orchardServices;
        }

        public JsonResult GetToken()
        {
            try
            {
                Logger.Information($"About to get token for sending otp as sms and email to user with UserPartRecordId {_orchardServices.WorkContext.CurrentUser.Id} for account wallet payment");
                string token = _handler.ProvideVerificationToken(_orchardServices.WorkContext.CurrentUser.Id, CBS.Core.Models.Enums.VerificationType.AccountWalletPayment);
                return Json(new APIResponse { ResponseObject = token });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = Core.Lang.PoliceErrorLang.genericexception().Text });
            }
        }


        public JsonResult ValidateVerificationCode(string token, string code)
        {
            try
            {
                Logger.Information($"About to validate verification code for user with UserPartRecordId {_orchardServices.WorkContext.CurrentUser.Id}, token - {token} and code - {code}");

                if(string.IsNullOrEmpty(token?.Trim())) 
                {
                    Logger.Error("Token not specified");
                    return Json(new APIResponse { Error = true, ResponseObject = "Token not specified" });
                }

                if (string.IsNullOrEmpty(code?.Trim()))
                {
                    Logger.Error("Code not specified");
                    return Json(new APIResponse { Error = true, ResponseObject = "Code not specified" });
                }

                if (_handler.TokenHasExpired(token))
                {
                    Logger.Error($"Token has expired for user with UserPartRecordId {_orchardServices.WorkContext.CurrentUser.Id}.");
                    return Json(new APIResponse { Error = true, ResponseObject = "Token has expired. Reload this page to get a new one" });
                }

                _handler.DoTokenValidation(token, code);
                Logger.Information($"Successfully validated verification code for user with UserPartRecordId {_orchardServices.WorkContext.CurrentUser.Id}");
                return Json(new APIResponse { ResponseObject = "Success" });
            }
            catch (TimeoutException)
            {
                Logger.Error(CBS.Core.Lang.Lang.yourprevcodeexpiredphonenumber.Text);
                return Json(new APIResponse { Error = true, ResponseObject = "Your previous code has expired, Reload this page to get a new one" });
            }
            catch (NoRecordFoundException)
            {
                Logger.Error(CBS.Core.Lang.ErrorLang.nomatchingcodefound().Text);
                return Json(new APIResponse { Error = true, ResponseObject = "Invalid Code" });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = Core.Lang.PoliceErrorLang.genericexception().Text });
            }
        }


        public JsonResult ResendVerificationCode(string token)
        {
            try
            {
                Logger.Information($"About to resend verification code to user with UserPartRecordId {_orchardServices.WorkContext.CurrentUser.Id}, token - {token}");

                if (string.IsNullOrEmpty(token?.Trim()))
                {
                    Logger.Error("Token not specified");
                    return Json(new APIResponse { Error = true, ResponseObject = "Token not specified" });
                }

                if (_handler.TokenHasExpired(token))
                {
                    Logger.Error($"Token has expired for user with UserPartRecordId {_orchardServices.WorkContext.CurrentUser.Id}.");
                    return Json(new APIResponse { Error = true, ResponseObject = "Token has expired. Reload this page to get a new one" });
                }

                _handler.ResendVerificationCode(token);
                Logger.Information($"Resending verification code to user with UserPartRecordId {_orchardServices.WorkContext.CurrentUser.Id} is successful");
                return Json(new APIResponse { ResponseObject = CBS.Core.Lang.Lang.codehasbeenresent.Text });
            }
            catch(InvalidOperationException)
            {
                return Json(new APIResponse { Error = true, ResponseObject = CBS.Core.Lang.ErrorLang.exceededcoderetry().Text });
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = Core.Lang.PoliceErrorLang.genericexception().Text });
            }
        }
    }
}