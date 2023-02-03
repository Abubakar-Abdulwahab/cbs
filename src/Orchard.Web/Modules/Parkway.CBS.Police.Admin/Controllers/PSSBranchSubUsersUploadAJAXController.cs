using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSBranchSubUsersUploadAJAXController : Controller
    {
        private readonly IPSSBranchSubUsersUploadHandler _handler;

        public PSSBranchSubUsersUploadAJAXController(IPSSBranchSubUsersUploadHandler handler)
        {
            _handler = handler;
        }

        public JsonResult CheckIfBranchSubUsersUploadCompleted(string batchToken)
        {
            return Json(_handler.CheckIfBatchUploadValidationCompleted(batchToken));
        }
    }
}