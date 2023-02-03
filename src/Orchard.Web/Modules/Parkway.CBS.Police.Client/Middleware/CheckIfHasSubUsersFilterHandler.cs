using System;
using System.Web.Mvc;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Admin;
using System.Web.Routing;
using Orchard.Mvc.Filters;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Orchard;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class CheckIfHasSubUsersFilterHandler : FilterProvider, IActionFilter
    {
        public ILogger Logger { get; set; }
        private readonly Lazy<IHandler> _handler;
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrchardServices _orchardServices;

        public CheckIfHasSubUsersFilterHandler(Lazy<IHandler> handler, IAuthenticationService authenticationService, IOrchardServices orchardServices)
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
                if (item.GetType() == typeof(HasSubUsersFilter))
                {
                    int userPartId = 0;
                    //do work
                    try
                    {
                        if (((HasSubUsersFilter)item).HasSubUsers())
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

                            //check if this user can view sub users request report
                            if (!taxEntityCategorySettings.CanShowSubUsersRequestReport)
                            {
                                filterContext.Result = new RedirectToRouteResult(RouteName.RequestList.ShowRequestList, new RouteValueDictionary { });
                            }
                        }
                    }
                    catch (Exception exception)
                    { Logger.Error(exception, string.Format("Exception in CheckIfHasSubUsersFilterHandler. UserPartRecord Id {0}, Exception {1}", userPartId, exception.Message)); }
                    break;
                }
            }
        }
    }
}