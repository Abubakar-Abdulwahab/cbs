using System.Web.Http.Filters;

namespace Parkway.CBS.Module.API.Middleware
{
    public class CORSHeaderValidation : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            //For calls that validate CORS
            //SPA first send preflight request that comes with HttpMethod OPTIONS to test if the origin and other header parameters are allowed
            //Options part return the origin, headers that are allowed and http status ok to preflight request
            if (actionExecutedContext.Request.Method.Method == "OPTIONS")
            {
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Headers", "clientid, billercode, signature, Content-Type, Accept, Pragma, Cache-Control");
                actionExecutedContext.Response.StatusCode = System.Net.HttpStatusCode.OK;
            }
            else
            {
                actionExecutedContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            }
        }
    }
}