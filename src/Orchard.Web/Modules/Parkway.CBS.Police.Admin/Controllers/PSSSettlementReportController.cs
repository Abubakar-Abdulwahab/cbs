using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.VM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Parkway.CBS.Police.Admin.Controllers
{
    [Admin]
    public class PSSSettlementReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSSettlementReportHandler _handler;
        public ILogger Logger { get; set; }
        dynamic Shape { get; set; }

        public PSSSettlementReportController(IOrchardServices orchardServices, IPSSSettlementReportHandler handler, IShapeFactory shapeFactory)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            Shape = shapeFactory;
        }


        public ActionResult ReportSummary(PSSSettlementReportSummaryVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportSummary);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInput.From) && !string.IsNullOrEmpty(userInput.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $"Unable to parse date range. Exception Message ---- {exception.Message}");
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSSettlementReportSearchParams searchParams = new PSSSettlementReportSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = take,
                    Skip = skip,
                    PageData = true
                };

                PSSSettlementReportSummaryVM vm = _handler.GetVMForReportSummary(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalReportRecords);
                pageShape.RouteData(DoRouteDataForSettlementReportSummary(vm));
                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        public ActionResult ReportInvoices(string batchRef, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportSummary);
                if (string.IsNullOrEmpty(batchRef)) { throw new Exception("PSS Settlement Batch Ref not specified"); }
                batchRef = batchRef.Trim();
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);
                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSSettlementReportSearchParams searchParams = new PSSSettlementReportSearchParams
                {
                    Take = take,
                    Skip = skip,
                    BatchId = _handler.GetSettlementBatchId(batchRef),
                    PageData = true
                };

                PSSSettlementReportInvoicesVM vm = _handler.GetVMForReportInvoices(searchParams);
                vm.SettlementBatchRef = batchRef;
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalReportRecords);
                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        public ActionResult ReportParties(string batchRef)
        {
            return View(new PSSSettlementReportPartiesVM { });
        }


        public ActionResult ReportAggregate(PSSSettlementReportAggregateVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportSummary);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInput.From) && !string.IsNullOrEmpty(userInput.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $"Unable to parse date range. Exception Message ---- {exception.Message}");
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSSettlementReportAggregateSearchParams searchParams = new PSSSettlementReportAggregateSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = take,
                    Skip = skip,
                    PageData = true,
                    SettlementId = userInput.SelectedSettlement,
                    Status = (int)Core.Models.Enums.PSSSettlementBatchStatus.TransactionsMarkedAsSettled
                };

                PSSSettlementReportAggregateVM vm = _handler.GetVMForReportAggregate(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalRecordCount);
                pageShape.RouteData(DoRouteDataForSettlementReportAggregate(vm));
                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        public ActionResult ReportParty(string batchRef, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportParties);
                if(string.IsNullOrEmpty(batchRef) || string.IsNullOrWhiteSpace(batchRef.Trim()))
                {
                    Logger.Error("Settlement Batch Ref not specified");
                    throw new Exception();
                }
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSSettlementReportPartySearchParams searchParams = new PSSSettlementReportPartySearchParams
                {
                    BatchRef = batchRef.Trim(),
                    Take = take,
                    Skip = skip,
                    PageData = true
                };

                PSSSettlementReportPartyVM vm = _handler.GetVMForReportParty(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalRecordCount);
                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        public ActionResult ReportPartyBreakdown(string batchRef, int feePartyBatchAggregateId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportPartyBreakdown);
                if (string.IsNullOrEmpty(batchRef) || string.IsNullOrWhiteSpace(batchRef.Trim()))
                {
                    Logger.Error("Settlement Batch Ref not specified");
                    throw new Exception();
                }

                if(feePartyBatchAggregateId == 0)
                {
                    Logger.Error("Fee Party Id not specified");
                    throw new Exception();
                }

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSSettlementReportPartyBreakdownSearchParams searchParams = new PSSSettlementReportPartyBreakdownSearchParams
                {
                    FeePartyBatchAggregateId = feePartyBatchAggregateId,
                    BatchRef = batchRef.Trim(),
                    Take = take,
                    Skip = skip,
                    PageData = true
                };

                PSSSettlementReportPartiesBreakdownVM vm = _handler.GetVMForReportPartyBreakdown(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalRecordCount);
                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        public ActionResult ReportBatchBreakdown(string batchRef, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportBatchBreakdown);
                if (string.IsNullOrEmpty(batchRef) || string.IsNullOrWhiteSpace(batchRef.Trim()))
                {
                    Logger.Error("Settlement Batch Ref not specified");
                    throw new Exception();
                }

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSSettlementReportBatchBreakdownSearchParams searchParams = new PSSSettlementReportBatchBreakdownSearchParams
                {
                    BatchRef = batchRef.Trim(),
                    Take = take,
                    Skip = skip,
                    PageData = true
                };

                PSSSettlementReportBatchBreakdownVM vm = _handler.GetVMForReportBatchBreakdown(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalRecordCount);
                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        public ActionResult ReportBreakdown(PSSSettlementReportBreakdownVM userInput, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportPartyBreakdown);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInput.StartDate) && !string.IsNullOrEmpty(userInput.EndDate))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(exception, $"Unable to parse date range. Exception Message ---- {exception.Message}");
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                PSSSettlementReportBreakdownSearchParams searchParams = new PSSSettlementReportBreakdownSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = take,
                    Skip = skip,
                    FileNumber = userInput.FileNumber,
                    InvoiceNumber = userInput.InvoiceNumber,
                    SelectedState = userInput.SelectedState,
                    SelectedLGA = userInput.SelectedLGA,
                    SelectedSettlementParty = userInput.SelectedSettlementParty,
                    SelectedService = userInput.SelectedService,
                    SelectedCommand = userInput.SelectedCommand,
                    PageData = true
                };

                PSSSettlementReportBreakdownVM vm = _handler.GetSettlementReportBreakdownVM(searchParams);
                var pageShape = Shape.Pager(pager).TotalItemCount(vm.TotalRecordCount);
                pageShape.RouteData(DoRouteDataForSettlementReportBreakdown(vm));
                vm.Pager = pageShape;

                return View(vm);
            }
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        /// <summary>
        /// Pop route data with available data parameters to help pager keep track of search params
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RouteData</returns>
        private RouteData DoRouteDataForSettlementReportSummary(PSSSettlementReportSummaryVM model)
        {
            RouteData routeData = new RouteData();
            routeData.Values.Add("From", model.From);
            routeData.Values.Add("End", model.End);
            return routeData;
        }


        /// <summary>
        /// Pop route data with available data parameters to help pager keep track of search params
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RouteData</returns>
        private RouteData DoRouteDataForSettlementReportAggregate(PSSSettlementReportAggregateVM model)
        {
            RouteData routeData = new RouteData();
            if (model.SelectedSettlement != 0) { routeData.Values.Add("SelectedSettlement", model.SelectedSettlement); }
            routeData.Values.Add("From", model.From);
            routeData.Values.Add("End", model.End);
            return routeData;
        }


        /// <summary>
        /// Pop route data with available data parameters to help pager keep track of search params
        /// </summary>
        /// <param name="model"></param>
        /// <returns>RouteData</returns>
        private RouteData DoRouteDataForSettlementReportBreakdown(PSSSettlementReportBreakdownVM model)
        {
            RouteData routeData = new RouteData();
            if (!string.IsNullOrEmpty(model.InvoiceNumber)) { routeData.Values.Add("InvoiceNumber", model.InvoiceNumber); }
            if (!string.IsNullOrEmpty(model.FileNumber)) { routeData.Values.Add("FileNumber", model.FileNumber); }
            if (model.SelectedState != 0) { routeData.Values.Add("SelectedState", model.SelectedState); }
            if (model.SelectedLGA != 0) { routeData.Values.Add("SelectedLGA", model.SelectedLGA); }
            if (model.SelectedSettlementParty != 0) { routeData.Values.Add("SelectedSettlementParty", model.SelectedSettlementParty); }
            if (model.SelectedService != 0) { routeData.Values.Add("SelectedService", model.SelectedService); }
            if (model.SelectedCommand != 0) { routeData.Values.Add("SelectedCommand", model.SelectedCommand); }
            routeData.Values.Add("StartDate", model.StartDate);
            routeData.Values.Add("EndDate", model.EndDate);
            return routeData;
        }
    }
}