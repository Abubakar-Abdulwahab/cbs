using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSAdminUserAJAXController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IPSSAdminUserHandler _adminUserHandler;

        public PSSAdminUserAJAXController(IPSSAdminUserHandler adminUserHandler)
        {
            _adminUserHandler = adminUserHandler;
            Logger = NullLogger.Instance;
        }


        public JsonResult GetFlowDefinitionsForService(string serviceTypeId)
        {
            try { 
            if (string.IsNullOrEmpty(serviceTypeId)) { return Json(new APIResponse { Error = true, ResponseObject = "Service Type Id not specified" }); }
                int serviceTypeIdParsed = 0;
                if(int.TryParse(serviceTypeId, out serviceTypeIdParsed))
                {
                    return Json(_adminUserHandler.GetFlowDefinitionForServiceType(serviceTypeIdParsed));
                }
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }


        public JsonResult GetApprovalFlowDefinitionLevelsForDefinition(string definitionId)
        {
            try
            {
                if (string.IsNullOrEmpty(definitionId)) { return Json(new APIResponse { Error = true, ResponseObject = "Flow Definition Id not specified" }); }
                int definitionIdParsed = 0;
                if (int.TryParse(definitionId, out definitionIdParsed))
                {
                    return Json(_adminUserHandler.GetApprovalFlowDefinitionLevelsForDefinitionWithId(definitionIdParsed));
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
            }
            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
        }

        public JsonResult GetPersonnelWithServiceNumber(string serviceNumber)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceNumber) || serviceNumber.Trim().Length == 0) { throw new Exception("Service number not specified."); }
                return Json(_adminUserHandler.GetPoliceOfficerDetails(serviceNumber.Trim()), JsonRequestBehavior.AllowGet);

            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}