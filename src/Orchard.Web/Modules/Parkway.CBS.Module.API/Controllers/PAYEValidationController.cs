using Orchard.Logging;
using System.Web.Http;
using Microsoft.Web.Http;
using Parkway.CBS.Core.HelperModels;
using Orchard;
using System.Web;
using Parkway.CBS.Module.API.Middleware;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using System.IO;
using System.Text;
using Parkway.CBS.Core.Lang;
using System.Collections.Generic;
using Parkway.CBS.Core;

namespace Parkway.CBS.Module.API.Controllers
{

    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/paye")]
    public class PAYEValidationController : ApiController
    {
        private readonly IAPIPAYEValidationHandler _apiPAYEValidateHandler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;


        public PAYEValidationController(IOrchardServices orchardServices, IAPIPAYEValidationHandler apiPAYEValidateHandler)
        {
            _apiPAYEValidateHandler = apiPAYEValidateHandler;
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
        /// Validate PAYE batch items
        /// </summary>
        /// <param name="model"></param>
        /// <returns>APIResponse</returns>
        [HttpPost]
        [Route("validate-batch")]
        public IHttpActionResult ValidateBatch(PAYEValidateBatchModel model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();

            try
            {
                LogRequest("PAYE : BatchValidation");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                APIResponse response = _apiPAYEValidateHandler.ValidateBatchItem(model, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("BatchValidation exception {0}", exception.Message));
                errors.Add(new ErrorModel { FieldName = "Model", ErrorMessage = ErrorLang.genericexception().ToString() });
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ErrorCode.PPIE.ToString(), Error = true, ResponseObject = errors });
            }

        }


    }
}