using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.Middleware.Filters
{
    public class BrowserHeaderFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);  // HTTP 1.1.
            filterContext.HttpContext.Response.Cache.AppendCacheExtension("no-store, must-revalidate, proxy-revalidate");
            filterContext.HttpContext.Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            filterContext.HttpContext.Response.AppendHeader("Expires", "Thu, 01 Dec 1994 16:00:00 GMT"); // Proxies.
            filterContext.HttpContext.Response.AppendHeader("Expires", "-1"); // Proxies.
            //filterContext.HttpContext.Response.AppendHeader("Last-Modified", DateTime.Now.ToString());
            //filterContext.HttpContext.Response.AppendHeader("If-Modified-Since", DateTime.Now.AddDays(-1).ToString());
            filterContext.HttpContext.Response.AppendHeader("Cache-Control", "no-cache");
            //after I have tried everything to make every request revalidate for anon, I have resorted to this
            //when an unknown response code is set as the status code, the page must revalidate on the server side
            filterContext.HttpContext.Response.StatusCode = 777;
            base.OnActionExecuted(filterContext);
        }
    }
}