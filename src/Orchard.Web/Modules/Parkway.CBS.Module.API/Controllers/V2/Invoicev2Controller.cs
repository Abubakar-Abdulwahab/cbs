using Orchard.Logging;
using System.Web.Http;
using Microsoft.Web.Http;
using Parkway.CBS.Core.HelperModels;
using Orchard;
using System.Web;
using System.IO;
using System.Text;
using Parkway.CBS.Module.API.Middleware;
using Parkway.CBS.Module.API.Controllers.V2.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Parkway.Cashflow.Ng.Models;
using System.Net.Http;
using System.Net;

namespace Parkway.CBS.Module.API.Controllers.V2
{

    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("2.0")]
    [RoutePrefix("v2/invoice")]
    public class InvoiceV2Controller : ApiController
    {
        private readonly IAPINIBSSInvoiceHandlerV2 _apiInvoiceHandlerV2;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;

        public InvoiceV2Controller(IOrchardServices orchardServices, IAPINIBSSInvoiceHandlerV2 apiInvoiceHandler2)
        {
            _apiInvoiceHandlerV2 = apiInvoiceHandler2;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        /// <summary>
        /// Invoice Validation for NIBSS Ebills Pay (Encrypted headers)
        /// <para>returns application/json</para>
        /// </summary>
        /// <returns>encrypted string</returns>
        [HttpPost]
        [Route("nibss-ebills/validate-invoice")]
        public HttpResponseMessage Validation()
        {
            HttpResponseMessage responseMessage = new HttpResponseMessage();
            APIResponse response = null;
            try
            {
                string signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                string Authorization = HttpContext.Current.Request.Headers.Get("Authorization");
                string hash = HttpContext.Current.Request.Headers.Get("HASH");
                string signature_method = HttpContext.Current.Request.Headers.Get("SIGNATURE_METH");
                string IP = HttpContext.Current.Request.UserHostAddress;
                Logger.Information(string.Format("Processing ValidateInvoiceNumberNIBSS Request Headers : {0}, Authorization Header : {1}, HASH Header : {2}, signature_method header : {3}, IP: {4}", signature, Authorization, hash, signature_method, IP));
                response = _apiInvoiceHandlerV2.ValidateInvoiceNIBSS(signature, Authorization, hash);
                Logger.Information(string.Format("NibssEbillsValidateInvoiceResponse : {0}", response));
                responseMessage = Request.CreateResponse(response.StatusCode);
                responseMessage.Content = new StringContent(response.ResponseObject, Encoding.UTF8, "text/plain");
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                responseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);
                responseMessage.Content = new StringContent(ErrorLang.genericexception().ToString(), Encoding.UTF8, "text/plain");
                return responseMessage;
            }
            return responseMessage;
        }
    }
}