using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class NotificationMessageController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;

        public NotificationMessageController(IHandler compHandler, IAuthenticationService authenticationService) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
        }


        public ActionResult List()
        {
            try
            {
                return View(new NotificationMessageListVM { HeaderObj = HeaderFiller(GetLoggedInUserDetails()) });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in Contact {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute(RouteName.SelectService.ShowSelectService);
            }
        }
    }
}