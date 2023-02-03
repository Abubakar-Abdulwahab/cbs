using System.IO;
using System.Web;
using System.Text;
using Orchard.Logging;
using System.Web.Http;
using Microsoft.Web.Http;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.API.Middleware;
using System.Collections.Generic;
using Parkway.CBS.Core;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Newtonsoft.Json;

namespace Parkway.CBS.Module.API.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v1/statetin")]
    public class StateTINController : ApiController
    {
        private readonly IAPIStateTINHandler _handler;
        public ILogger Logger { get; set; }

        public StateTINController(IAPIStateTINHandler handler)
        {
            _handler = handler;
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
        /// generate invoice for this agent
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        [HttpOptions]
        [CORSHeaderValidation]
        public IHttpActionResult CreateStateTIN(CreateStateTINModel model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            string errorCode = ResponseCodeLang.generic_exception_code;
            try
            {
                LogRequest("StateTINCreation : StateTINCreation");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                errors = _handler.DoModelCheck(this);
                if (errors != null && errors.Count > 0) return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = errors });

                APIResponse response = _handler.CreateStateTIN(model, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateInvoiceNumber exception {0}", exception.Message));
                errors.Add(new ErrorModel { FieldName = "StateTIN", ErrorMessage = ErrorLang.genericexception().ToString() });
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = errors, ErrorCode = errorCode });
            }
        }
    }
}
