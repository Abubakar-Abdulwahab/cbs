using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PrivacyTermsController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;

        public PrivacyTermsController(IHandler compHandler, IAuthenticationService authenticationService) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
        }
        // GET: PrivacyTerms
        public ActionResult PrivacyTerms()
        {
            try
            {
                return View(new PrivacyTermsVM { HeaderObj = HeaderFiller(GetLoggedInUserDetails()) });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in Contact {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute(Client.RouteName.SelectService.ShowSelectService);
            }
        }
    }
}