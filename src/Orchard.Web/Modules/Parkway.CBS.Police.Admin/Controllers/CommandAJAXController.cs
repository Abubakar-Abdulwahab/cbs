using System;
using System.Web.Mvc;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class CommandAJAXController : Controller
    {
        private readonly IPSSCommandHandler _handler;
        public ILogger Logger { get; set; }


        public CommandAJAXController(IPSSCommandHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        [HttpPost]
        /// <summary>
        /// Get the list of PSS commands in the specified category
        /// </summary>
        /// <param name="commandCategoryId"></param>
        public JsonResult CommandsByCommandCategory(string commandCategoryId)
        {
            Logger.Information("Getting command by command category Id " + commandCategoryId);
            if (string.IsNullOrEmpty(commandCategoryId) || commandCategoryId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid formation level." }); }
            int parsedCommandCategoryVal = 0;
            if(!Int32.TryParse(commandCategoryId, out parsedCommandCategoryVal))
            {
                return Json(new APIResponse { Error = true, ResponseObject = "Select a valid formation level." });
            }
            return Json(_handler.GetCommandsByCommandCategory(parsedCommandCategoryVal));
        }


        public JsonResult CommandsByParentCodeFormationLevel(string parentCode)
        {
            try
            {
                if (string.IsNullOrEmpty(parentCode)) { return Json(new APIResponse { Error = true, ResponseObject = "Formation code not specified" }); }
                return Json(_handler.GetCommandsByParentCode(parentCode));
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }

    }
}