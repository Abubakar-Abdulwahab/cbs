using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/charactercertificate")]
    public class CharacterCertificateController : ApiController
    {
        private readonly ICharacterCertificateHandler _handler;

        public ILogger Logger { get; set; }

        public CharacterCertificateController(ICharacterCertificateHandler handler)
        {
            Logger = NullLogger.Instance;
            _handler = handler;
        }

        [HttpGet]
        [Route("details")]
        public IHttpActionResult GetCharacterCertificateDetails(string fileNumber, string token)
        {
            try
            {
                LogRequest("GetCharacterCertificateDetails : GetCharacterCertificateDetails");
                Logger.Information(string.Format("GetCharacterCertificateDetails File Number : {0}, token : {1}", fileNumber, token));
                if (Request.Headers.TryGetValues("Version", out var appVersion) && Request.Headers.TryGetValues("Mac", out var mac))
                {
                    Logger.Information(string.Format($"SubmitBiometrics, MAC : {mac.FirstOrDefault()}, Version : {appVersion.FirstOrDefault()}"));
                }
                APIResponse response = _handler.GetCharacterCertificateDetails(fileNumber, token, Request);
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("GetCharacterCertificateDetails exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }

        [HttpPost]
        [Route("biometrics")]
        public IHttpActionResult SubmitBiometrics(CharacterCertificateBiometricRequestVM model)
        {
            try
            {
                Logger.Information($"SubmitBiometrics, Request File Number: {model.FileNumber}, Token: {model.Token}");
                if (Request.Headers.TryGetValues("Version", out var appVersion) && Request.Headers.TryGetValues("Mac", out var mac))
                {
                    Logger.Information(string.Format($"SubmitBiometrics, MAC : {mac.FirstOrDefault()}, Version : {appVersion.FirstOrDefault()}"));
                }
                APIResponse response = _handler.SaveCharacterCertificateBiometrics(model, Request);
                return Content(HttpStatusCode.OK, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("SubmitBiometrics exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
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
            { Logger.Error(exception.Message, "Exception in Log Req"); }
        }

    }
}