using Orchard.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;

namespace Parkway.CBS.Route.Constraints
{
    public class AdminRouteFilter : FilterProvider, IActionFilter, IResultFilter
    {

        private readonly IOrchardServices _orchardServices;
        public AdminRouteFilter(IOrchardServices orchardServices) { _orchardServices = orchardServices; }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (TenantConfig.GetTenantConfig(_orchardServices.WorkContext.CurrentSite.SiteName) == filterContext.RequestContext.HttpContext.Request.Url.Host)
            {
                string[] routeDomain = filterContext.RequestContext.HttpContext.Request.RawUrl.Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (routeDomain.Length == 0)
                {
                    filterContext.Result = new RedirectResult("/admin");
                }
                else if (routeDomain.Length > 0)
                {
                    if (routeDomain[0].ToLower() == "p")
                    {
                        filterContext.Result = new RedirectResult("/admin");
                    }
                }
            }
            else
            {
                string[] routeDomain = filterContext.RequestContext.HttpContext.Request.RawUrl.Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
                if (routeDomain.Length > 0)
                {
                    if (routeDomain[0].ToLower() == "admin")
                    {
                        filterContext.Result = new RedirectToRouteResult(Police.Client.RouteName.ErrorPage.Error404, new RouteValueDictionary { });
                    }
                }
            }

        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {

        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {

        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {

        }
    }
}