using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PoliceOfficerAJAXController : Controller
    {
        private readonly IPoliceOfficerReportHandler _policeOfficerReportHandler;
        public ILogger Logger { get; set; }

        public PoliceOfficerAJAXController(IPoliceOfficerReportHandler policeOfficerReportHandler)
        {
            _policeOfficerReportHandler = policeOfficerReportHandler;
            Logger = NullLogger.Instance;
        }


        [HttpPost]
        /// <summary>
        /// Get list of police officers of a specified rank in a specified command
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="rankId"></param>
        /// <returns></returns>
        public JsonResult PoliceOfficersByCommandAndRankId(string commandId, string rankId)
        {
            Logger.Information($"Getting police officers by command Id {commandId} and rank Id {rankId}");
            if (string.IsNullOrEmpty(commandId) || commandId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid Command." }); }
            if (string.IsNullOrEmpty(rankId) || rankId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid Rank." }); }
            int parsedCommandVal = 0;
            if (!Int32.TryParse(commandId, out parsedCommandVal))
            {
                return Json(new APIResponse { Error = true, ResponseObject = "Select a valid Command." });
            }
            long parsedRankVal = 0;
            if(!long.TryParse(rankId, out parsedRankVal))
            {
                return Json(new APIResponse { Error = true, ResponseObject = "Select a valid Rank." });
            }
            return Json(_policeOfficerReportHandler.GetPoliceOfficersByCommandAndRankId(parsedCommandVal,parsedRankVal));
        }
    }
}