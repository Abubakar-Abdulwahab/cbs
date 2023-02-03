using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.ThirdParty.Payment.Processor.Models;
using Parkway.ThirdParty.Payment.Processor.Processors;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Xml;

namespace Parkway.CBS.Module.API.Middleware
{
    /// <summary>
    /// Format customer details response to text/xml
    /// </summary>
    public class PayDirectResponseTextXMLFormatter : ActionFilterAttribute
    {
        public ILogger Logger { get; set; }

        public PayDirectResponseTextXMLFormatter()
        {
            Logger = NullLogger.Instance;
        }

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
            PayDirectAPIResponseObj resObj = null;
            var response = new HttpResponseMessage(actionExecutedContext.Response.StatusCode);
            var responseString = actionExecutedContext.Response.Content.ReadAsStringAsync().Result;
            //lets get the type
            resObj = JsonConvert.DeserializeObject<PayDirectAPIResponseObj>(responseString);

            if(string.IsNullOrEmpty(resObj.ReturnType))
            {
                try
                {
                    Logger.Information(string.Format("Transforming response string {0} in string format", responseString));
                    response.Content = new StringContent(resObj.ResponseObject, Encoding.UTF8, "text/xml");
                    actionExecutedContext.Response = response;
                    return;
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("Exception in string serializer str : {0}, exception: {1}", responseString, exception.Message));
                    /*if there is an error deserializing, that means we have the object CustomerInformationResponse as the response type */
                }
            }
            //very quick hack, 

            if (resObj.ReturnType == "CustomerInformationResponse")
            {
                try
                {
                    Logger.Information(string.Format("Transforming response string {0} in CustomerInformationResponse format", responseString));
                    var objString = JsonConvert.SerializeObject(resObj.ResponseObject);
                    CustomerInformationResponse customerResponse = JsonConvert.DeserializeObject<CustomerInformationResponse>(objString);
                    if (customerResponse.Customers != null)
                    {
                        Logger.Information(string.Format("Serializing response string {0} in CustomerInformationResponse format", responseString));

                        var xml = PaymentProcessorUtil.SerializeResponseToXML(customerResponse);
                        response.Content = new StringContent(xml.ToString(), Encoding.UTF8, "text/xml");
                        actionExecutedContext.Response = response;
                        return;
                    }
                }
                catch (Exception exception)
                {
                    Logger.Error(exception, string.Format("Exception in CustomerInformationResponse serializer str : {0}, exception {1}", responseString, exception.Message));
                }
            }
              

            try
            {
                Logger.Information(string.Format("Transforming response string {0} in PaymentNotificationResponse format", responseString));
                var objString = JsonConvert.SerializeObject(resObj.ResponseObject);
                PaymentNotificationResponse paymentNotification = JsonConvert.DeserializeObject<PaymentNotificationResponse>(objString);
                var paymentXML = PaymentProcessorUtil.SerializeResponseToXML(paymentNotification);
                response.Content = new StringContent(paymentXML.ToString(), Encoding.UTF8, "text/xml");
                actionExecutedContext.Response = response;
                return;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("Exception in PaymentNotificationResponse serializer str : {0}, exception {1}", responseString, exception.Message));
            }

            response.Content = new StringContent(ErrorLang.genericexception().ToString(), Encoding.UTF8, "text/xml");
            actionExecutedContext.Response = response;
        }
    }
}