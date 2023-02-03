using Microsoft.Web.Http;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using System;
using System.Net;
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
    [RoutePrefix("v1/billing")]
    public class BillingController : ApiController
    {
        private readonly IAPIBillingHandler _apiBillingHandler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;


        public BillingController(IOrchardServices orchardServices, IAPIBillingHandler apiBillingHandler)
        {
            _apiBillingHandler = apiBillingHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Create billing
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="APIResponse"/></returns>
        [ResponseType(typeof(APIResponse))]
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateBilling(BillingHelperModel model)
        {
            try
            {
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                APIResponse response = _apiBillingHandler.CreateBilling(this, model, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("CreateBilling exception {0}", exception.Message));
                return Content(HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }


        /// <summary>
        /// Edit billing
        /// </summary>
        /// <param name="model">BillingHelperModel</param>
        /// <returns>APIResponse</returns>
        [ResponseType(typeof(APIResponse))]
        [HttpPost]
        [Route("edit")]
        public IHttpActionResult EditBilling(BillingHelperModel model)
        {
            try
            {
                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
                APIResponse response = _apiBillingHandler.EditBilling(this, model, new { SIGNATURE = signature, CLIENTID = clientID });
                return Content(response.StatusCode, response);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, string.Format("EditBilling exception {0}", exception.Message));
                return Content(HttpStatusCode.BadRequest, new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception() });
            }
        }
    }
}
