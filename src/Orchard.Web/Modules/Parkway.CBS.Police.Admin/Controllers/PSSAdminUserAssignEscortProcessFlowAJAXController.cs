using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSAdminUserAssignEscortProcessFlowAJAXController : Controller
    {
        public readonly IPSSAdminUserAssignEscortProcessFlowAJAXHandler _handler;
        public ILogger Logger { get; set; }

        public PSSAdminUserAssignEscortProcessFlowAJAXController(IPSSAdminUserAssignEscortProcessFlowAJAXHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public JsonResult GetAdminUser(string adminUsername)
        {
            try
            {
                if (string.IsNullOrEmpty(adminUsername?.Trim())) { return Json(new APIResponse { Error = true, ResponseObject = "Username not specified" }); }
                return Json(_handler.GetAdminUser(adminUsername.Trim()));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }


        public JsonResult GetEscortProcessStageDefinitions(int commandTypeId)
        {
            try
            {
                if (commandTypeId < 1) { return Json(new APIResponse { Error = true, ResponseObject = "Unit not specified" }); }
                return Json(_handler.GetEscortProcessStageDefinitions(commandTypeId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }
    }
}