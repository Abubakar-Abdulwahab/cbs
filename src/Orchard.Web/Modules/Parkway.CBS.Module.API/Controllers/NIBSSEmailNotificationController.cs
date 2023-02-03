using Microsoft.Web.Http;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using System.IO;
using System.Linq;
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
    [RoutePrefix("v1/nibss-email-notification")]
    public class NIBSSEmailNotificationController : ApiController
    {
        private readonly INIBSSEmailNotificationHandler _nibssEmailNotificationHandler;
        public ILogger Logger { get; set; }


        public NIBSSEmailNotificationController(INIBSSEmailNotificationHandler nibssEmailNotificationHandler)
        {
            _nibssEmailNotificationHandler = nibssEmailNotificationHandler;
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
        /// Send email containing IV and secret credentials to NIBSS email address 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("reset")]
        public IHttpActionResult SendNIBSSIntegrationCredentials()
        {
            APIResponse response = null;
            try
            {
                LogRequest("SendNIBSSIntegrationCredentials : NIBSSEmailNotification");

                response = _nibssEmailNotificationHandler.SendNIBSSIntegrationCredentials();
                Logger.Information(string.Format("SendNIBSSIntegrationCredentials : {0}", response));
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Content(System.Net.HttpStatusCode.BadRequest, ErrorLang.genericexception().ToString());
            }
            return Content(response.StatusCode, response);
        }
    }
}