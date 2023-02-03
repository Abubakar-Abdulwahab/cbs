using System;
using Orchard;
using System.Web.Mvc;
using Newtonsoft.Json;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Users.Events;
using Parkway.CBS.Core.Lang;
using Orchard.Users.Services;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;


namespace Parkway.CBS.Police.Client.Controllers
{
    public class SignOutController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        public ILogger Logger { get; set; }


        public SignOutController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
        }



        [HttpPost]
        /// <summary>
        /// Route name: P.Signout
        /// sign out a logged in user
        /// </summary>
        /// <param name="returnURL"></param>
        public virtual ActionResult Signout()
        {
            var authUser = _authenticationService.GetAuthenticatedUser();
            try
            {
                if(authUser != null)
                {
                    if (Session["PSSRequestStage"] != null) { Session.Remove("PSSRequestStage"); }
                    Logger.Information(string.Format("Signing out {0}", authUser.Id));
                    _authenticationService.SignOut();
                }
            }
            catch (Exception exception)
            {
                var message = " Exception Signing out. User Id " + authUser == null ? 0 : authUser.Id;
                Logger.Error(exception, exception.Message + message);
            }
            return RedirectToRoute("P.SelectService");

        }
    }
}