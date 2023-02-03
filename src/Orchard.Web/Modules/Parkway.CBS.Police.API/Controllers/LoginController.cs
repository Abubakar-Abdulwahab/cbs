using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/login")]
    public class LoginController : ApiController
    {
        private readonly ILoginHandler _loginHandler;

        public ILogger Logger { get; set; }

        public LoginController(ILoginHandler loginHandler)
        {
            Logger = NullLogger.Instance;
            _loginHandler = loginHandler;
        }

        [HttpPost]
        [Route("signin")]
        public IHttpActionResult SignIn(AuthenticationModel model)
        {
            try
            {
                Logger.Information($"{model.Email} trying to login");
                if(Request.Headers.TryGetValues("Version", out var appVersion) && Request.Headers.TryGetValues("Mac", out var mac))
                {
                    Logger.Information(string.Format("PSS SignIn, MAC : {0}, Version : {1}", mac.FirstOrDefault(), appVersion.FirstOrDefault()));
                }

                APIResponse response = _loginHandler.ProcessLoginRequest(model, Request);
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("SignIn exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }
    }
}