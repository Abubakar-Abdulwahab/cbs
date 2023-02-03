using Orchard;
using Orchard.Logging;
using Orchard.Themes;
using Parkway.CBS.Client.Web.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Module.Web.Middleware.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Client.Web.Controllers
{
    [Themed]
    [CBSCollectionAuthorized]
    public class TaxPayerEnumerationController : Controller
    {
        public ILogger Logger { get; set; }
        private readonly IOrchardServices _orchardServices;
        private readonly ICommonHandler _commonHandler;
        private readonly ITaxPayerEnumerationHandler _handler;

        public TaxPayerEnumerationController(IOrchardServices orchardServices, ICommonHandler commonHandler, ITaxPayerEnumerationHandler handler)
        {
            Logger = NullLogger.Instance;
            _orchardServices = orchardServices;
            _commonHandler = commonHandler;
            _handler = handler;
        }

        [HttpGet]
        public ActionResult UploadSchedule()
        {
            string errorMessage = string.Empty;
            try
            {
                if (TempData["Error"] != null) { errorMessage = TempData["Error"].ToString(); }
                TempData = null;
                UserDetailsModel user = _commonHandler.GetLoggedInUserDetails();
                HeaderObj headerObj = _commonHandler.HeaderFiller(user);
                if(headerObj.CategoryId == (int)Core.Models.Enums.TaxEntityCategoryEnum.Corporate)
                {
                    return View(new TaxPayerEnumerationVM { HeaderObj = headerObj, TaxEntity = user.TaxPayerProfileVM, StateLGAs = _handler.GetStatesAndLgas(), ErrorMessage = errorMessage });
                }
                else { throw new UserNotAuthorizedForThisActionException(); }
            }
            catch(UserNotAuthorizedForThisActionException)
            {
                TempData.Add("Error", ErrorLang.usernotauthorized().ToString());
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                TempData.Add("Error", ErrorLang.genericexception().ToString());
            }

            return RedirectToRoute(Module.Web.RouteName.Collection.GenerateInvoice);
        }


        [HttpGet]
        public ActionResult ShowScheduleResult(string batchToken)
        {
            string errorMessage = string.Empty;
            try
            {

                if (TempData["Error"] != null) { errorMessage = TempData["Error"].ToString(); }
                TempData = null;
                UserDetailsModel user = _commonHandler.GetLoggedInUserDetails();
                HeaderObj headerObj = _commonHandler.HeaderFiller(user);
                if (headerObj.CategoryId == (int)Core.Models.Enums.TaxEntityCategoryEnum.Corporate)
                {
                    return View(new TaxPayerEnumerationVM { HeaderObj = headerObj, TaxEntity = user.TaxPayerProfileVM, BatchToken = batchToken, ErrorMessage = errorMessage });
                }
                else { throw new UserNotAuthorizedForThisActionException(); }
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            TempData["Error"] = errorMessage;
            return RedirectToRoute(RouteName.TaxPayerEnumeration.UploadSchedule);
        }


        [HttpPost]
        public ActionResult OnScreen(ICollection<TaxPayerEnumerationLine> TaxPayerEnumerationLineItems)
        {
            TempData = null;
            string errorMessage = string.Empty;
            try
            {
                Logger.Information("Processing for enumeration schedule onscreen post");
                if (TaxPayerEnumerationLineItems == null || !TaxPayerEnumerationLineItems.Any()) { errorMessage = "submitted enumeration schedule has no entries."; throw new Exception(); }
                string batchToken = _handler.ProcessEnumerationItemsForOnScreenForm(TaxPayerEnumerationLineItems, _commonHandler.GetLoggedInUserDetails());
                return RedirectToRoute(RouteName.TaxPayerEnumeration.ShowScheduleResult, new { batchToken });
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            TempData["Error"] = errorMessage;
            return RedirectToRoute(RouteName.TaxPayerEnumeration.UploadSchedule);
        }


        [HttpPost]
        public ActionResult FileUpload()
        {
            TempData = null;
            string errorMessage = string.Empty;
            try
            {
                Logger.Information("Processing for enumeration schedule file upload post");
                if (HttpContext.Request.Files.Get("enumerationfile") == null || HttpContext.Request.Files.Get("enumerationfile").ContentLength == 0) { errorMessage = "submitted enumeration schedule file not found."; throw new Exception(); }
                if(Path.GetExtension(HttpContext.Request.Files.Get("enumerationfile").FileName) != ".xls" && Path.GetExtension(HttpContext.Request.Files.Get("enumerationfile").FileName) != ".xlsx")
                {
                    errorMessage = "Uploaded enumeration schedule file format not supported. Only .xls and .xlsx are supported.";
                    throw new Exception();
                }
                string batchToken = _handler.ProcessEnumerationItemsForFileUpload(HttpContext.Request.Files.Get("enumerationfile"), _commonHandler.GetLoggedInUserDetails());
                return RedirectToRoute(RouteName.TaxPayerEnumeration.ShowScheduleResult, new { batchToken });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                errorMessage = string.IsNullOrEmpty(errorMessage) ? ErrorLang.genericexception().ToString() : errorMessage;
            }
            TempData["Error"] = errorMessage;
            return RedirectToRoute(RouteName.TaxPayerEnumeration.UploadSchedule);
        }
    }
}