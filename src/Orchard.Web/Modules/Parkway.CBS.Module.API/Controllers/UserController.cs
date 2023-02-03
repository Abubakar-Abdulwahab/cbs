using Microsoft.Web.Http;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace Parkway.CBS.Module.API.Controllers
{
    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except stated otherwise</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/user")]
    public class UserController : ApiController
    {
        private readonly IAPIUserSettingsHandler _apiUserSettingsHandler;
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;


        public UserController(IOrchardServices orchardServices, IAPIUserSettingsHandler apiUserSettingsHandler)
        {
            _apiUserSettingsHandler = apiUserSettingsHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }

        [HttpPost]
        [Route("register")]
        public IHttpActionResult RegisterCBSUser(RegisterUserModel model)
        {
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

            APIResponse response = _apiUserSettingsHandler.CreateCBSUser(this, model, new { SIGNATURE = signature, CLIENTID = clientID });
            return Content(response.StatusCode, response);
        }


        [HttpPost]
        [Route("search-by-filter")]
        public IHttpActionResult SearchByFilter(TaxProfilesSearchParams searchFilter)
        {
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

            List<ErrorModel> errors = _apiUserSettingsHandler.DoModelCheck(this);
            if(errors != null && errors.Count > 0) return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { Error = true, ErrorCode = ErrorCode.PPVE.ToString(), ResponseObject = errors });

            APIResponse response = _apiUserSettingsHandler.SearchTaxProfilesByFilter(searchFilter, new { SIGNATURE = signature, CLIENTID = clientID });
            return Content(response.StatusCode, response);
        }

    }

}
