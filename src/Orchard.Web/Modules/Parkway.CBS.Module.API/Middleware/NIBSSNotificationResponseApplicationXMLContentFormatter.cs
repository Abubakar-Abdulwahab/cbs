using Newtonsoft.Json;
using Parkway.EbillsPay;
using Parkway.EbillsPay.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Parkway.CBS.Module.API.Middleware
{
    public class NIBSSNotificationResponseApplicationXMLContentFormatter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            //for calls that send accept header not application/json
            //some API test tools add the said header for each request
            //what this does is remove whatever header was added and replaces
            //therefore this request would be serialized based on the default JSON serialization impl
            //as per requirements the OnActionExecuted sets the content-type to text/xml
            actionContext.Request.Headers.Remove("Accept");
            actionContext.Request.Headers.Add("Accept", "*/*");
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            var response = new HttpResponseMessage(actionExecutedContext.Response.StatusCode);
            var responseString = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
            try
            {
                var obj = JsonConvert.DeserializeObject<NotificationResponse>(responseString);
                var xmlVal = new NIBSSEBillsPay().SerializeResponseToXML(obj);
                response.Content = new StringContent(xmlVal, Encoding.UTF8, "application/xml");
                actionExecutedContext.Response = response;
                return;
            }
            catch (Exception exception)
            {
                throw;
                /*if there is an error deserializing, that means we have the object CustomerInformationResponse as the response type */
            }
        }
    }
}