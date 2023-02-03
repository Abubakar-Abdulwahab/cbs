using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class BranchOfficerAJAXController : Controller
    {
        private readonly IBranchOfficerHandler _branchOfficerHandler;

        public BranchOfficerAJAXController(IBranchOfficerHandler branchOfficerHandler)
        {
            _branchOfficerHandler = branchOfficerHandler;
        }


        public JsonResult CheckIfBranchOfficerUploadCompleted(string batchToken)
        {
            return Json(_branchOfficerHandler.CheckIfBatchUploadValidationCompleted(batchToken));
        }


        public JsonResult GetServiceCategoryTypes(string serviceCategoryId)
        {
            return Json(_branchOfficerHandler.GetCategoryTypesForServiceCategoryWithId(serviceCategoryId));
            
        }


        public JsonResult GetTacticalSquads(string commandTypeId)
        {
            return Json(_branchOfficerHandler.GetCommandsForCommandTypeWithId(commandTypeId));
        }


        public JsonResult GetNextLevelCommandsWithCode(string code)
        {
            return Json(_branchOfficerHandler.GetNextLevelCommandsWithCode(code));
        }


        public JsonResult GetStateFormations(string stateId, string lgaId)
        {
            return Json(_branchOfficerHandler.GetAreaAndDivisionalCommandsByStateAndLGA(stateId, lgaId));
        }
    }
}