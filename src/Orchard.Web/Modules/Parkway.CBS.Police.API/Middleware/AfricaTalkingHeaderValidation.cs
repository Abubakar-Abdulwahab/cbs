using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Parkway.CBS.Police.API.Middleware
{
    public class AfricaTalkingHeaderValidation : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //For calls that send accept header not application/json
            //some API test tools add the said header for each request
            //what this does is remove whatever header was added and set the content type to application/json
            actionContext.Request.Content.Headers.Remove("Content-Type");
            actionContext.Request.Content.Headers.Add("Content-Type", "application/json");
        }
    }
}