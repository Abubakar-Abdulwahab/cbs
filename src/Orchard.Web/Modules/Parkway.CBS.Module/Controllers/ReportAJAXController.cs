using Orchard;
using Orchard.Logging;
using Orchard.Localization;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Models.Enums;
using System.Web.Mvc;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;
using System;

namespace Parkway.CBS.Module.Controllers
{
    public class ReportAJAXController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthenticationService _authenticationService;
        public ILogger Logger { get; set; }
        private Localizer T { get; }
        public readonly IReportHandler _handler;

        public ReportAJAXController(IOrchardServices orchardServices, IAuthenticationService authenticationService, IReportHandler handler)
        {
            _orchardServices = orchardServices;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            _handler = handler;
        }


        /// <summary>
        /// Get the revenue heads that belong to this MDA
        /// </summary>
        /// <param name="sId"></param>
        [HttpPost]
        public JsonResult GetInvoiceReportMDARevenueHeads(string sId, string objectToken)
        {
            string errmessage = string.Empty;
            try
            {
                string jsonObj = Core.Utilities.Util.LetsDecrypt(objectToken, Core.Utilities.AppSettingsConfigurations.EncryptionSecret);
                AccessType baseType;
                if(Enum.TryParse(jsonObj, out baseType))
                {
                    _handler.CheckForPermission(Permissions.ViewInvoiceReport);
                    return Json(_handler.GetRevenueHeads(sId, baseType), JsonRequestBehavior.AllowGet);
                }
                errmessage = "Error finding report type";
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                errmessage = ErrorLang.usernotauthorized().ToString();
            }
            catch (System.Exception)
            {
                errmessage = ErrorLang.genericexception().ToString();
            }
            return Json(new APIResponse { Error = true, ResponseObject = errmessage }, JsonRequestBehavior.AllowGet);
        }


    }
}