using System;
using System.Web.Mvc;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Lang;

namespace Parkway.CBS.Police.Client.Controllers
{
    [CheckVerificationFilter(false)]
    public class CommandController : Controller
    {
        private readonly IPSSCommandHandler _handler;
        public ILogger Logger { get; set; }

        public CommandController(IPSSCommandHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        [HttpPost]
        /// <summary>
        /// Get the list of PSS commands in the LGA
        /// </summary>
        /// <param name="lgaId"></param>
        public JsonResult CommandsByLGA(string lgaId)
        {
            Logger.Information("Getting command by LGA Id " + lgaId);
            try
            {
                if (string.IsNullOrEmpty(lgaId) || lgaId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." }); }
                if (!int.TryParse(lgaId, out int parsedLGAVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." });
                }
                return Json(_handler.GetCommands(parsedLGAVal));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = new { Message = ErrorLang.genericexception().ToString() } }, JsonRequestBehavior.AllowGet);
                
            }
        }

        [HttpPost]
        /// <summary>
        /// Get the list of PSS commands in the State
        /// </summary>
        /// <param name="stateId"></param>
        public JsonResult CommandsByState(string stateId)
        {
            Logger.Information("Getting command by State Id " + stateId);
            try
            {
                if (string.IsNullOrEmpty(stateId) || stateId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid State." }); }
                if (!int.TryParse(stateId, out int parsedStateVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid State." });
                }
                return Json(_handler.GetAreaAndDivisionalCommandsByStateId(parsedStateVal));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = new { Message = ErrorLang.genericexception().ToString() } }, JsonRequestBehavior.AllowGet);
                
            }
        }

        [HttpPost]
        /// <summary>
        /// Get the list of PSS commands in the LGA
        /// </summary>
        /// <param name="lgaId"></param>
        public JsonResult GetAreaAndDivisionalCommandsByLGA(string lgaId)
        {
            Logger.Information("Getting command by LGA Id " + lgaId);
            try
            {
                if (string.IsNullOrEmpty(lgaId) || lgaId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." }); }
                if (!int.TryParse(lgaId, out int parsedLGAVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." });
                }
                return Json(_handler.GetAreaAndDivisionalCommandsByLGA(parsedLGAVal));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = new { Message = ErrorLang.genericexception().ToString() } }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}