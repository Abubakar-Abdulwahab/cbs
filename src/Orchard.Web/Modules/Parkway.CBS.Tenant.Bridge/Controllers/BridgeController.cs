using System.IO;
using System.Web;
using System.Text;
using Orchard.Logging;
using System.Web.Http;
using Microsoft.Web.Http;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.API.Middleware;
using Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts;
using Newtonsoft.Json;
using System.Dynamic;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Parkway.CBS.Core;

namespace Parkway.CBS.Tenant.Bridge.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("v1/bridge/invoice")]
    public class BridgeController : ApiController
    {
        private readonly IBridgeAPIInvoiceHandler _apiInvoiceHandler;
        public ILogger Logger { get; set; }

        public BridgeController(IBridgeAPIInvoiceHandler apiInvoiceHandler)
        {
            _apiInvoiceHandler = apiInvoiceHandler;
            Logger = NullLogger.Instance;
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


        /// <summary>
        /// invoice validation for readycash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("rdc/validate-invoice")]
        public IHttpActionResult ValidateInvoiceReadyCash(ReadycashInvoiceValidationModel model)
        {
            try
            {
                LogRequest("ValidateInvoiceReadyCash : ValidationRequest");
                APIResponse response = _apiInvoiceHandler.ValidateInvoice(model);
                Logger.Information(string.Format("RDCValidateInvoice: {0} ", JsonConvert.SerializeObject(response)));
                return Content(response.StatusCode, response.ResponseObject);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
            }
        }


        /// <summary>
        /// invoice validation for readycash
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HasClientKey]
        [HttpPost]
        [Route("validation")]
        public IHttpActionResult ValidateInvoice(ReadycashInvoiceValidationModel model)
        {
            try
            {
                LogRequest("ValidateBillerInvoice : ValidateBillerInvoice");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                var billerCode = HttpContext.Current.Request.Headers.Get("BILLERCODE");

                List<ErrorModel> errors = new List<ErrorModel>();
                if (model == null || string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = "Model is empty" });
                    return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ErrorCode.PPVE.ToString(), Error = true, ResponseObject = errors });
                }

                if (!IsInvoiceFormatValid(model.InvoiceNumber.Trim()))
                {
                    errors.Add(new ErrorModel { FieldName = "InvoiceNumber", ErrorMessage = ErrorLang.invoice404().ToString() });
                    return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ResponseCodeLang.invoice_404, Error = true, ResponseObject = errors });
                }

                APIResponse response = _apiInvoiceHandler.BillerValidateInvoice(model, new { SIGNATURE = signature, CLIENTID = clientID, BILLERCODE = billerCode });
                Logger.Information(string.Format("ValidateInvoice: {0} ", JsonConvert.SerializeObject(response)));
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
            }
        }

        /// <summary>
        /// Validation of invoice for all the tenants
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("validate-invoice")]
        public IHttpActionResult Validate(ValidationRequest model)
        {
            try
            {
                LogRequest("Validate-invoice : Validate-invoice");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                dynamic headerParams = new ExpandoObject();
                headerParams.SIGNATURE = signature;
                headerParams.CLIENTID = clientID;

                List<ErrorModel> errors = new List<ErrorModel>();
                if (model == null || string.IsNullOrEmpty(model.InvoiceNumber))
                {
                    errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = "Model is empty" });
                    return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ErrorCode.PPVE.ToString(), Error = true, ResponseObject = errors });
                }

                if (!IsInvoiceFormatValid(model.InvoiceNumber.Trim()))
                {
                    errors.Add(new ErrorModel { FieldName = "InvoiceNumber", ErrorMessage = ErrorLang.invoice404().ToString() });
                    return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ResponseCodeLang.invoice_404, Error = true, ResponseObject = errors });
                }

                APIResponse response = _apiInvoiceHandler.InvoiceValidation(model, headerParams);
                Logger.Information(string.Format("ValidateResponse : {0}", JsonConvert.SerializeObject(response)));
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }

        private bool IsInvoiceFormatValid(string invoiceNumber)
        {
            //Regex Pattern: The first digit must be between 1 to 9, then followed by 00 and at least 7 digits
            string invoiceNumberPattern = @"^[1-9](00)(\d{7})";
            var matches = Regex.Matches(invoiceNumber, invoiceNumberPattern);
            if (matches.Count != 1)
            {
                return false;
            }

            return true;
        }
    }
}
