using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Module.Controllers.Handlers.Contracts;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Controllers
{
    public class FileExportController : Controller
    {

        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAuthenticationService _authenticationService;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        public IFileExportHandler _handler;
        private Localizer T { get; }
        int maximumReportMonths = 6;
        int oneMonth = 1;


        public FileExportController(IOrchardServices orchardServices, IAuthenticationService authenticationService, IShapeFactory shapeFactory, IFileExportHandler handler, ITaxPayerReportFilter taxPayerReportFilter)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Shape = shapeFactory;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            _handler = handler;
            T = NullLocalizer.Instance;
        }


        /// <summary>
        /// Export Assessment report to excel
        /// <para>43th iteration.</para>
        /// </summary>
        /// <param name="model"></param>
        /// <param name="options"></param>
        /// <param name="From"></param>
        /// <param name="End"></param>
        /// <param name="mdaSelected"></param>
        /// <param name="revenueHeadSelected"></param>
        /// <returns></returns>
        public async Task<ActionResult> AssessmentReportDownload(string from, string end, string mda, string revenueheadId, string sector, InvoiceStatus? paymentStatus, string invoiceNumber, string TIN, ExportFormat? format)
        {
            try
            {
                _handler.CheckForPermission(Permissions.ViewInvoiceReport);
                PaymentOptions options = new PaymentOptions { PaymentStatus = paymentStatus ?? InvoiceStatus.All };

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(end))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                        if (selectedMonth > maximumReportMonths)
                        {
                            _notifier.Add(NotifyType.Error, ErrorLang.maximumreportmonthsrexceeded());
                            return RedirectToAction("AssessmentReport", "Report", new { });
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                var searchData = new InvoiceAssessmentSearchParams
                {
                    EndDate = endDate,
                    SMDA = mda,
                    Options = options,
                    SRevenueHeadId = revenueheadId,
                    SCategory = sector,
                    Take = 0,
                    Skip = 0,
                    StartDate = startDate,
                    InvoiceNumber = invoiceNumber,
                    TIN = TIN,
                    DontPageData = true,
                };
                MDAReportViewModel viewModel = _handler.GetAssessmentReport(searchData);

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "Assessment_Report_from_" + startDate.ToString("dd/MM/yyyy") + "-" + endDate.ToString("dd/MM/yyyy") + ".xlsx";
                        await DoExcelAsync(viewModel.ReportRecords, fileName, SettingsFileNames.AssessmentReport.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "Assessment_Report_from_" + startDate.ToString("dd-MM-yyyy") + "_to_" + endDate.ToString("dd-MM-yyyy") + ".pdf";
                        StateConfig siteConfig = Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        //viewModel.ReportRecords = detailReports;
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor("AssessmentReport");

                        IPDFExportEngine _pdf = new PdfEngine();
                        var pdfData = _pdf.DownloadAsPdf(null, template.File, viewModel, template.BasePath);
                        System.Web.HttpContext.Current.Response.ClearHeaders();
                        System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                        System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + PDFfileName);
                        System.Web.HttpContext.Current.Response.OutputStream.Write(pdfData, 0, pdfData.Length);
                        System.Web.HttpContext.Current.Response.End();
                        //await DoPDFAsync(viewModel, PDFfileName, SettingsFileNames.AssessmentReport.ToString());
                        break;
                    default:
                        break;
                }
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
                return RedirectToAction("AssessmentReport", "Report", new { });
            }
            #endregion
            return null;
        }


        /// <summary>
        /// This is use to download excel copy of collection report
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fromRange"></param>
        /// <param name="endRange"></param>
        /// <param name="paymentRef"></param>
        /// <param name="mda"></param>
        /// <param name="revenueheadId"></param>
        /// <param name="invoiceNumber"></param>
        /// <param name="receiptNumber"></param>
        /// <param name="selectedRevenueHead"></param>
        /// <param name="selectedPaymentChannel"></param>
        /// <param name="paymentDirection"></param>
        /// <returns></returns>
        public async Task<ActionResult> CollectionReportDownload(string fromRange, string endRange, string paymentRef, string mda, string revenueheadId, string invoiceNumber, string receiptNumber, PaymentChannel? selectedPaymentChannel, CollectionPaymentDirection? paymentDirection, ExportFormat? format, string SelectedBank, string selectedPaymentProvider)
        {
            try
            {

                DateTime startDate = DateTime.Now.AddMonths(-6);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(fromRange) && !string.IsNullOrEmpty(endRange))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(fromRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(endRange, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                        if (selectedMonth > maximumReportMonths)
                        {
                            _notifier.Add(NotifyType.Error, ErrorLang.maximumreportmonthsrexceeded());
                            return RedirectToAction("CollectionReport", "Report", new { });
                        }
                    }
                    catch (Exception) { throw; }
                }
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                CollectionSearchParams searchData = new CollectionSearchParams
                {
                    FromRange = startDate,
                    EndRange = endDate,
                    InvoiceNumber = invoiceNumber,
                    PaymentRef = paymentRef,
                    SRevenueHeadId = revenueheadId,
                    SelectedPaymentProvider = string.IsNullOrEmpty(selectedPaymentProvider) ? ((int)PaymentProvider.None).ToString() : selectedPaymentProvider,
                    SelectedPaymentChannel = selectedPaymentChannel ?? PaymentChannel.None,
                    ReceiptNumber = receiptNumber,
                    SelectedMDA = mda,
                    SelectedBankCode = SelectedBank,
                    PaymentDirection = paymentDirection ?? CollectionPaymentDirection.PaymentDate,
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                    DontPageData = true,
                };

                CollectionReportViewModel viewModel = _handler.GetCollectionReport(searchData);

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "Collection_Report_" + searchData.FromRange.Value.ToString("dd/MM/yyyy") + "-" + searchData.EndRange.Value.ToString("dd/MM/yyyy") + ".xlsx";
                        await DoExcelAsync(viewModel.ReportRecords, fileName, SettingsFileNames.CollectionReport.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "Collection_Report_from_" + searchData.FromRange.Value.ToString("dd-MM-yyyy") + "_to_" + searchData.EndRange.Value.ToString("dd-MM-yyyy") + ".pdf";
                        StateConfig siteConfig = Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        await DoPDFAsync(viewModel, PDFfileName, SettingsFileNames.CollectionReport.ToString());
                        break;
                    default:
                        break;
                }
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                var message = String.Format("\nUser ID {0} tried to CollectionReport without permission", _orchardServices.WorkContext.CurrentUser.Id);
                Logger.Error(message);
                _notifier.Add(NotifyType.Warning, ErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (NoSearchFilterExceptionFound exception)
            {
                Logger.Error(exception, string.Format("Could not find filter parameters. Exception {0}", exception.ToString()));
                _notifier.Add(NotifyType.Error, Lang.searchcrriteriacouldnotbefound);
                return RedirectToAction("CollectionReport", "Report", new { });
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToAction("CollectionReport", "Report", new { });
            }
            #endregion
            return null;
        }



        /// <summary>
        /// Prep for PDF download
        /// </summary>
        /// <param name="searchData"></param>
        /// <param name="viewModel"></param>
        private async Task DoPDFAsync(dynamic viewModel, string fileName, string templateNAme)
        {
            var template = TemplateUtil.RazorTemplateFor(SettingsFileNames.CollectionReport.ToString());

            IPDFExportEngine _pdf = new PdfEngine();
            var pdfData = await _pdf.DownloadAsPdfAsync(null, template.File, viewModel, template.BasePath);
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            System.Web.HttpContext.Current.Response.OutputStream.Write(pdfData, 0, pdfData.Length);
            System.Web.HttpContext.Current.Response.End();
        }



        /// <summary>
        /// Prep excel download
        /// </summary>
        private async Task DoExcelAsync(dynamic viewModel, string fileName, string templateNAme)
        {
            var headers = TemplateUtil.KeyValueTemplateFor(templateNAme);

            IExcelExportEngine _excel = new ExcelEngine(headers);
            var excelData = await _excel.DownloadAsExcelAsync(headers, viewModel, fileName, string.Empty, string.Empty);
            System.Web.HttpContext.Current.Response.ClearHeaders();
            System.Web.HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            System.Web.HttpContext.Current.Response.OutputStream.Write(excelData, 0, excelData.Length);
            System.Web.HttpContext.Current.Response.End();
        }

    }
}