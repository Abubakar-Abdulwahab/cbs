using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
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

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSSettlementReportFileExportController : Controller
    {

        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IPSSSettlementReportHandler _handler;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        private Localizer T { get; }
        int maximumReportMonths = 6;
        int oneMonth = 1;


        public PSSSettlementReportFileExportController(IOrchardServices orchardServices, IShapeFactory shapeFactory, IPSSSettlementReportHandler handler)
        {
            _orchardServices = orchardServices;
            _handler = handler;
            _notifier = orchardServices.Notifier;
            Shape = shapeFactory;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }


        /// <summary>
        /// Export PSS Settlement Summary report to excel or pdf depending on the specified format
        /// <para>43th iteration.</para>
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SummaryReportDownload(PSSSettlementReportSummaryVM userInput, ExportFormat? format)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportSummary);
                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInput.From) && !string.IsNullOrEmpty(userInput.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                        if (selectedMonth > maximumReportMonths)
                        {
                            _notifier.Add(NotifyType.Error, ErrorLang.maximumreportmonthsrexceeded());
                            return RedirectToAction("ReportSummary", "PSSSettlementReport", new { });
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                var searchData = new PSSSettlementReportSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = 0,
                    Skip = 0,
                    PageData = false
                };

                PSSSettlementReportSummaryVM viewModel = _handler.GetVMForReportSummary(searchData);
                viewModel.TotalReportAmount = viewModel.ReportRecords.Sum(x => x.SettlementAmount);
                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "PSS_Settlement_Report_Summary_from_" + startDate.ToString("dd/MM/yyyy") + "-" + endDate.ToString("dd/MM/yyyy") + ".xlsx";
                        await DoExcelAsync(viewModel.ReportRecords, fileName, PSSRequestSettingsName.PssSettlementReportSummary.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "PSS_Settlement_Report_Summary_from_" + startDate.ToString("dd-MM-yyyy") + "_to_" + endDate.ToString("dd-MM-yyyy") + ".pdf";
                        StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor(PSSRequestSettingsName.PssSettlementReportSummary.ToString());

                        IPDFExportEngine _pdf = new PdfEngine();
                        var pdfData = _pdf.DownloadAsPdf(null, template.File, viewModel, template.BasePath);
                        System.Web.HttpContext.Current.Response.ClearHeaders();
                        System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                        System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + PDFfileName);
                        System.Web.HttpContext.Current.Response.OutputStream.Write(pdfData, 0, pdfData.Length);
                        System.Web.HttpContext.Current.Response.End();
                        break;
                    default:
                        break;
                }
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToAction("ReportSummary", "PSSSettlementReport", new { });
            }
            #endregion
            return null;
        }




        /// <summary>
        /// Export PSS Settlement Invoices report to excel or pdf depending on the specified format
        /// <para>43th iteration.</para>
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> InvoicesReportDownload(string batchRef, ExportFormat? format)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportSummary);
                if (string.IsNullOrEmpty(batchRef)) 
                {
                    _notifier.Add(NotifyType.Error, T("PSS Settlement Batch Ref not specified"));
                    return RedirectToAction("ReportSummary", "PSSSettlementReport", new { });
                }
                batchRef = batchRef.Trim();
                var searchData = new PSSSettlementReportSearchParams
                {
                    Take = 0,
                    Skip = 0,
                    BatchId = _handler.GetSettlementBatchId(batchRef),
                    PageData = false
                };

                PSSSettlementReportInvoicesVM viewModel = _handler.GetVMForReportInvoices(searchData);
                viewModel.SettlementBatchRef = batchRef;
                viewModel.TotalReportAmount = viewModel.ReportRecords.Sum(x => x.SettlementAmount);
                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "PSS_Settlement_Invoices_Report_For_" + batchRef + ".xlsx";
                        await DoExcelAsync(viewModel.ReportRecords, fileName, PSSRequestSettingsName.PssSettlementReportInvoices.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "PSS_Settlement_Invoices_Report_For_" + batchRef + ".pdf";
                        StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor(PSSRequestSettingsName.PssSettlementReportInvoices.ToString());

                        IPDFExportEngine _pdf = new PdfEngine();
                        var pdfData = _pdf.DownloadAsPdf(null, template.File, viewModel, template.BasePath);
                        System.Web.HttpContext.Current.Response.ClearHeaders();
                        System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                        System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + PDFfileName);
                        System.Web.HttpContext.Current.Response.OutputStream.Write(pdfData, 0, pdfData.Length);
                        System.Web.HttpContext.Current.Response.End();
                        break;
                    default:
                        break;
                }
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToAction("ReportSummary", "PSSSettlementReport", new { });
            }
            #endregion
            return null;
        }


        /// <summary>
        /// Export PSS Settlement Report Breakdown to excel or pdf depending on the specified format
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ReportBreakdownDownload(PSSSettlementReportBreakdownVM userInput, ExportFormat? format)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewSettlementReportPartyBreakdown);
                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInput.StartDate) && !string.IsNullOrEmpty(userInput.EndDate))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                        if (selectedMonth > maximumReportMonths)
                        {
                            _notifier.Add(NotifyType.Error, ErrorLang.maximumreportmonthsrexceeded());
                            return RedirectToRoute(RouteName.PSSSettlementReport.ReportBreakdown);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                PSSSettlementReportBreakdownSearchParams searchParams = new PSSSettlementReportBreakdownSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Take = 0,
                    Skip = 0,
                    FileNumber = userInput.FileNumber,
                    InvoiceNumber = userInput.InvoiceNumber,
                    SelectedState = userInput.SelectedState,
                    SelectedLGA = userInput.SelectedLGA,
                    SelectedSettlementParty = userInput.SelectedSettlementParty,
                    SelectedService = userInput.SelectedService,
                    SelectedCommand = userInput.SelectedCommand,
                    PageData = false
                };

                PSSSettlementReportBreakdownVM viewModel = _handler.GetSettlementReportBreakdownVM(searchParams);
                viewModel.TotalReportAmount = viewModel.ReportRecords.Sum(x => x.SettlementAmount);
                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "PSS_Settlement_Report_Breakdown_from_" + startDate.ToString("dd/MM/yyyy") + "-" + endDate.ToString("dd/MM/yyyy") + ".xlsx";
                        await DoExcelAsync(viewModel.ReportRecords, fileName, nameof(PSSRequestSettingsName.PssSettlementReportBreakdown));
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "PSS_Settlement_Report_Breakdown_from_" + startDate.ToString("dd-MM-yyyy") + "_to_" + endDate.ToString("dd-MM-yyyy") + ".pdf";
                        StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == nameof(TenantConfigKeys.LogoPath)).FirstOrDefault();
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor(nameof(PSSRequestSettingsName.PssSettlementReportBreakdown));

                        IPDFExportEngine _pdf = new PdfEngine();
                        var pdfData = _pdf.DownloadAsPdf(null, template.File, viewModel, template.BasePath);
                        System.Web.HttpContext.Current.Response.ClearHeaders();
                        System.Web.HttpContext.Current.Response.ContentType = "application/pdf";
                        System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + PDFfileName);
                        System.Web.HttpContext.Current.Response.OutputStream.Write(pdfData, 0, pdfData.Length);
                        System.Web.HttpContext.Current.Response.End();
                        break;
                    default:
                        break;
                }
            }
            #region Catch clauses
            catch (UserNotAuthorizedForThisActionException)
            {
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute(RouteName.PSSSettlementReport.ReportBreakdown);
            }
            #endregion
            return null;
        }


        /// <summary>
        /// Prep for PDF download
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="fileName"></param>
        /// <param name="templateName"></param>
        private async Task DoPDFAsync(dynamic viewModel, string fileName, string templateName)
        {
            var template = TemplateUtil.RazorTemplateFor(templateName);

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
        /// <param name="viewModel"></param>
        /// <param name="fileName"></param>
        /// <param name="templateName"></param>
        private async Task DoExcelAsync(dynamic viewModel, string fileName, string templateName)
        {
            var headers = TemplateUtil.KeyValueTemplateFor(templateName);

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