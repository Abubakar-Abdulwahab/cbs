using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Admin;
using System.Web.Routing;
using Orchard.Mvc.Filters;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class PSSAnonymousFilterHandler : FilterProvider, IActionFilter
    {
        public ILogger Logger { get; set; }
        private readonly IAuthenticationService _authenticationService;

        public PSSAnonymousFilterHandler(IAuthenticationService authenticationService)
        {
            Logger = NullLogger.Instance;
            _authenticationService = authenticationService;
        }


        public void OnActionExecuted(ActionExecutedContext filterContext) { }


        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //do verification filter for only non admin requests
            if (AdminFilter.IsApplied(filterContext.RequestContext))
                return;

            foreach (var item in filterContext.ActionDescriptor.ControllerDescriptor.GetFilterAttributes(true))
            {
                if (item.GetType() == typeof(PSSAnonymous))
                {
                    CheckIfUserLoggedIn(filterContext);
                    break;
                }
            }

            foreach (var item in filterContext.ActionDescriptor.GetFilterAttributes(true))
            {
                if (item.GetType() == typeof(PSSAnonymous))
                {
                    CheckIfUserLoggedIn(filterContext);
                    break;
                }
            }
        }


        private void CheckIfUserLoggedIn(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.HttpContext.Request.IsAuthenticated)
                {
                    //do work
                    filterContext.Controller.TempData.Add("Error", PoliceErrorLang.usernotauthorized().Text);
                    filterContext.Result = new RedirectToRouteResult(RouteName.SelectService.ShowSelectService, new RouteValueDictionary { });
                    _authenticationService.SignOut();
                }
            }
            catch (Exception exception)
            { Logger.Error(exception, string.Format("Exception in PSSAnonymousFilterHandler. Exception {0}", exception.Message)); }
        }
    }
}