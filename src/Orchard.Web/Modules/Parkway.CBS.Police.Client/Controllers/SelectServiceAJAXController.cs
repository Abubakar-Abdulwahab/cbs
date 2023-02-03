using Orchard.Logging;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Client.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Client.Controllers
{
    public class SelectServiceAJAXController : Controller
    {
        private readonly ISelectServiceHandler _handler;
        public ILogger Logger { get; set; }

        public SelectServiceAJAXController(ISelectServiceHandler handler)
        {
            _handler = handler;
            Logger = NullLogger.Instance;
        }


        [HttpPost]
        /// <summary>
        /// Get the list of tax entity sub categories
        /// </summary>
        /// <param name="categoryId"></param>
        public JsonResult PSSSubCategories(string categoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryId) || categoryId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid category." }); }
                int parsedCategoryVal = 0;
                if (!Int32.TryParse(categoryId, out parsedCategoryVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid Category." });
                }
                return Json(_handler.GetPSSSubCategories(parsedCategoryVal));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error in getting tax entity subcategories " + exception.Message);
            }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }


        [HttpPost]
        /// <summary>
        /// Get the list of sub categories for a tax entity sub category with the specified id
        /// </summary>
        /// <param name="subCategoryId"></param>
        public JsonResult PSSSubSubCategories(string subCategoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(subCategoryId) || subCategoryId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid sub category." }); }
                int parsedSubCategoryVal = 0;
                if (!Int32.TryParse(subCategoryId, out parsedSubCategoryVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid sub category." });
                }
                return Json(_handler.GetPSSSubSubCategories(parsedSubCategoryVal));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error in getting tax entity subsubcategories " + exception.Message);
            }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }


        [HttpPost]
        /// <summary>
        /// Get the list of services per category with the specified category Id
        /// </summary>
        /// <param name="categoryId"></param>
        public JsonResult GetServicesPerCategory(string categoryId)
        {
            try
            {
                if (string.IsNullOrEmpty(categoryId) || categoryId.Trim() == "0") { return Json(new APIResponse { Error = true, ResponseObject = "Select a valid category." }); }
                int parsedCategoryVal = 0;
                if (!Int32.TryParse(categoryId, out parsedCategoryVal))
                {
                    return Json(new APIResponse { Error = true, ResponseObject = "Select a valid category." });
                }
                return Json(_handler.GetServicePerCategory(parsedCategoryVal));
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error in getting services per category " + exception.Message);
            }

            return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().ToString() });
        }
    }
}