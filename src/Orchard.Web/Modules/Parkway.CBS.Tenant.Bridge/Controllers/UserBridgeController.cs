using System.IO;
using System.Web;
using System.Text;
using Orchard.Logging;
using System.Web.Http;
using Parkway.CBS.Core;
using Microsoft.Web.Http;
using Parkway.CBS.Core.Lang;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.API.Middleware;
using Parkway.CBS.Tenant.Bridge.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Tenant.Bridge.Controllers
{
    [ApiVersion("1.0")]
    [RoutePrefix("api/v1/bridge/user")]
    public class UserBridgeController : ApiController
    {
        private readonly IBridgeAPIUserHandler _handler;
        public ILogger Logger { get; set; }


        public UserBridgeController(IBridgeAPIUserHandler handler)
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
        [Route("search-by-filter")]
        [HttpOptions]
        [CORSHeaderValidation]
        public IHttpActionResult SearchByFilter(TaxProfilesSearchParams model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            string errorCode = ResponseCodeLang.generic_exception_code;
            try
            {
                LogRequest("SearchByFilter : SearchByFilter");
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                var billerCode = HttpContext.Current.Request.Headers.Get("BILLERCODE");

                errors = _handler.DoModelCheck(this);
                if (errors != null && errors.Count > 0) return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = errors });

                APIResponse response = _handler.SearchForTaxProfileByFilter(model, new { SIGNATURE = signature, CLIENTID = clientID, BILLERCODE = billerCode });
                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("SearchByFilter exception {0}", exception.Message));
                errors.Add(new ErrorModel { FieldName = "SearchModel", ErrorMessage = ErrorLang.genericexception().ToString() });
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = errors, ErrorCode = errorCode });
            }
        }
    }
}