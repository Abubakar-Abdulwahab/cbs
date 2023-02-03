using Microsoft.Web.Http;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Utilities;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using Parkway.ThirdParty.Payment.Processor.Models;
using Parkway.ThirdParty.Payment.Processor.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Parkway.CBS.Module.API.Controllers
{

    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/payment")]
    public class PaymentController : ApiController
    {
        private readonly IAPIPaymentHandler _apiPaymentHandler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;


        public PaymentController(IOrchardServices orchardServices, IAPIPaymentHandler apiPaymentHandler)
        {
            _apiPaymentHandler = apiPaymentHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        private void LogRequest(string endPointDescription)
        {
            try
            {
                string requestBody = string.Empty;
                string IP = HttpContext.Current.Request.UserHostAddress;

                using (var stream = new MemoryStream())
                {
                    var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                    context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                    context.Request.InputStream.CopyTo(stream);
                    requestBody = Encoding.UTF8.GetString(stream.ToArray());
                }
                Logger.Information(string.Format("{0} dump {1} IP: {2} ", endPointDescription, requestBody, IP));
            }
            catch (System.Exception exception)
            { Logger.Error(exception.Message, "Exception ing Log Req"); }
        }


        [HttpPost]
        [Route("paye-notification")]
        public IHttpActionResult PayePaymentNotification(PaymentNotification model)
        {
            LogRequest("PayePaymentNotification : PaymentNotification");
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
            APIResponse response = _apiPaymentHandler.PayePaymentNotification(model, new { SIGNATURE = signature, CLIENTID = clientID });
            Logger.Information(string.Format("PayeNotificationResponse : {0}", response));
            return Content(response.StatusCode, response);
        }


        [NIBSSNotificationResponseApplicationXMLContentFormatter]
        [HttpPost]
        [Route("nibss-ebills/payment-notification")]
        public IHttpActionResult NIBSSEBillsPayPaymentNotification()
        {
            APIResponse response = null;
            try
            {
                string requestStreamString = string.Empty;
                using (var stream = Request.Content.ReadAsStreamAsync().Result)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        requestStreamString = reader.ReadToEnd();
                    }
                }
                Logger.Information(string.Format("NIBSSEBillsPayPaymentNotification Processing Request Body : {0}", requestStreamString));
                response = _apiPaymentHandler.NIBSSPaymentNotif(requestStreamString);
                Logger.Information(string.Format("NibssEbillsResponse : {0}", response));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "NIBSSEBillsPayPaymentNotification ERROR: " + exception.Message);
                return Content(HttpStatusCode.BadRequest, ErrorLang.genericexception().ToString());
            }
            return Content(response.StatusCode, response.ResponseObject);
        }


        /// <summary>
        /// payment notification for bankcollect
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("payment-notification")]
        public IHttpActionResult PaymentNotification(PaymentNotification model)
        {
            string requestBody = string.Empty;

            using (var stream = new MemoryStream())
            {
                var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                context.Request.InputStream.CopyTo(stream);
                requestBody = Encoding.UTF8.GetString(stream.ToArray());
            }
            Logger.Information(string.Format("Validating invoice dump {0}", requestBody));

            APIResponse response = _apiPaymentHandler.PaymentNotification(model);
            Logger.Information(string.Format("PaymentNotificationResponse : {0}", response));
            return Content(response.StatusCode, response.ResponseObject);
        }


        /// <summary>
        /// payment notification for bankcollect
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("notification")]
        public IHttpActionResult Notification(PaymentNotification model)
        {
            string requestBody = string.Empty;

            using (var stream = new MemoryStream())
            {
                var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                context.Request.InputStream.CopyTo(stream);
                requestBody = Encoding.UTF8.GetString(stream.ToArray());
            }

            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

            Logger.Information(string.Format("Payment Notification dump {0}", requestBody));
            model.RequestDump = requestBody;
            APIResponse response = _apiPaymentHandler.PaymentNotification(model, new { SIGNATURE = signature, CLIENTID = clientID });
            Logger.Information(string.Format("NotificationResponse : {0}", response));
            return Content(response.StatusCode, response);
        }


        [PayDirectResponseTextXMLFormatter]
        [HttpPost]
        [Route("pay-direct")]
        public IHttpActionResult PayDirectPaymentProcessing()
        {
            try
            {
                //validate the IP
                string payDirectIP = HttpContext.Current.Request.UserHostAddress;

                PayDirectConfigurations payDirectConfig = PaymentProcessorUtil.GetConfigurations<PayDirectConfigurations>(Util.GetAppRemotePath(), _orchardServices.WorkContext.CurrentSite.SiteName);
                //validate IPs
                if (payDirectConfig == null)
                {
                    Logger.Error("Could not find config for pay direct");
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }
                var IPsConfig = payDirectConfig.ConfigNodes.Where(node => node.Key == "IPs").FirstOrDefault();

                if (IPsConfig == null)
                {
                    Logger.Error("PAY DIRECT ::: Could not find IP " + payDirectIP);
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }

                string stringIPs = IPsConfig.Value;
                if (string.IsNullOrEmpty(stringIPs))
                {
                    Logger.Error("PAY DIRECT ::: Could not find IP from file config " + payDirectIP);
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }

                //check if any of the IPs match the pay direct IP
                var IPs = stringIPs.Split(',');
                var ipValue = IPs.Where(ip => ip.Trim() == payDirectIP).FirstOrDefault();

                if (string.IsNullOrEmpty(ipValue))
                {
                    Logger.Error("PAY DIRECT ::: Could not match paydirect IP file config " + payDirectIP);
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }

                string requestStreamString = string.Empty;
                using (var stream = Request.Content.ReadAsStreamAsync().Result)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        requestStreamString = reader.ReadToEnd();
                    }
                }
                Logger.Information(string.Format("Processing Pay Direct request for IP : {0}, Request Body : {1}", payDirectIP, requestStreamString));
                PayDirectAPIResponseObj response = _apiPaymentHandler.ProcessPaymentRequestForPayDirect(requestStreamString, payDirectConfig);
                Logger.Information(string.Format("PayDirectResponse : {0}", response));
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Content(HttpStatusCode.BadRequest, ErrorLang.genericexception().ToString());
            }
        }

        [HttpPost]
        [Route("netpay/payment-notification")]
        public IHttpActionResult NetPayPaymentNotification(NetPayTransactionVM model)
        {
            APIResponse response = null;
            try
            {
                LogRequest("NetPayNotification : NetPayNotification");
                response = _apiPaymentHandler.NetPayPaymentNotification(this, model);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "NetPayPaymentNotification ERROR: " + exception.Message);
                return Content(HttpStatusCode.BadRequest, ErrorLang.genericexception().ToString());
            }
            return Content(response.StatusCode, response.ResponseObject);
        }



        [PayDirectResponseTextXMLFormatter]
        [HttpPost]
        [Route("pay-direct/flat")]
        public IHttpActionResult PayDirectFlatPaymentProcessing()
        {
            try
            {
                //validate the IP
                string payDirectIP = HttpContext.Current.Request.UserHostAddress;

                PayDirectConfigurations payDirectConfig = PaymentProcessorUtil.GetConfigurations<PayDirectConfigurations>(Util.GetAppRemotePath(), _orchardServices.WorkContext.CurrentSite.SiteName);
                //validate IPs
                if (payDirectConfig == null)
                {
                    Logger.Error("Could not find config for pay direct flat");
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }
                var IPsConfig = payDirectConfig.ConfigNodes.Where(node => node.Key == "FlatIPs").FirstOrDefault();

                if (IPsConfig == null)
                {
                    Logger.Error("PAY DIRECT flat ::: Could not find IP " + payDirectIP);
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }

                string stringIPs = IPsConfig.Value;
                if (string.IsNullOrEmpty(stringIPs))
                {
                    Logger.Error("PAY DIRECT flat ::: Could not find IP from file config " + payDirectIP);
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }

                //check if any of the IPs match the pay direct IP
                var IPs = stringIPs.Split(',');
                var ipValue = IPs.Where(ip => ip.Trim() == payDirectIP).FirstOrDefault();

                if (string.IsNullOrEmpty(ipValue))
                {
                    Logger.Error("PAY DIRECT flat ::: Could not match paydirect IP file config " + payDirectIP);
                    return Content(HttpStatusCode.OK, new PayDirectAPIResponseObj { ResponseObject = ErrorLang.couldnotverifyIP().ToString() });
                }

                string requestStreamString = string.Empty;
                using (var stream = Request.Content.ReadAsStreamAsync().Result)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        requestStreamString = reader.ReadToEnd();
                    }
                }
                Logger.Information(string.Format("Processing Pay Direct flat request for IP : {0}, Request Body : {1}", payDirectIP, requestStreamString));
                PayDirectAPIResponseObj response = _apiPaymentHandler.ProcessPaymentRequestForPayDirect(requestStreamString, payDirectConfig, true);
                Logger.Information(string.Format("PayDirectFlatResponse : {0}", response));
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Content(HttpStatusCode.BadRequest, ErrorLang.genericexception().ToString());
            }
        }

    }
}