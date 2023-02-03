using Microsoft.Web.Http;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.API.Controllers.Binders;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;

namespace Parkway.CBS.Module.API.Controllers
{
    /// <summary>
    /// Version 1
    /// <para>All object responses should be contained in the APIResponse object <see cref="APIResponse"/> except incases we cannot control</para>
    /// </summary>
    [HasClientKey]
    [ApiVersion("1.0")]
    [RoutePrefix("v1/mda")]
    public class MDAController : ApiController
    {
        private readonly IAPIMDAHandler _apiMDAHandler;
        public ILogger Logger { get; set; }
        private IOrchardServices _orchardServices;


        public MDAController(IOrchardServices orchardServices, IAPIMDAHandler apiMDAHandler)
        {
            _apiMDAHandler = apiMDAHandler;
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
        }


        /// <summary>
        /// Create MDA
        /// <para>Send request as multi part, if files</para>
        /// </summary>
        /// <param name="model">CreateMDAModel <see cref="CreateMDAModel"/></param>
        /// <returns><see cref="APIResponse"/><see cref="MDACreatedModel"/></returns>
        /// <errorcodes>PPS1</errorcode>
        /// <errorcodes>PPT1</errorcode>
        /// <errocodes>PPB1</errocodes>
        /// <errocodes>PPVE</errocodes>
        /// <errocodes>PPM1</errocodes>
        /// <errocodes>PPC1</errocodes>
        /// <errocodes>PPU1</errocodes>
        /// <errocodes>PPIE</errocodes>
        [HttpPost]
        [Route("create")]
        [ResponseType(typeof(APIResponse))]
        public IHttpActionResult CreateMDA([ModelBinder(typeof(CreateMDAModelBinder))] CreateMDAModel model)
        {
            if (model == null)
            {
                Logger.Error("no data in model");
                return Content(HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ErrorCode.PPIE.ToString(), Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "MDA" } } } });
            }
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
            Logger.Error(string.Format("Create MDA request - client ID: {0} signature: {1} IP: {1} ", clientID, signature, System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"]));
            APIResponse response = _apiMDAHandler.CreateMDA(this, model, new HttpFileCollectionWrapper(HttpContext.Current.Request.Files), new { SIGNATURE = signature, CLIENTID = clientID });
            return Content(response.StatusCode, response);
        }


        /// <summary>
        /// Edit MDA
        /// <para>Send request as multi part, if files</para>
        /// </summary>
        /// <param name="model">CreateMDAModel <see cref="CreateMDAModel"/></param>
        /// <returns><see cref="APIResponse"/><see cref="MDACreatedModel"/></returns>
        /// <errorcodes>PPS1</errorcode>
        /// <errorcodes>PPT1</errorcode>
        /// <errocodes>PPB1</errocodes>
        /// <errocodes>PPVE</errocodes>
        /// <errocodes>PPM1</errocodes>
        /// <errocodes>PPC1</errocodes>
        /// <errocodes>PPU1</errocodes>
        /// <errocodes>PPIE</errocodes>
        [HttpPost]
        [Route("edit")]
        [ResponseType(typeof(APIResponse))]
        public IHttpActionResult EditMDA([ModelBinder(typeof(EditMDAModelBinder))] EditMDAModel model)
        {
            if (model == null)
            {
                Logger.Error("no data in model");
                return Content(HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ErrorCode.PPIE.ToString(), Error = true, ResponseObject = new List<ErrorModel> { { new ErrorModel { ErrorMessage = ErrorLang.genericexception().ToString(), FieldName = "MDA" } } } });
            }
            var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
            var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");
            APIResponse response = _apiMDAHandler.EditMDA(this, model, new HttpFileCollectionWrapper(HttpContext.Current.Request.Files), new { SIGNATURE = signature, CLIENTID = clientID });
            return Content(response.StatusCode, response);
        }
    }
}
