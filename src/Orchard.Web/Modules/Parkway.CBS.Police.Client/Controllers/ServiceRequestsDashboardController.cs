using Orchard.Security;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class ServiceRequestsDashboardController : BaseController
    {
        private readonly IHandler _compHandler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IPSSRequestConfirmationHandler _handler;

        public ServiceRequestsDashboardController(IHandler compHandler, IAuthenticationService authenticationService, IPSSRequestConfirmationHandler handler)
            : base(authenticationService, compHandler)
        {
            _compHandler = compHandler;
            _authenticationService = authenticationService;
            _handler = handler;
        }

        public ActionResult Approvals()
        {
            return View("ServiceRequestsAdminDash");
        }
    }
}