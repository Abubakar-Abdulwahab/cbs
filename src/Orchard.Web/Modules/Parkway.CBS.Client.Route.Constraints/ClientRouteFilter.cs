using Orchard.Mvc.Filters;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Client.Route.Constraints
{
    public class ClientRouteFilter : FilterProvider, IActionFilter, IResultFilter
    {

        public ClientRouteFilter()
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.RawUrl.Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries).Length == 0)
                return;

            if (filterContext.RequestContext.HttpContext.Request.RawUrl.Split(new[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries)[0] != "p")
                filterContext.Result = new RedirectToRouteResult(Police.Client.RouteName.ErrorPage.Error404, new RouteValueDictionary { });
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