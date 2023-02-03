using Parkway.CBS.Core.Utilities;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Module.Web.Middleware.Filters
{
    public class AnonymousOnly : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                //filterContext.Result = new RedirectToRouteResult("C.SelfAssessment", new RouteValueDictionary { });
                object area = string.Empty;
                if(filterContext.RouteData.Values.TryGetValue("area", out area))
                {
                    string prefix = Util.GetRouteNamePrefix((string) area);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        filterContext.Result = new RedirectToRouteResult(prefix+"C.SelfAssessment", new RouteValueDictionary { });
                        return;
                    }
                }
             filterContext.Result = new RedirectToRouteResult("C.SelfAssessment", new RouteValueDictionary { });
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
        }
    }
}