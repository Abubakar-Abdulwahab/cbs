using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class PSSEscortDetailsAJAXController : Controller
    {
        private readonly IPSSEscortHandler _handler;
        public ILogger Logger { get; set; }
        public PSSEscortDetailsAJAXController(IPSSEscortHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        public JsonResult GetServiceCategoryTypes(string serviceCategoryId)
        {
            try
            {
                int serviceCategoryIdParsed = 0;
                if (!string.IsNullOrEmpty(serviceCategoryId) && int.TryParse(serviceCategoryId, out serviceCategoryIdParsed))
                {
                    return Json(new APIResponse { ResponseObject = _handler.GetCategoryTypesForServiceCategoryWithId(serviceCategoryIdParsed) });
                }

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }


        public JsonResult GetTacticalSquads(string commandTypeId)
        {
            try
            {
                int commandTypeIdParsed = 0;
                if (!string.IsNullOrEmpty(commandTypeId) && int.TryParse(commandTypeId, out commandTypeIdParsed))
                {
                    return Json(new APIResponse { ResponseObject = _handler.GetCommandsForCommandTypeWithId(commandTypeIdParsed) });
                }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }


        public JsonResult GetNextLevelCommandsWithCode(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    return Json(new APIResponse { ResponseObject = _handler.GetNextLevelCommandsWithCode(code) });
                }
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }


        public JsonResult GetStateFormations(string stateId, string lgaId)
        {
            try
            {
                int stateIdParsed = 0;
                int lgaIdParsed = 0;
                if (!string.IsNullOrEmpty(stateId) && int.TryParse(stateId, out stateIdParsed))
                {
                    if (!string.IsNullOrEmpty(lgaId)) { int.TryParse(lgaId, out lgaIdParsed); }
                    return Json(new APIResponse { ResponseObject = _handler.GetAreaAndDivisionalCommandsByStateAndLGA(stateIdParsed, lgaIdParsed) });
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }
    }
}