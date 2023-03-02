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

    [RoutePrefix("api/vq/pss/extract")]
    public class PSSExtractController : ApiController
    {

        public ILogger Logger { get; set; }

        public PSSExtractController( )
        {
            Logger = NullLogger.Instance;
        }

        [HttpGet]
        [Route("formData")]
        public IHttpActionResult GetPSSFormData()
        {
            try
            {
                //Logger.Information($"{model.Email} trying to login");
                //if (Request.Headers.TryGetValues("Version", out var appVersion) && Request.Headers.TryGetValues("Mac", out var mac))
                //{
                //    Logger.Information(string.Format("PSS SignIn, MAC : {0}, Version : {1}", mac.FirstOrDefault(), appVersion.FirstOrDefault()));
                //}

                return Content(HttpStatusCode.OK, "");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("SignIn exception {0}", exception.Message));
                return Content(HttpStatusCode.OK, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }
    }
}