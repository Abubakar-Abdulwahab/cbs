using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Parkway.CBS.Module.API.Middleware
{
    public class NIBSSEncryptedResponseTextPlainContentFormatter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //for calls that send accept header not application/json
            //some API test tools add the said header for each request
            //what this does is remove whatever header was added and replaces
            //therefore this request would be serialized based on the default JSON serialization impl
            //as per requirements the OnActionExecuted sets the content-type to text/plain
            actionContext.Request.Headers.Remove("Accept");
            actionContext.Request.Headers.Add("Accept", "text/plain");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var response = new HttpResponseMessage(actionExecutedContext.Response.StatusCode);
            var responseString = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
            try
            {
                response.Content = new StringContent(responseString, Encoding.UTF8, "text/plain");
                actionExecutedContext.Response = response;
                return;
            }
            catch (Exception exception)
            {
                throw;
            }
        }
    }
}