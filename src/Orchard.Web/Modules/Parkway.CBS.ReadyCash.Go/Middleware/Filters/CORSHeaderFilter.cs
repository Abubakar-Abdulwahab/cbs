using System.Web.Mvc;

namespace Parkway.CBS.ReadyCash.Go.Middleware.Filters
{
    public class CORSHeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //For calls that validate CORS
            //SPA first send preflight request that comes with HttpMethod OPTIONS to test if the origin and other header parameters are allowed
            //Options part return the origin, headers that are allowed and http status ok to preflight request
            if (filterContext.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "clientid, billercode, signature, Content-Type, Accept, Pragma, Cache-Control");
                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
            }
            else
            {
                filterContext.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
        }

    }
}