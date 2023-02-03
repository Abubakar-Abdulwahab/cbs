using System.Web.Mvc;
using Orchard.Mvc.Filters;

namespace Parkway.CBS.Admin.Route.Constraints
{
    public class AdminRouteFilter : FilterProvider, IActionFilter, IResultFilter
    {

        public AdminRouteFilter()
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string[] routeDomain = filterContext.RequestContext.HttpContext.Request.RawUrl.Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
            if (routeDomain.Length > 0)
            {
                if (routeDomain[0].ToLower() == "p")
                {
                    filterContext.Result = new RedirectResult("/admin");
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