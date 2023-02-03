using Microsoft.Web.Http;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Parkway.CBS.Module.API.Controllers
{
    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except stated otherwise</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/revenuehead")]
    public class RevenueHeadController : ApiController
    {

        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly IAPIRevenueHeadHandler _apiRevenueHeadHandler;


        public RevenueHeadController(IOrchardServices orchardServices, IAPIRevenueHeadHandler apiRevenueHeadHandler)
        {
            _apiRevenueHeadHandler = apiRevenueHeadHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Create revenue head
        /// <para>Send request as multi part, if files else send as application/json</para>
        /// </summary>
        /// <param name="model">CreateMDAModel</param>
        /// <returns><see cref="APIResponse"/>MDACreatedModel</returns>
        /// <response></response>
        [ResponseType(typeof(APIResponse))]
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateRevenueHead(CreateRevenueHeadRequestModel model)
        {
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
            APIResponse response = _apiRevenueHeadHandler.CreateRevenueHead(this, model, new { SIGNATURE = signature, CLIENTID = clientID });
            return Content(response.StatusCode, response);
        }


        /// <summary>
        /// Create revenue head
        /// <para>Send request as multi part, if files else send as application/json</para>
        /// </summary>
        /// <param name="model">CreateMDAModel</param>
        /// <returns><see cref="APIResponse"/>MDACreatedModel</returns>
        /// <response></response>
        [ResponseType(typeof(APIResponse))]
        [HttpPost]
        [Route("edit")]
        public IHttpActionResult EditRevenueHead(EditRevenueHeadModel model)
        {
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
            APIResponse response = _apiRevenueHeadHandler.EditRevenueHead(this, model, new { SIGNATURE = signature, CLIENTID = clientID });
            return Content(response.StatusCode, response);
        }
    }
}
