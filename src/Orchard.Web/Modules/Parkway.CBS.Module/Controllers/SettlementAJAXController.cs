using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Parkway.CBS.Core.Lang;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Microsoft.Reporting.WebForms;
using Orchard.Security;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;
using Parkway.DataExporter.Implementations.Util;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using System.Threading.Tasks;
using System.Web;

namespace Parkway.CBS.Module.Controllers
{
    public class SettlementAJAXController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthenticationService _authenticationService;
        public ILogger Logger { get; set; }
        private Localizer T { get; }
        private readonly ISettlementHandler _handler;


        public SettlementAJAXController(IOrchardServices orchardServices, IAuthenticationService authenticationService, ISettlementHandler handler)
        {
            _orchardServices = orchardServices;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            _handler = handler;
        }



        [HttpPost]
        public JsonResult MDARevenueHeads(string sId)
        {
            return Json(_handler.GetRevenueHeads(sId), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GetRevenueHeadsPerMda(string mdaIds)
        {
            try
            {
                return Json(_handler.GetRevenueHeadsPerMda(mdaIds), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }
    }
}