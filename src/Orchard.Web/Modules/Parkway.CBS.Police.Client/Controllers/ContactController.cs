using Orchard.Logging;
using Orchard.Security;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class ContactController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;

        public ContactController(IHandler compHandler, IAuthenticationService authenticationService) : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
        }
        // GET: Contact
        public ActionResult Contact()
        {
            try
            {
                return View(new ContactVM { HeaderObj = HeaderFiller(GetLoggedInUserDetails()) });
            }
            catch (Exception exception)
            {
                TempData = null;
                Logger.Error(exception, string.Format("Exception in Contact {0}", exception.Message));
                TempData.Add("Error", ErrorLang.genericexception().ToString());
                return RedirectToRoute("P.SelectService");
            }
        }
    }
}