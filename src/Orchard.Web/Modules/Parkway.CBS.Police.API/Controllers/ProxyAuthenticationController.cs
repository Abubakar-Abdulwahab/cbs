using Newtonsoft.Json;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Net;
using System.Web.Http;

namespace Parkway.CBS.Police.API.Controllers
{
    [RoutePrefix("api/v1/pss/proxyauthentication")]
    public class ProxyAuthenticationController : ApiController
    {
        private readonly IPSSProxyAuthenticationHandler _proxyAuthenticationHandler;
        public ILogger Logger { get; set; }

        public ProxyAuthenticationController(IPSSProxyAuthenticationHandler proxyAuthenticationHandler)
        {
            Logger = NullLogger.Instance;
            _proxyAuthenticationHandler = proxyAuthenticationHandler;
        }

        /// <summary>
        /// Login request from the POSSAP API
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("signin")]
        public IHttpActionResult SignIn(PSSProxyAuthenticationModel model)
        {
            try
            {
                if (model == null)
                {
                    Logger.Error("ValidateUserName exception: Empty login credentials");
                    return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = "Empty login credentials" });
                }

                if (string.IsNullOrWhiteSpace(model.UserName))
                {
                    Logger.Error("ValidateUserName exception: UserName is required");
                    return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = "UserName is required" });
                }

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    Logger.Error("ValidateUserName exception: Password is required");
                    return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = "Password is required" });
                }

                Logger.Information($"{model.UserName} trying to signin");

                APIResponse response = _proxyAuthenticationHandler.ValidateUserSigninCredentials(model);
                Logger.Information($"Signin response for user: {model.UserName}, { JsonConvert.SerializeObject(response) }");
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("SignIn exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }

        [HttpPost]
        [Route("validate-username")]
        public IHttpActionResult ValidateUserName(PSSProxyAuthenticationModel model)
        {
            try
            {
                Logger.Information($"Trying to ValidateUserName: {model.UserName}");
                if (model == null || string.IsNullOrWhiteSpace(model.UserName))
                {
                    Logger.Error("ValidateUserName exception: Invalid username");
                    return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = "Invalid username" });
                }
                APIResponse response = _proxyAuthenticationHandler.ValidateUserName(model);
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("ValidateUserName exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }
    }
}