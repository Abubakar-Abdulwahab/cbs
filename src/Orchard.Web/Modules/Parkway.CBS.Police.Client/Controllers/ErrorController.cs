using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class ErrorController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;

        public ErrorController(IHandler compHandler, IAuthenticationService authenticationService) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Route name: P.Error.404
        /// </summary>
        public ActionResult Error404()
        {
            try
            {
                ErrorVM vm = new ErrorVM { HeaderObj = HeaderFiller(GetLoggedInUserDetails()) };
                return View(vm);
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in ErrorController {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute(Police.Client.RouteName.SelectService.ShowSelectService);
            }

        }
    }
}