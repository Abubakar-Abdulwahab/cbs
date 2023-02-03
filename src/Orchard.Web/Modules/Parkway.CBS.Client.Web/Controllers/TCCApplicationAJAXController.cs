using Orchard;
using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Core.Utilities;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class TCCApplicationAJAXController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly ITCCApplicationHandler _handler;
        private readonly IOrchardServices _orchardServices;
        private readonly ICommonHandler _commonHandler;

        public TCCApplicationAJAXController(ITCCApplicationHandler handler, IOrchardServices orchardServices, ICommonHandler commonHandler)
        {
            Logger = NullLogger.Instance;
            _handler = handler;
            _orchardServices = orchardServices;
            _commonHandler = commonHandler;
        }


        public JsonResult ValidateDevelopmentLevyInvoiceStatus(string invoiceNumber)
        {
            try
            {
                if (!string.IsNullOrEmpty(invoiceNumber))
                {
                    int developmentLevyRevenueHeadId = 0;
                    StateConfig siteConfig = Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                    Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.DevelopmentLevyRevenueHeadID.ToString()).FirstOrDefault();
                    if (!int.TryParse(node.Value, out developmentLevyRevenueHeadId))
                    {
                        Logger.Error(string.Format("Unable to convert configured Development Levy Revenue head config value"));
                        return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
                    }
                    else
                    {
                        return Json(_handler.CheckIfDevelopmentLevyInvoiceHasBeenUsed(invoiceNumber, developmentLevyRevenueHeadId));
                    }
                }
                else { return Json(new APIResponse { Error = true, ResponseObject = "Invoice number not specified." }); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }


        public JsonResult ValidatePayerId(string payerId)
        {
            try
            {
                if (!string.IsNullOrEmpty(payerId))
                {
                    return Json(_handler.CheckIfPayerIdValid(payerId,_commonHandler.GetLoggedInUserDetails().Entity.Id));
                }
                else { return Json(new APIResponse { Error = true, ResponseObject = "Payer Id not specified." }); }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }
    }
}