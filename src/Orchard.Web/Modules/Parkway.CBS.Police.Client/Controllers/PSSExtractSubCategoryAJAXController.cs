using System;
using System.Web.Mvc;
using Orchard.Logging;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Client.Middleware;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;

namespace Parkway.CBS.Police.Client.Controllers
{
    [CheckVerificationFilter(false)]
    public class PSSExtractSubCategoryAJAXController : Controller
    {
        private readonly IPSSExtractSubCategoryHandler _handler;

        public PSSExtractSubCategoryAJAXController(IPSSExtractSubCategoryHandler handler)
        {
            _handler = handler;
        }


        [HttpPost]
        /// <summary>
        /// Get the list of extract sub categories
        /// </summary>
        /// <param name="categoryId"></param>
        public JsonResult ExtractSubCategories(string categoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryId) || categoryId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid category." }); }
                int parsedCategoryVal = 0;
                if (!Int32.TryParse(categoryId, out parsedCategoryVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid category." });
                }
                return Json(_handler.GetSubCategories(parsedCategoryVal));
            }
            catch (Exception) { }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }

    }
}