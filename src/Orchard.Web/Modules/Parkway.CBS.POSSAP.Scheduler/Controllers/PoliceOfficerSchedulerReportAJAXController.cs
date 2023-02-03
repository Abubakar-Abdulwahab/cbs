using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.ExternalSourceData.HRSystem;
using Parkway.CBS.POSSAP.Scheduler.Controllers.Handlers.Contracts;
using System;
using System.Web.Mvc;

namespace Parkway.CBS.POSSAP.Scheduler.Controllers
{
    [Admin]
    public class PoliceOfficerSchedulerReportAJAXController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IPoliceOfficerSchedulerReportAJAXHandler _handler;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        public PoliceOfficerSchedulerReportAJAXController(IOrchardServices orchardServices, IPoliceOfficerSchedulerReportAJAXHandler handler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        /// <summary>
        /// this method starts the process of getting the data we need to display
        /// We first get the request identifier, which identifies the search params
        /// for this request
        /// </summary>
        public JsonResult GetData(string searchParametersToken)
        {
            try
            {
                //logic that does something before returning request identifier
                return Json(new APIResponse { ResponseObject = _handler.GetRequestIdentifier(searchParametersToken) }, JsonRequestBehavior.AllowGet);
            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Check to see if the search params constraints (page offset) has been
        /// cached
        /// </summary>
        /// <param name="searchParams"></param>
        /// <param name="requestId"></param>
        public JsonResult CheckConstraints(string searchParams, string requestIdentifier)
        {
            try
            {
                //logic that does something before returning value indicating if the specified constraints exists
                return Json(_handler.CheckSearchParamsConstraints(searchParams, requestIdentifier), JsonRequestBehavior.AllowGet);
            }
            catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetReportData(string page)
        {
            try
            {
                int pageNumber = 0;
                if(!int.TryParse(page, out pageNumber))
                {
                    throw new Exception("Unable to parse page number.");
                }

                //logic that does something before returning report data
                return Json(new APIResponse { ResponseObject = _handler.GetOfficers(pageNumber) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetExternalReportData()
        {
            try
            {
                //logic that does something before returning value that indicates if external report data has been retrieved
                return Json(new APIResponse { ResponseObject = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetPager(string page, string totalOfficers)
        {
            try
            {
                int pageNumber = 0;
                int totalOfficersNumber = 0;
                if(int.TryParse(page, out pageNumber) && int.TryParse(totalOfficers, out totalOfficersNumber))
                {
                    var pager = new Pager(_orchardServices.WorkContext.CurrentSite, new PagerParameters { Page = pageNumber, PageSize = 10 });
                    var pageShape = Shape.Pager(pager).TotalItemCount(totalOfficersNumber);
                    string renderedView = Core.Utilities.Util.RenderViewToString(ControllerContext, "Pager", pageShape, true);
                    return Json(new APIResponse { ResponseObject = renderedView }, JsonRequestBehavior.AllowGet);
                }
                else { throw new Exception("Unable to parse page size and page number"); }

            }catch(Exception exception)
            {
                Logger.Error(exception, exception.Message);
                return Json(new APIResponse { Error = true, ResponseObject = ErrorLang.genericexception().Text }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}