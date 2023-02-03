using Newtonsoft.Json;
using Parkway.CBS.Core.HelperModels;
using Parkway.EbillsPay;
using Parkway.EbillsPay.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Xml.Linq;

namespace Parkway.CBS.Module.API.Middleware
{
    public class NIBSSValidationResponseApplicationXMLContentFormatter : ActionFilterAttribute
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
                var obj = JsonConvert.DeserializeObject<ValidationResponse>(responseString);
                var xmlVal = new NIBSSEBillsPay().SerializeResponseToXML(obj);
                //xmlVal = "<?xml version=\"1.0\" encoding=\"utf - 8\" standalone=\"yes\"?><ValidationResponse><BillerID>279</BillerID><NextStep>0</NextStep><ResponseCode>00</ResponseCode><Param><Key>InvoiceNumber</Key><Value>1000010445</Value></Param><Param><Key>Amount</Key><Value>1</Value></Param><Param><Key>PhoneNumber</Key><Value>09087937483</Value></Param><Param><Key>Email</Key><Value>adminoags@nassarawaigr.ng</Value></Param><Param><Key>Name</Key><Value>OAGS</Value></Param><Param><Key>Status</Key><Value>Valid</Value></Param></ValidationResponse>";
                //var sfd = XElement.Parse(xmlVal).ToString(SaveOptions.DisableFormatting);

                //response.Content = new StringContent(XElement.Parse(xmlVal).ToString(SaveOptions.OmitDuplicateNamespaces), Encoding.UTF8, "application/xml");
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