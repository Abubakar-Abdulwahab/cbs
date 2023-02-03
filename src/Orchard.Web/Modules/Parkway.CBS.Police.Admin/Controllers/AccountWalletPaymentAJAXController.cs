using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class AccountWalletPaymentAJAXController : Controller
    {
        public readonly IAccountWalletPaymentAJAXHandler _accountWalletPaymentAJAXHandler;
        public ILogger Logger { get; set; }

        public AccountWalletPaymentAJAXController(IAccountWalletPaymentAJAXHandler accountWalletPaymentAJAXHandler)
        {
            Logger = NullLogger.Instance;
            _accountWalletPaymentAJAXHandler = accountWalletPaymentAJAXHandler;
        }

        // GET: ValidateAccountNumber
        public JsonResult ValidateAccountNumber(string accountNumber, int bankId)
        {
            try
            {
                if (string.IsNullOrEmpty(accountNumber?.Trim())) { return Json(new APIResponse { Error = true, ResponseObject = $"{nameof(accountNumber)} not specified" }); }
                Logger.Information($"About to do name enquiry. Account Number -> {accountNumber}::Bank Id -> {bankId}");
                return Json(_accountWalletPaymentAJAXHandler.ValidateAccountNumber(accountNumber.Trim(), bankId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }

        // GET: GetAccountBalance
        public JsonResult GetAccountBalance(int walletId)
        {
            try
            {
                if (walletId <= 0) { return Json(new APIResponse { Error = true, ResponseObject = $"{nameof(walletId)} not specified" }); }
                return Json(_accountWalletPaymentAJAXHandler.GetAccountBalance(walletId), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }
    }
}