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
    public class TermsOfUseController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrchardServices _orchardServices;

        public TermsOfUseController(IHandler compHandler, IAuthenticationService authenticationService, IOrchardServices orchardServices) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _orchardServices = orchardServices;
            Logger = NullLogger.Instance;
        }

        // GET: TermsOfUse
        public ActionResult TermsOfUse()
        {
            try
            {
                var headerObj = HeaderFiller(GetLoggedInUserDetails());
                return View(new TermsOfUseVM { HeaderObj = headerObj});
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in Terms of use {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute(RouteName.SelectService.ShowSelectService);
            }
        }
    }
}