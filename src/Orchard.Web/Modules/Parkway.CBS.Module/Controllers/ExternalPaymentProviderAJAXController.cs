using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Admin;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class ExternalPaymentProviderAJAXController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAuthenticationService _authenticationService;
        public ILogger Logger { get; set; }
        private readonly IExternalPaymentProviderHandler _handler;

        public ExternalPaymentProviderAJAXController(IOrchardServices orchardServices, IAuthenticationService authenticationService, IExternalPaymentProviderHandler handler)
        {
            _orchardServices = orchardServices;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            _handler = handler;
        }      


        public JsonResult GetClientSecret(string clientId)
        {
            return Json(_handler.GetClientSecret(clientId), JsonRequestBehavior.AllowGet);
        }

    }
}