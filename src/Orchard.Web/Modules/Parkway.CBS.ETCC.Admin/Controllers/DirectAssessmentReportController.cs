using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.ETCC.Admin.Controllers
{
    [Admin]
    public class DirectAssessmentReportController : Controller
    {
        public ITCCReportHandler _handler;
        private readonly IDirectAssessmentReportHandler _directAssessmentReportHandler;
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;

        public ILogger Logger { get; set; }
        private dynamic Shape { get; set; }

        public DirectAssessmentReportController(IOrchardServices orchardServices, IShapeFactory shapeFactory, ITCCReportHandler handler, IDirectAssessmentReportHandler directAssessmentReportHandler)
        {
            _directAssessmentReportHandler = directAssessmentReportHandler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
            _orchardServices = orchardServices;
            _handler = handler;
            _notifier = orchardServices.Notifier;
        }

        // GET: DirectAssessmentRequestReport
        public ActionResult DirectAssessmentRequestReport(PAYEDirectAssessmentReportVM userInputModel, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.ViewDirectAssessmentReport);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInputModel.From) && !string.IsNullOrEmpty(userInputModel.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInputModel.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInputModel.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize ?? 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                userInputModel.From = startDate.ToString("dd'/'MM'/'yyyy");
                userInputModel.End = endDate.ToString("dd'/'MM'/'yyyy");

                int.TryParse(userInputModel.DirectAssessmentType, out int directAssessmentType);

                var model = _directAssessmentReportHandler.GetDirectAssessmentReport(new DirectAssessmentSearchParams
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    TIN = userInputModel.TIN,
                    InvoiceNumber = userInputModel.InvoiceNo,
                    DirectAssessmentType = directAssessmentType,
                    InvoiceStatus = userInputModel.InvoiceStatus,
                    Skip = skip,
                    Take = take
                });

                var pageShape = Shape.Pager(pager).TotalItemCount(model.DataSize);
                pageShape.RouteData(DoRouteDataForRequestsReport(model));

                model.Pager = pageShape;

                return View(model);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        /// <summary>
        /// Pop route data with available data parameters to help pager keep track of search params
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RouteData</returns>
        private RouteData DoRouteDataForRequestsReport(PAYEDirectAssessmentReportVM userInputModel)
        {
            int.TryParse(userInputModel.DirectAssessmentType, out int directAssessmentType);

            RouteData routeData = new RouteData();

            if (!string.IsNullOrEmpty(userInputModel.TIN)) { routeData.Values.Add("TIN", userInputModel.TIN); }
            if (!string.IsNullOrEmpty(userInputModel.InvoiceNo)) { routeData.Values.Add("invoiceNumber", userInputModel.InvoiceNo); }
            if (userInputModel.InvoiceStatus != InvoiceStatus.All) { routeData.Values.Add("InvoiceStatus", userInputModel.InvoiceStatus); }
            if (directAssessmentType != 0) { routeData.Values.Add("DirectAssessmentType", directAssessmentType); }

            routeData.Values.Add("from", userInputModel.From);
            routeData.Values.Add("end", userInputModel.End);
            return routeData;
        }
    }
}