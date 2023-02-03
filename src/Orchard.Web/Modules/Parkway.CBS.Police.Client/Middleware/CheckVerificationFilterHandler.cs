using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Admin;
using System.Web.Routing;
using Orchard.Mvc.Filters;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class CheckVerificationFilterHandler : FilterProvider, IActionFilter
    {
        public ILogger Logger { get; set; }
        private readonly Lazy<IHandler> _handler;
        private readonly IAuthenticationService _authenticationService;

        public CheckVerificationFilterHandler(Lazy<IHandler> handler, IAuthenticationService authenticationService)
        {
            Logger = NullLogger.Instance;
            _authenticationService = authenticationService;
            _handler = handler;
        }


        public void OnActionExecuted(ActionExecutedContext filterContext) { }


        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //do verification filter for only non admin requests
            if (AdminFilter.IsApplied(filterContext.RequestContext))
                return;

            foreach (var item in filterContext.ActionDescriptor.ControllerDescriptor.GetFilterAttributes(true))
            {
                if(item.GetType() == typeof(CheckVerificationFilter))
                {
                    int userPartId = 0;
                    //do work
                    try
                    {
                        if (((CheckVerificationFilter)item).DoVerificationCheck())
                        {
                            //do work
                            IUser currentUser = _authenticationService.GetAuthenticatedUser();
                            if (currentUser == null) { return; }
                            userPartId = currentUser.Id;
                            UserDetailsModel userDetails = _handler.Value.CheckIfUserIsVerified(currentUser.Id);
                            if (userDetails == null) { _authenticationService.SignOut(); return; }
                            //check if this use is verified
                            if (!userDetails.CBSUserVM.Verified)
                            {
                                string token = _handler.Value.ProviderVerificationToken(userDetails.CBSUserVM, CBS.Core.Models.Enums.VerificationType.AccountVerification);
                                filterContext.Result = new RedirectToRouteResult("P.Verify.Account", new RouteValueDictionary { { "token", token } });
                            }
                        }
                    }
                    catch (Exception exception)
                    { Logger.Error(exception, string.Format("Exception in CheckVerificationFilterHandler. UserPartRecord Id {0}, Exception {1}", userPartId, exception.Message )); }
                    break;
                }
            }
        }
    }
}