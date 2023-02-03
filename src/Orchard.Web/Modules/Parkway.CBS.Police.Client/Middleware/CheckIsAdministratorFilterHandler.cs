using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Admin;
using System.Web.Routing;
using Orchard.Mvc.Filters;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Orchard;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class CheckIsAdministratorFilterHandler : FilterProvider, IActionFilter
    {
        public ILogger Logger { get; set; }
        private readonly Lazy<IHandler> _handler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrchardServices _orchardServices;

        public CheckIsAdministratorFilterHandler(Lazy<IHandler> handler, IAuthenticationService authenticationService, IOrchardServices orchardServices)
        {
            Logger = NullLogger.Instance;
            _authenticationService = authenticationService;
            _handler = handler;
            _orchardServices = orchardServices;
        }


        public void OnActionExecuted(ActionExecutedContext filterContext) { }


        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //do is administrator filter for only non admin requests
            if (AdminFilter.IsApplied(filterContext.RequestContext))
                return;

            foreach (var item in filterContext.ActionDescriptor.ControllerDescriptor.GetFilterAttributes(true))
            {
                if (item.GetType() == typeof(CheckIsAdministratorFilter))
                {
                    int userPartId = 0;
                    //do work
                    try
                    {
                        if (((CheckIsAdministratorFilter)item).DoIsAdministratorCheck())
                        {
                            //do work
                            string tenant = _orchardServices.WorkContext.CurrentSite.SiteName;
                            IUser currentUser = _authenticationService.GetAuthenticatedUser();
                            if (currentUser == null) { return; }
                            userPartId = currentUser.Id;
                            CBS.Core.HelperModels.TaxEntityCategorySettings taxEntityCategorySettings = CacheProvider.ObjectCacheProvider.GetCachedObject<CBS.Core.HelperModels.TaxEntityCategorySettings>(tenant, $"{nameof(POSSAPCachePrefix.TaxEntityCategorySettings)}-{userPartId}");

                            if (taxEntityCategorySettings == null)
                            {
                                taxEntityCategorySettings = _handler.Value.GetUserDetails(currentUser.Id).CategoryVM.GetSettings();

                                if (taxEntityCategorySettings != null)
                                {
                                    CacheProvider.ObjectCacheProvider.TryCache(tenant, $"{nameof(POSSAPCachePrefix.TaxEntityCategorySettings)}-{userPartId}", taxEntityCategorySettings);
                                }
                            }
                            //check if this use is an administrator or if this user can view sub users request report
                            if (!taxEntityCategorySettings.CanShowSubUsersRequestReport || !_handler.Value.CheckIfUserIsAdministrator(currentUser.Id))
                            {
                                filterContext.Controller.TempData.Add("Error", PoliceErrorLang.usernotauthorized().ToString());
                                filterContext.Result = new RedirectToRouteResult(RouteName.SelectService.ShowSelectService, new RouteValueDictionary { });
                            }
                        }
                    }
                    catch (Exception exception)
                    { Logger.Error(exception, string.Format("Exception in CheckIsAdministratorFilterHandler. UserPartRecord Id {0}, Exception {1}", userPartId, exception.Message)); }
                    break;
                }
            }
        }
    }
}