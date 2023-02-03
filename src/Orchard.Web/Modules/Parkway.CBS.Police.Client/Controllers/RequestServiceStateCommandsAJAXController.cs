using System;
using System.Web.Mvc;
using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Lang;
using Orchard.Security;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Client.Controllers
{
    [PSSAuthorizedAJAX]
    [HasSubUsersFilter(false)]
    public class RequestServiceStateCommandsAJAXController : BaseController
    {
        private readonly IPSSCommandHandler _handler;
        public ILogger Logger { get; set; }

        public RequestServiceStateCommandsAJAXController(IPSSCommandHandler handler, IAuthenticationService authenticationService, IHandler compHandler) : base(authenticationService, compHandler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }

        
        [HttpPost]
        /// <summary>
        /// Get the list of PSS commands in the State
        /// </summary>
        /// <param name="stateId"></param>
        public JsonResult GetCommands(string stateId)
        {
            Logger.Information("Getting command by State Id " + stateId);
            try
            {
                string errorMessage = string.Empty;
                if (string.IsNullOrEmpty(stateId) || stateId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid State." }); }
                if (!int.TryParse(stateId, out int parsedStateVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid State." });
                }
                return Json(_handler.GetCommandsByStateAndService(parsedStateVal, GetDeserializedSessionObj(ref errorMessage).ServiceId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}