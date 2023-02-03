using Orchard;
using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class AboutController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrchardServices _orchardServices;

        public AboutController(IHandler compHandler, IAuthenticationService authenticationService, IOrchardServices orchardServices) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        // GET: About
        public ActionResult About()
        {
            try
            {
                return View(new AboutVM { HeaderObj = HeaderFiller(GetLoggedInUserDetails()), TenantName = _orchardServices.WorkContext.CurrentSite.SiteName });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in About {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute("P.SelectService");
            }
        }
    }
}