using System.Web.Mvc;
using System.Web.Routing;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Client.Middleware
{
    public class PSSAnonymous : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (filterContext.HttpContext.Request.IsAuthenticated)
            //{
            //    filterContext.Controller.TempData.Add("Error", PoliceErrorLang.usernotauthorized().ToString());
            //    filterContext.Result = new RedirectToRouteResult("P.SelectService", new RouteValueDictionary { });
            //}
            //else
            //{
            //    base.OnActionExecuting(filterContext);
            //}
        }
    }
   
}