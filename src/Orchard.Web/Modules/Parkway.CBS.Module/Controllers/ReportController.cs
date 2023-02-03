using Orchard;
using Orchard.DisplayManagement;
using Orchard.Logging;
using Orchard.Localization;
using Orchard.UI.Admin;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Parkway.CBS.Core.Lang;
using Orchard.Security;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;
using Parkway.DataExporter.Implementations.Util;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using System.Threading.Tasks;

namespace Parkway.CBS.Module.Controllers
{
    [Admin]
    public class ReportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        public IReportHandler _handler;
        private Localizer T { get; }
        private ITaxPayerReportFilter _taxPayerReportFilter;        


        public ReportController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IReportHandler handler, ITaxPayerReportFilter taxPayerReportFilter)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
            _handler = handler;
            T = NullLocalizer.Instance;
            _taxPayerReportFilter = taxPayerReportFilter;
        }


        public int GetWeekNumber()
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }


        [HttpPost]
        public JsonResult MDARevenueHeads(string mdaSlug)
        {
            return Json(_handler.GetRevenueHeads(mdaSlug), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Assessment report
        /// <para>47th iteration.</para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pagerParameters"></param>
        /// <param name="options"></param>
        /// <param name="From"></param>
        /// <param name="End"></param>
        /// <param name="mdaSelected"></param>
        /// <param name="revenueHeadSelected"></param>
        /// <returns></returns>
        public ActionResult AssessmentReport(MDAReportViewModel model, PagerParameters pagerParameters, string from, string end, string mda, string revenueheadId, string sector, InvoiceStatus? paymentStatus, string invoiceNumber, string TIN)
        {
            try
            {
                _handler.CheckForPermission(Permissions.ViewInvoiceReport);
                PaymentOptions options = new PaymentOptions { PaymentStatus = paymentStatus ?? InvoiceStatus.All };

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(end))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                //populate an object with the search parameters
                var searchData = new InvoiceAssessmentSearchParams
                {
                    EndDate = endDate,
                    SMDA = mda,
                    Options = options,
                    SRevenueHeadId = revenueheadId,
                    SCategory = sector,
                    Take = take,
                    Skip = skip,
                    StartDate = startDate,
                    InvoiceNumber = invoiceNumber,
                    TIN = TIN,
                    DateFilterBy = model.DateFilterBy
                };

                model = _handler.GetInvoiceAssessmentReport(searchData);

                var pageShape = Shape.Pager(pager).TotalItemCount(model.TotalNumberOfInvoicesSent);
                model.FromRange = startDate.ToString("dd'/'MM'/'yyyy");
                model.EndRange = endDate.ToString("dd'/'MM'/'yyyy");

                #region routedata
                RouteData routeData = new RouteData();
                if (!string.IsNullOrEmpty(sector) && sector != "0") { routeData.Values.Add("sector", sector); }
                if (!string.IsNullOrEmpty(TIN)) { routeData.Values.Add("TIN", TIN); }
                if (!string.IsNullOrEmpty(invoiceNumber)) { routeData.Values.Add("invoiceNumber", invoiceNumber); }
                if (!string.IsNullOrEmpty(mda) && mda != "0") { routeData.Values.Add("mda", mda); }
                if (!string.IsNullOrEmpty(revenueheadId) && revenueheadId != "0") { routeData.Values.Add("revenueheadId", revenueheadId); }
                if (options.PaymentStatus != InvoiceStatus.All) { routeData.Values.Add("paymentStatus", options.PaymentStatus); }
                if (searchData.DateFilterBy != FilterDate.InvoiceDate) { routeData.Values.Add("DateFilterBy", searchData.DateFilterBy); }
                routeData.Values.Add("from", model.FromRange);
                routeData.Values.Add("end", model.EndRange);
                #endregion

                pageShape.RouteData(routeData);
                model.Pager = pageShape;
                model.Options = options;

                model.SelectedMDA = mda;
                model.SelectedRevenueHead = revenueheadId;
                model.SectorSelected = sector;
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, Lang.usernotauthorized_ex_text);
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, Lang.genericexception_text);
                return RedirectToAction("List", "MDA", new { });
            }
            #endregion

            return View(model);
        }


        /// <summary>
        /// Collections report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pagerParameters"></param>
        /// <param name="fromRange"></param>
        /// <param name="endRange"></param>
        /// <param name="taxpayerName"></param>
        /// <param name="paymentRef"></param>
        /// <param name="mda"></param>
        /// <param name="revenueHead"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="receiptNumber"></param>
        /// <param name="selectedRevenueHead"></param>
        /// <param name="selectedPaymentChannel"></param>
        /// <returns></returns>
        public ActionResult CollectionReport(CollectionReportViewModel model, PagerParameters pagerParameters, string fromRange, string endRange, string paymentRef, string mda, string revenueheadId, string invoiceNumber, string receiptNumber, string selectedRevenueHead, PaymentChannel? selectedPaymentChannel, CollectionPaymentDirection? paymentDirection, string selectedPaymentProvider)
        {
            try
            {
                _handler.CheckForPermission(Permissions.ViewCollectionReport);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(fromRange) && !string.IsNullOrEmpty(endRange))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(fromRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(endRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (Exception) { throw; }
                }
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                CollectionSearchParams searchData = new CollectionSearchParams
                {
                    FromRange = startDate,
                    EndRange = endDate,
                    InvoiceNumber = invoiceNumber,
                    PaymentRef = paymentRef,
                    SRevenueHeadId = revenueheadId,
                    SelectedPaymentProvider = selectedPaymentProvider,
                    SelectedPaymentChannel = selectedPaymentChannel ?? PaymentChannel.None,
                    ReceiptNumber = receiptNumber,
                    SelectedMDA = mda,
                    SelectedBankCode = model.SelectedBank,
                    PaymentDirection = paymentDirection ?? CollectionPaymentDirection.PaymentDate,
                    Take = take,
                    Skip = skip,
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id
                };

                CollectionReportViewModel viewModel = _handler.GetReportForCollection(searchData);

                var pageShape = Shape.Pager(pager).TotalItemCount(viewModel.TotalNumberOfPayment);
                viewModel.FromRange = startDate.ToString("dd'/'MM'/'yyyy");
                viewModel.EndRange = endDate.ToString("dd'/'MM'/'yyyy");


                #region routedata
                RouteData routeData = new RouteData();
                if (!string.IsNullOrEmpty(paymentRef)) { routeData.Values.Add("paymentRef", paymentRef); }
                if (!string.IsNullOrEmpty(invoiceNumber)) { routeData.Values.Add("invoiceNumber", invoiceNumber); }
                if (!string.IsNullOrEmpty(receiptNumber)) { routeData.Values.Add("receiptNumber", receiptNumber); }
                if (!string.IsNullOrEmpty(mda)) { routeData.Values.Add("mda", mda); }
                if (!string.IsNullOrEmpty(model.SelectedBank) && model.SelectedBank != "#All") { routeData.Values.Add("SelectedBank", model.SelectedBank); }
                if (!string.IsNullOrEmpty(revenueheadId)) { routeData.Values.Add("revenueheadId", revenueheadId); }

                if (selectedPaymentChannel != null && selectedPaymentChannel != PaymentChannel.None) { routeData.Values.Add("selectedPaymentChannel", selectedPaymentChannel); }

                if (!string.IsNullOrEmpty(selectedPaymentProvider)) { routeData.Values.Add("selectedPaymentProvider", selectedPaymentProvider); }

                if (paymentDirection != null) { routeData.Values.Add("paymentDirection", paymentDirection); }
                routeData.Values.Add("fromRange", viewModel.FromRange);
                routeData.Values.Add("endRange", viewModel.EndRange);
                #endregion

                pageShape.RouteData(routeData);

                viewModel.Pager = pageShape;

                viewModel.SelectedMDA = mda;
                viewModel.SelectedRevenueHead = revenueheadId ?? selectedRevenueHead;
                viewModel.PaymentDirection = paymentDirection ?? CollectionPaymentDirection.PaymentDate;
                viewModel.SelectedPaymentProvider = selectedPaymentProvider;

                return View(viewModel);
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to CollectionReport without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            #endregion
        }


        public ActionResult TaxProfilesReport(TaxProfilesReportVM model, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.ViewTaxPayersReport);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;
                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                TaxProfilesReportVM viewModel = _handler.GetTaxProfilesReport(model, skip, take);

                var pageShape = Shape.Pager(pager).TotalItemCount(viewModel.TotalNumberOfTaxPayers);

                #region routedata
                RouteData routeData = new RouteData();
                if (!string.IsNullOrEmpty(viewModel.PayerId)) { routeData.Values.Add("PayerId", viewModel.PayerId); }
                if (!string.IsNullOrEmpty(viewModel.TIN)) { routeData.Values.Add("TIN", viewModel.TIN); }
                if (!string.IsNullOrEmpty(viewModel.PhoneNumber)) { routeData.Values.Add("PhoneNumber", viewModel.PhoneNumber); }
                if (!string.IsNullOrEmpty(viewModel.Name)) { routeData.Values.Add("Name", viewModel.Name); }
                if (model.TaxCategory > 0) { routeData.Values.Add("TaxCategory", viewModel.TaxCategory); }
                #endregion

                pageShape.RouteData(routeData);

                viewModel.Pager = pageShape;
                //viewModel.TaxCategories = _handler.GetTaxCategories();
                return View(viewModel);
            }
            #region Catch clauses 
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToAction("TaxPayersReport", "Report", new { });
            }
            #endregion
        }

        public ActionResult StatementOfAccountDetails(TaxPayerDetailsViewModel model, string FromRange, string EndRange, string payerId, PagerParameters pagerParameters)
        {
            try
            {
                _handler.CheckForPermission(Permissions.ViewAccountStatement);
                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);
                int take = pagerParameters.PageSize.HasValue ? pagerParameters.PageSize.Value : 10;

                if (take == 0) { take = 10; pager.PageSize = 10; }
                int skip = pagerParameters.Page.HasValue ? (pagerParameters.Page.Value - 1) * take : 0;

                DateTime startDate = new DateTime();
                DateTime endDate = new DateTime();

                if (!string.IsNullOrEmpty(FromRange) && !string.IsNullOrEmpty(EndRange))
                {
                    model.FromRange = FromRange;
                    model.EndRange = EndRange;
                }

                try
                {
                    startDate = DateTime.ParseExact(model.FromRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(model.EndRange, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date.AddDays(1).AddMilliseconds(-1);
                }
                catch (Exception)
                {
                    startDate = DateTime.Now.AddMonths(-1);
                    endDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);
                    model.FromRange = startDate.ToString("dd/MM/yyyy");
                    model.EndRange = endDate.ToString("dd/MM/yyyy");
                }

                TaxPayerDetailsViewModel vm = _handler.GetTaxPayer(payerId);

                var accountStatements = _taxPayerReportFilter.GetReportForStatementOfAccount(vm.Id, startDate, endDate, model.PaymentTypeId);
                var pageShape = Shape.Pager(pager).TotalItemCount(accountStatements.Report.Count());
                vm.Report = accountStatements.Report.Skip(skip).Take(take);

                #region routedata
                RouteData routeData = new RouteData();
                routeData.Values.Add("FromRange", model.FromRange);
                routeData.Values.Add("EndRange", model.EndRange);
                #endregion

                pageShape.RouteData(routeData);
                vm.Pager = pageShape;
                vm.TotalCreditAmount = accountStatements.TotalCreditAmount;
                vm.TotalBillAmount = accountStatements.TotalBillAmount;
                vm.FromRange = model.FromRange;
                vm.EndRange = model.EndRange;

                return View(vm);
            }
            catch (NoRecordFoundException exception)
            {
                //do work here
                Logger.Error(exception, string.Format("No tax payer 404 in StatementOfAccount payer id {0}. Exception {1}", payerId, exception.ToString()));
                _notifier.Add(NotifyType.Error, ErrorLang.notaxpayerrecord404(payerId));
                return RedirectToAction("TaxPayersReport", "Report", new { });
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error occurred {0}", ex));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }

        /// <summary>
        /// Edit Tax Payer details
        /// </summary>
        /// <param name="payerId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult EditTaxPayer(string payerId)
        {
            try
            {
                //Check if user has permission to access this module
                _handler.CheckForPermission(Permissions.EditTaxPayer);

                //Get Tax Payer details using the Payer Id
                TaxPayerDetailsViewModel vm = _handler.GetTaxPayer(payerId);

                return View(vm);
            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error occurred {0}", ex));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditTaxPayer(TaxPayerDetailsViewModel model)
        {
            try
            {
                //Check if user has permission to access this module
                _handler.CheckForPermission(Permissions.EditTaxPayer);

                //Check if the Tax Payer Code already exist
                var checkPayerCode = _handler.CheckIfTaxPayerCodeExist(model.TaxPayerCode, model.PayerId);

                if (checkPayerCode)
                {
                    _notifier.Add(NotifyType.Error, Lang.taxpayercodealreadyexist);
                    return View(model);
                }

                bool response = _handler.UpdateTaxPayer(model);

                if (response)
                {
                    _notifier.Add(NotifyType.Information, Lang.savesuccessfully);
                    return RedirectToAction("TaxProfilesReport", "Report", null);
                }
                _notifier.Add(NotifyType.Error, Lang.genericexception_text);
                return Redirect("~/Admin");

            }
            catch (Exception ex)
            {
                Logger.Error(string.Format("Error occurred {0}", ex));
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
        }


        /// <summary>
        /// This is use to handle export to excel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ActionResult> ExcelTaxProfilesReport(TaxProfilesReportVM model)
        {
            try
            {

                TaxProfilesReportVM viewModel = _handler.GetTaxProfilesReport(model, 0, 0);


                #region routedata
                RouteData routeData = new RouteData();
                if (!string.IsNullOrEmpty(viewModel.PayerId)) { routeData.Values.Add("PayerId", viewModel.PayerId); }
                if (!string.IsNullOrEmpty(viewModel.TIN)) { routeData.Values.Add("TIN", viewModel.TIN); }
                if (!string.IsNullOrEmpty(viewModel.PhoneNumber)) { routeData.Values.Add("PhoneNumber", viewModel.PhoneNumber); }
                if (!string.IsNullOrEmpty(viewModel.Name)) { routeData.Values.Add("Name", viewModel.Name); }
                if (model.TaxCategory > 0) { routeData.Values.Add("TaxCategory", viewModel.TaxCategory); }
                #endregion

                string site = _orchardServices.WorkContext.CurrentSite.SiteName;
                string tenantIdentifier = site.Replace(" ", "");
                string fileName = "TaxpayerReport_" + DateTime.Now.Ticks + ".xlsx";
                var headers = TemplateUtil.KeyValueTemplateFor("TaxPayerReport", tenantIdentifier);


                IExcelExportEngine _excel = new ExcelEngine(headers);
                var excelData = await _excel.DownloadAsExcelAsync(headers, viewModel.ReportRecords, fileName, "", "");
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                System.Web.HttpContext.Current.Response.OutputStream.Write(excelData, 0, excelData.Length);
                System.Web.HttpContext.Current.Response.End();

            }
            #region Catch clauses 
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToAction("TaxPayersReport", "Report", new { });
            }
            #endregion

            return null;

        }

        /// <summary>
        /// This is use to handle export to PDF
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult PdfTaxProfilesReport(TaxProfilesReportVM model)
        {
            try
            {

                TaxProfilesReportVM viewModel = _handler.GetTaxProfilesReport(model, 0, 0);


                #region routedata
                RouteData routeData = new RouteData();
                if (!string.IsNullOrEmpty(viewModel.PayerId)) { routeData.Values.Add("PayerId", viewModel.PayerId); }
                if (!string.IsNullOrEmpty(viewModel.TIN)) { routeData.Values.Add("TIN", viewModel.TIN); }
                if (!string.IsNullOrEmpty(viewModel.PhoneNumber)) { routeData.Values.Add("PhoneNumber", viewModel.PhoneNumber); }
                if (!string.IsNullOrEmpty(viewModel.Name)) { routeData.Values.Add("Name", viewModel.Name); }
                if (model.TaxCategory > 0) { routeData.Values.Add("TaxCategory", viewModel.TaxCategory); }
                #endregion

                string site = _orchardServices.WorkContext.CurrentSite.SiteName;
                string tenantIdentifier = site.Replace(" ", "");
                string fileName = "TaxpayerReport_" + DateTime.Now.Ticks + ".pdf";
                var template = TemplateUtil.RazorTemplateFor("TaxPayerReport", tenantIdentifier);

                IPDFExportEngine _pdf = new PdfEngine();
                var pdfData = _pdf.DownloadAsPdf(null, template.File, viewModel, template.BasePath);
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                System.Web.HttpContext.Current.Response.OutputStream.Write(pdfData, 0, pdfData.Length);
                System.Web.HttpContext.Current.Response.End();

            }
            #region Catch clauses 
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToAction("TaxPayersReport", "Report", new { });
            }
            #endregion
            return null;

        }

    }
}