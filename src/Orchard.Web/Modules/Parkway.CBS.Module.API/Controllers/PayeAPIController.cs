using Microsoft.Web.Http;
using Orchard;
using Orchard.Logging;
using Parkway.CBS.Core;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Module.API.Controllers.Handlers.Contracts;
using Parkway.CBS.Module.API.Middleware;
using System.Collections.Generic;
using System.IO;
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
    [RoutePrefix("v1/PAYE")]
    public class PayeAPIController : ApiController
    {
        private readonly IAPIPAYEHandler _APIPAYEHandler;

        public PayeAPIController(IAPIPAYEHandler apiPAYEHandler)
        {
            Logger = NullLogger.Instance;
            _APIPAYEHandler = apiPAYEHandler;
        }

        public ILogger Logger { get; set; }

        [HttpPost]
        [Route("add-batch-items")]
        public IHttpActionResult AddBatchItems(PAYEAddBatchItemsRequestModel model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();

            try
            {
                LogRequest("AddBatchItems : PAYEAddBatchItemsRequestModel");

                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                APIResponse response = _APIPAYEHandler.ProcessAddBatchItemsRequest(model, new { SIGNATURE = signature, CLIENTID = clientID });
                Logger.Information(string.Format("AddBatchItems : {0}", response));

                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("AddBatchItems exception {0}", exception.Message));
                errors.Add(new ErrorModel { FieldName = "Model", ErrorMessage = ErrorLang.genericexception().ToString() });
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ErrorCode.PPIE.ToString(), Error = true, ResponseObject = errors });
            }
        }

        [HttpPost]
        [Route("initialize-batch")]
        public IHttpActionResult InitializeBatch(PAYEIntializeBatchRequestModel model)
        {
            List<ErrorModel> errors = new List<ErrorModel>();

            try
            {
                LogRequest("InitializeBatch : PAYEIntializeBatchRequestModel");

                var signature = HttpContext.Current.Request.Headers.Get("SIGNATURE");
                var clientID = HttpContext.Current.Request.Headers.Get("CLIENTID");

                APIResponse response = _APIPAYEHandler.ProcessInitializeRequest(model, new { SIGNATURE = signature, CLIENTID = clientID });
                Logger.Information(string.Format("InitializeBatchResponse : {0}", response));

                return Content(response.StatusCode, response);
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception, string.Format("InitializeBatch exception {0}", exception.Message));
                errors.Add(new ErrorModel { FieldName = "Model", ErrorMessage = ErrorLang.genericexception().ToString() });
                return Content(System.Net.HttpStatusCode.BadRequest, new APIResponse { ErrorCode = ErrorCode.PPIE.ToString(), Error = true, ResponseObject = errors });
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
            catch (System.Exception exception)
            { Logger.Error(exception.Message, "Exception in Log Req"); }
        }
    }
}