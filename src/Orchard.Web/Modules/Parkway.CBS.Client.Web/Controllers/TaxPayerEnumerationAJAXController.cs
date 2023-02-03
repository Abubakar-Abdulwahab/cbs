using Orchard.Logging;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    public class TaxPayerEnumerationAJAXController : Controller
    {
        private readonly ITaxPayerEnumerationHandler _handler;
        private readonly ICommonHandler _commonHandler;
        public ILogger Logger { get; set; }

        public TaxPayerEnumerationAJAXController(ITaxPayerEnumerationHandler handler, ICommonHandler commonHandler)
        {
            _handler = handler;
            _commonHandler = commonHandler;
            Logger = NullLogger.Instance;
        }

        public JsonResult CheckIfEnumerationUploadCompleted(string batchToken)
        {
            string errorMessage = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(batchToken)) { errorMessage = "Batch token not found"; throw new Exception(); }
                UserDetailsModel loggedInUser = _commonHandler.GetLoggedInUserDetails();
                if (loggedInUser == null || loggedInUser.Entity == null) { errorMessage = ErrorLang.usernotauthorized().Text; throw new AuthorizedUserNotFoundException(); }
                return Json(_handler.CheckIfEnumerationUploadIsCompleted(batchToken, loggedInUser.TaxPayerProfileVM.Id), JsonRequestBehavior.AllowGet);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = (string.IsNullOrEmpty(errorMessage)) ? ErrorLang.genericexception().Text : errorMessage;
            }
            return Json(new APIResponse { Error = true, ResponseObject = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetReportData(string batchToken)
        {
            string errorMessage = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(batchToken)) { errorMessage = "Batch token not found"; throw new Exception(); }
                UserDetailsModel loggedInUser = _commonHandler.GetLoggedInUserDetails();
                if (loggedInUser == null || loggedInUser.Entity == null) { errorMessage = ErrorLang.usernotauthorized().Text; throw new AuthorizedUserNotFoundException(); }
                return Json(_handler.GetLineItemsForEnumerationWithId(batchToken, loggedInUser.TaxPayerProfileVM.Id), JsonRequestBehavior.AllowGet);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = (string.IsNullOrEmpty(errorMessage)) ? ErrorLang.genericexception().Text : errorMessage;
            }
            return Json(new APIResponse { Error = true, ResponseObject = errorMessage }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEnumerationLineItemsMoveRight(string batchToken, int page)
        {
            string errorMessage = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(batchToken)) { errorMessage = "Batch token not found"; throw new Exception(); }
                UserDetailsModel loggedInUser = _commonHandler.GetLoggedInUserDetails();
                if (loggedInUser == null || loggedInUser.Entity == null) { errorMessage = ErrorLang.usernotauthorized().Text; throw new AuthorizedUserNotFoundException(); }
                return Json(_handler.GetPagedLineItemsForEnumerationWithId(batchToken, loggedInUser.TaxPayerProfileVM.Id, page), JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = (string.IsNullOrEmpty(errorMessage)) ? ErrorLang.genericexception().Text : errorMessage;
            }
            return Json(new APIResponse { Error = true, ResponseObject = errorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}