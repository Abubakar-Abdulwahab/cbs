using Orchard.Logging;
using System.Web.Http;
using Microsoft.Web.Http;
using Parkway.CBS.Core.HelperModels;
using Orchard;
using System.Web;
using System.IO;
using System.Text;
using System.Web.Http.ModelBinding;
using Parkway.CBS.Module.API.Middleware;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Controllers.Binders;
using Parkway.CBS.Core.Lang;
using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core;
using System.Collections.Generic;

namespace Parkway.CBS.Module.API.Controllers
{

    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/invoice")]
    public class InvoiceController : ApiController
    {
        private readonly IAPIInvoiceHandler _apiInvoiceHandler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;

        public InvoiceController(IOrchardServices orchardServices, IAPIInvoiceHandler apiInvoiceHandler)
        {
            _apiInvoiceHandler = apiInvoiceHandler;
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


        /// <summary>
        /// Create invoice
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="APIResponse"/></returns>
        [HttpPost]
        [Route("create/multiple")]
        public IHttpActionResult GenerateInvoice(CreateInvoiceUserInputModel model)
        {
            try
            {
                LogRequest("GenerateInvoice : CreateInvoiceModel");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                APIResponse response = _apiInvoiceHandler.GenerateInvoice(this, model, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("CreateInvoice exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        /// <summary>
        /// Create invoice
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="APIResponse"/></returns>
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateInvoice(CreateInvoiceModel model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            string errorCode = ResponseCodeLang.generic_exception_code;
            try
            {
                LogRequest("CreateInvoice : CreateInvoiceModel");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                errors = _apiInvoiceHandler.DoModelCheck(this);
                if (errors != null && errors.Count > 0) return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = errors });

                APIResponse response = _apiInvoiceHandler.CreateInvoice(model, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string .Format("CreateInvoice exception {0}", exception.Message));
                errors.Add(new ErrorModel { FieldName = "Invoice", ErrorMessage = ErrorLang.genericexception().ToString() });
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = errors, ErrorCode = errorCode });
            }
        }


        [HttpPost]
        [Route("validate-invoice")]
        public IHttpActionResult ValidateInvoiceNumber(ValidationRequest model)
        {
            try
            {
                LogRequest("ValidateInvoiceNumber : ValidationRequest");
                APIResponse response = _apiInvoiceHandler.ValidateInvoice(model);
                Logger.Information(string.Format("ValidateInvoiceResponse : {0}", response));
                return Content(response.StatusCode, response.ResponseObject);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        /// <summary>
        /// Validation of invoice
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("validate")]
        public IHttpActionResult Validate(ValidationRequest model)
        {
            try
            {
                LogRequest("Validate : Validate");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                APIResponse response = _apiInvoiceHandler.InvoiceValidation(model, new { SIGNATURE = signature, CLIENTID = clientID });
                Logger.Information(string.Format("ValidateResponse : {0}", response));
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        [HttpPost]
        [Route("validate-invoice/flat")]
        public IHttpActionResult ValidateInvoiceNumberFlat(ValidationRequest model)
        {
            try
            {
                LogRequest("ValidateInvoiceNumber : ValidationRequest");
                APIResponse response = _apiInvoiceHandler.ValidateInvoice(model, true);
                Logger.Information(string.Format("ValidateInvoiceFlatResponse : {0}", response));
                return Content(response.StatusCode, response.ResponseObject);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        /// <summary>
        /// Generate an invoice for paye assessment
        /// <para>This invoice paye amount has already been pre computated, so not additional computation from the adapter is needed</para>
        /// </summary>
        /// <param name="model">ProcessPayeModel</param>
        [HttpPost]
        [Route("process-paye")]
        public IHttpActionResult ProcessPaye([ModelBinder(typeof(ProcessPayeeScheduleBinder))] ProcessPayeModel model)
        {
            try
            {
                LogRequest("ProcessPaye : ProcessPayeModel");

                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                var file = HttpContext.Current.Request.Files.Get("assessmentfile");

                APIResponse response = _apiInvoiceHandler.ProcessPayeeInvoice(this, model, file, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ProcessPaye exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        /// <summary>
        /// Invoice Validation for NIBSS Ebills Pay
        /// <para>returns application/xml</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>string</returns>
        [NIBSSValidationResponseApplicationXMLContentFormatter]
        [HttpPost]
        [Route("nibss-ebills/validate-invoice")]
        public IHttpActionResult ValidateInvoiceNumberNIBSS()
        {
            APIResponse response = null;
            try
            {
                LogRequest("ValidateInvoiceNumberNIBSS : ValidationRequest");
                InvoiceIssuerAndVendorCode c = new InvoiceIssuerAndVendorCode { };
                string requestStreamString = string.Empty;
                using (var stream = Request.Content.ReadAsStreamAsync().Result)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        requestStreamString = reader.ReadToEnd();
                    }
                }
                Logger.Information(string.Format("Processing ValidateInvoiceNumberNIBSS request Request Body : {0}", requestStreamString));
                response = _apiInvoiceHandler.ValidateInvoiceNIBSS(requestStreamString);
                Logger.Information(string.Format("NibssEbillsValidateInvoiceResponse : {0}", response));
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Content(System.Net.HttpStatusCode.BadRequest, ErrorLang.genericexception().ToString());
            }

            return Content(response.StatusCode, response.ResponseObject);
        }


        [HttpPost]
        [Route("batchresponse")]
        public IHttpActionResult BatchResponse(CashflowBatchCustomerAndInvoicesResponse model)
        {
            LogRequest("batchresponse");

            APIResponse response = _apiInvoiceHandler.BatchInvoiceResponse(this, model);
            return Content(response.StatusCode, response);
        }

        [HttpPost]
        [Route("status")]
        public IHttpActionResult QueryStatus(ValidationRequest model)
        {
            try
            {
                LogRequest("QueryInvoice : QueryInvoice");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                APIResponse response = _apiInvoiceHandler.GetInvoiceStatus(model, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("QueryInvoice exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        [HttpPost]
        [Route("invalidate")]
        public IHttpActionResult Invalidate(ValidationRequest model)
        {
            try
            {
                LogRequest("QueryInvoice : QueryInvoice");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                APIResponse response = _apiInvoiceHandler.InvalidateInvoice(model, new { SIGNATURE = signature, CLIENTID = clientID });
                Logger.Information(string.Format("InvalidateResponse : {0}", response));
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("QueryInvoice exception {0}", exception.Message));
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }

    }
}