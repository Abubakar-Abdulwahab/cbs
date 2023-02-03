using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class AccountWalletConfigurationAJAXController : Controller
    {
        public readonly IAccountWalletConfigurationAJAXHandler _walletConfigurationAJAXHandler;
        public ILogger Logger { get; set; }

        public AccountWalletConfigurationAJAXController(IAccountWalletConfigurationAJAXHandler walletConfigurationAJAXHandler)
        {
            _walletConfigurationAJAXHandler = walletConfigurationAJAXHandler;
            Logger = NullLogger.Instance;
        }

        // GET: GetAdminUserDetail
        public JsonResult GetAdminUserDetail(string adminUsername)
        {
            try
            {
                if (string.IsNullOrEmpty(adminUsername.Trim())) { return Json(new APIResponse { Error = true, ResponseObject = "Username not specified" }); }
                return Json(_walletConfigurationAJAXHandler.GetAdminUserDetail(adminUsername.Trim()), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }
    }
}