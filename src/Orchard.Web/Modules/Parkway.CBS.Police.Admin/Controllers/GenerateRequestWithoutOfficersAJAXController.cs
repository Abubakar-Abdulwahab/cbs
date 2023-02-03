using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class GenerateRequestWithoutOfficersAJAXController : Controller
    {
        private readonly IGenerateRequestWithoutOfficersHandler _handler;

        public GenerateRequestWithoutOfficersAJAXController(IGenerateRequestWithoutOfficersHandler handler)
        {
            _handler = handler;
        }

        public JsonResult CheckIfBatchUploadCompleted(string batchToken)
        {
            return Json(_handler.CheckIfBatchUploadValidationCompleted(batchToken));
        }
    }
}