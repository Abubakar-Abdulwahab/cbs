using Microsoft.Web.Http;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.API.Controllers.V2.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Parkway.CBS.Module.API.Controllers.V2
{

    /// <summary>
    /// Version 2
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("2.0")]
    [RoutePrefix("v2/payment")]
    public class PaymentV2Controller : ApiController
    {
        private readonly INIBSSPaymentHandlerV2 _apiPaymentHandlerV2;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;


        public PaymentV2Controller(IOrchardServices orchardServices, INIBSSPaymentHandlerV2 apiPaymentHandlerV2)
        {
            _apiPaymentHandlerV2 = apiPaymentHandlerV2;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Payment notification for NIBSS EBills pay (Encrypted Headers)
        /// </summary>
        /// <returns>encrypted string</returns>
        [HttpPost]
        [Route("nibss-ebills/payment-notification")]
        public HttpResponseMessage PaymentNotification()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            APIResponse response = null;
            try
            {
                string signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                string Authorization = HttpContext.Current.Request.Headers.Get("Authorization");
                string hash = HttpContext.Current.Request.Headers.Get("HASH");
                string IP = HttpContext.Current.Request.UserHostAddress;
                string signature_method = HttpContext.Current.Request.Headers.Get("SIGNATURE_METH");
                Logger.Information(string.Format("NIBSSEBillsPayPaymentNotification Processing Request Headers : {0}, Authorization Header : {1}, HASH Header : {2}, signature_method header : {3}, IP: {4}", signature, Authorization, hash, signature_method, IP));
                response = _apiPaymentHandlerV2.NIBSSPaymentNotif(signature, Authorization, hash);
                Logger.Information(string.Format("NibssEbillsResponse : {0}", response));
                responseMessage = Request.CreateResponse(response.StatusCode);
                responseMessage.Content = new StringContent(response.ResponseObject, Encoding.UTF8, "text/plain");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "NIBSSEBillsPayPaymentNotification ERROR: " + exception.Message);
                responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
                responseMessage.Content = new StringContent(ErrorLang.genericexception().ToString(), Encoding.UTF8, "text/plain");
                return responseMessage;
            }
            return responseMessage;
        }
    }
}