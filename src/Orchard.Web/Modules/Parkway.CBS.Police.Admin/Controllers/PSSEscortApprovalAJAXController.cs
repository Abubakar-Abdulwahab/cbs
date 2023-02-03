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
    public class PSSEscortApprovalAJAXController : Controller
    {
        private readonly IPSSEscortApprovalHandler _handler;
        public ILogger Logger { get; set; }
        public PSSEscortApprovalAJAXController(IPSSEscortApprovalHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }



        public JsonResult GetPersonnelWithServiceNumber(string serviceNumber)
        {
            try
            {
                if(string.IsNullOrEmpty(serviceNumber) || serviceNumber.Trim().Length == 0) { throw new Exception("Service number not specified."); }
                return Json(_handler.GetPoliceOfficer(serviceNumber.Trim()), JsonRequestBehavior.AllowGet);

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetAllocatedOfficers(long requestId, int commandId)
        {
            try
            {
                if (requestId == 0 || commandId == 0) { return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }); }
                return Json(_handler.GetProposedEscortOfficers(requestId, commandId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
            }
        }


        public JsonResult GetNumberOfOfficersRequested(long formationAllocationId, long allocationGroupId)
        {
            try
            {
                if (formationAllocationId == 0 || allocationGroupId == 0) { return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }); }
                return Json(_handler.GetNumberOfOfficersRequestedFromFormation(formationAllocationId, allocationGroupId));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text });
            }
        }
    }
}