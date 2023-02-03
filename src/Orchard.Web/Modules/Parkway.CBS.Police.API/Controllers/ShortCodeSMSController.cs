using System;
using System.IO;
using System.Web;
using System.Net;
using System.Text;
using System.Web.Http;
using Orchard.Logging;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Orchard;
using Newtonsoft.Json;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/shortcodesms")]
    public class ShortCodeSMSController : ApiController
    {
        private readonly IShortCodeSMSHandler _handler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;

        public ShortCodeSMSController(IOrchardServices orchardServices, IShortCodeSMSHandler handler)
        {
            _orchardServices = orchardServices;
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
            catch (Exception exception)
            { Logger.Error(exception.Message, "Exception ing Log Req"); }
        }


        /// <summary>
        /// This handles content update
        /// </summary>
        /// <returns><see cref="APIResponse"/></returns>
        [HttpPost]
        [Route("content")]
        public IHttpActionResult ContentUpdate(ShortCodeSMSRequestModel model)
        {
            try
            {
                Logger.Information($"ShortCodeSMSContent: {JsonConvert.SerializeObject(model)}");
                if (model == null)
                {
                    return Content(HttpStatusCode.BadRequest, new APIResponse{ Error = true, StatusCode = HttpStatusCode.BadRequest, ResponseObject = "Model is empty" });
                }

                APIResponse response = _handler.ProcessRequest(model);
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("ShortCodeSMSContent exception {0}", exception.Message));
                return Content(HttpStatusCode.BadRequest, new APIResponse { Error = true, StatusCode = HttpStatusCode.BadRequest, ResponseObject = "Sorry something went wrong while processing your request. Please try again later or contact admin." });
            }
        }

    }
}