using Microsoft.Web.Http;
using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Parkway.CBS.Tenant.Bridge.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v1/bridge/payment")]
    public class PaymentBridgeController : ApiController
    {

        private readonly IBridgeAPIPaymentHandler _apiPaymentHandler;
        public ILogger Logger { get; set; }

        public PaymentBridgeController(IBridgeAPIPaymentHandler apiPaymentHandler)
        {
            _apiPaymentHandler = apiPaymentHandler;
            Logger = NullLogger.Instance;
        }


        private string LogRequest(string endPointDescription)
        {
            string IP = string.Empty;
            try
            {
                string requestBody = string.Empty;
                IP = HttpContext.Current.Request.UserHostAddress;

                using (var stream = new MemoryStream())
                {
                    var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    context.Request.InputStream.CopyTo(stream);
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());
                }
                Logger.Information(string.Format("{0} dump {1} IP: {2} ", endPointDescription, requestBody, IP));
                return requestBody;
            }
            catch (System.Exception exception)
            { Logger.Error(exception.Message, string.Format("Exception in Log Req {0} IP: {1}", endPointDescription, IP)); }
            return ErrorLang.genericexception().ToString();
        }


        /// <summary>
        /// invoice validation for readycash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("rdc/notification")]
        public IHttpActionResult ReadycashPaymentNotification(ReadyCashPaymentNotification model)
        {
            try
            {
                model.RequestDump = LogRequest("ReadycashPaymentNotification : ReadycashPaymentNotification");
                APIResponse response = _apiPaymentHandler.PaymentNotification(model);
                return Content(response.StatusCode, response.ResponseObject);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ReadycashPaymentNotification exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        /// <summary>
        /// payment notification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("notification")]
        public IHttpActionResult PaymentNotification(PaymentNotification model)
        {
            try
            {
                model.RequestDump = LogRequest("PaymentNotification : PaymentNotification");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                var billerCode = HttpContext.Current.Request.Headers.Get("BILLERCODE");

                APIResponse response = _apiPaymentHandler.ProcessPaymentNotification(model, new { SIGNATURE = signature, CLIENTID = clientID, BILLERCODE = billerCode });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        /// <summary>
        /// check for transaction with the given ref
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("rdc/requery-ref")]
        public IHttpActionResult ReadycashRequeryReference(PaymentNotification model)
        {
            try
            {
                model.RequestDump = LogRequest("RequeryReference : RequeryReference");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                var billerCode = HttpContext.Current.Request.Headers.Get("BILLERCODE");

                APIResponse response = _apiPaymentHandler.RequeryTransaction(model, new { SIGNATURE = signature, CLIENTID = clientID, BILLERCODE = billerCode });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }

        /// <summary>
        /// payment notification for all the tenants
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("payment-notification")]
        public IHttpActionResult Notification(PaymentNotification model)
        {
            try
            {
                LogRequest("GenericPaymentNotification : GenericPaymentNotification");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                dynamic headerParams = new ExpandoObject();
                headerParams.SIGNATURE = signature;
                headerParams.CLIENTID = clientID;

                APIResponse response = _apiPaymentHandler.GenericePaymentNotification(model, headerParams);
                Logger.Information(string.Format("NotificationResponse : {0}", JsonConvert.SerializeObject(response)));
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("GenericPaymentNotification exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }

        }

    }
}
