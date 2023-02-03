using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.TIN.Middleware
{
    public class BrowserHeaderFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);  // HTTP 1.1.
            filterContext.HttpContext.Response.Cache.AppendCacheExtension("no-store, must-revalidate, proxy-revalidate");
            filterContext.HttpContext.Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
            filterContext.HttpContext.Response.AppendHeader("Expires", "0"); // Proxies.
            //filterContext.HttpContext.Response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            //filterContext.HttpContext.Response.AddHeader("Pragma", "no-cache");
            //filterContext.HttpContext.Response.AddHeader("Expires", "0");
            base.OnActionExecuted(filterContext);
            // The action filter logic.
        }
    }
}