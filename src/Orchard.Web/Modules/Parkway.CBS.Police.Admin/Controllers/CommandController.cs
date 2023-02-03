using System;
using System.Web.Mvc;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Admin.Controllers
{
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
            if (string.IsNullOrEmpty(lgaId) || lgaId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." }); }
            int parsedLGAVal = 0;
            if(!Int32.TryParse(lgaId, out parsedLGAVal))
            {
                return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." });
            }
            return Json(_handler.GetCommands(parsedLGAVal));
        }


        /// <summary>
        /// Get the list of PSS commands for Admin in the State
        /// </summary>
        /// <param name="stateId"></param>
        /// <returns></returns>
        public JsonResult CommandsByStateForAdmin(string stateId)
        {
            try { 
            Logger.Information("Getting command by state Id for logged in admin" + stateId);
            if (string.IsNullOrEmpty(stateId) || stateId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid state." }, JsonRequestBehavior.AllowGet); }
            int parsedStateVal = 0;
            if (!Int32.TryParse(stateId, out parsedStateVal))
            {
                return Json(new APIResponse { Error = true, ResponseObject = "Select a valid state." }, JsonRequestBehavior.AllowGet);
            }
            return Json(_handler.GetCommandsForAdmin(parsedStateVal), JsonRequestBehavior.AllowGet);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = CBS.Core.Lang.ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Get the list of Area and Divisional PSS commands for Admin in the LGA
        /// </summary>
        /// <param name="lgaId"></param>
        /// <returns></returns>
        public JsonResult CommandsByLGAForDCPAdmin(string lgaId)
        {
            try
            {
                Logger.Information("Getting command by LGA Id for logged in DCP admin" + lgaId);
                if (string.IsNullOrEmpty(lgaId) || lgaId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." }, JsonRequestBehavior.AllowGet); }
                int parsedLGAVal = 0;
                if (!Int32.TryParse(lgaId, out parsedLGAVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid LGA." }, JsonRequestBehavior.AllowGet);
                }
                return Json(_handler.GetAreaAndDivisionalCommandsForAdmin(parsedLGAVal), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = CBS.Core.Lang.ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}