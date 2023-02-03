using Orchard.Logging;
using System.Web.Http;
using Microsoft.Web.Http;
using Parkway.CBS.Core.HelperModels;
using Orchard;
using System.Web;
using Parkway.CBS.Module.API.Middleware;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Module.API.Controllers
{

    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/report")]
    public class ReportController : ApiController
    {
        // GET: Collection
        private readonly IAPIReportHandler _apiReportHandler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;


        public ReportController(IOrchardServices orchardServices, IAPIReportHandler apiReportHandler)
        {
            _apiReportHandler = apiReportHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Get collection report
        /// <para>Report showing the payments made for a particular MDA or all MDAs</para>
        /// </summary>
        /// <param name="model"></param>
        /// <returns>CollectionReportViewModel (json)</returns>
        [HttpPost]
        [Route("collection-report")]
        public IHttpActionResult CollectionReport(CollectionReportViewModel model)
        {
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

            APIResponse response = _apiReportHandler.GetCollectionReport(this, model, new { SIGNATURE = signature, CLIENTID = clientID });
            return Content(response.StatusCode, response);
        }


    }
}