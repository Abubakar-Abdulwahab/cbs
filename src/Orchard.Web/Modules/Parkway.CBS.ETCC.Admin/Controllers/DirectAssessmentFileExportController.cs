using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Parkway.CBS.ETCC.Admin.Controllers
{
    public class DirectAssessmentFileExportController : Controller
    {
        public ITCCReportHandler _handler;
        private readonly IDirectAssessmentReportHandler _directAssessmentReportHandler;
        private readonly INotifier _notifier;
        private readonly IOrchardServices _orchardServices;
        private readonly int maximumReportMonths = 6;
        private readonly int oneMonth = 1;
        public ILogger Logger { get; set; }
        private Localizer T { get; }

        public DirectAssessmentFileExportController(IOrchardServices orchardServices, ITCCReportHandler handler, IDirectAssessmentReportHandler directAssessmentReportHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            _handler = handler;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            _directAssessmentReportHandler = directAssessmentReportHandler;
        }

        /// <summary>
        ///  Export DirectAssessment report to excel or pdf depending on the specified format
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DirectAssessmentReportDownload(PAYEDirectAssessmentReportVM userInput, ExportFormat? format)
        {
            try
            {
                _handler.CheckForPermission(Permissions.ViewDirectAssessmentReport);

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInput.From) && !string.IsNullOrEmpty(userInput.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInput.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInput.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth;

                        if (selectedMonth > maximumReportMonths)
                        {
                            _notifier.Add(NotifyType.Error, ErrorLang.maximumreportmonthsrexceeded());
                            return RedirectToAction("DirectAssessmentRequestReport", "DirectAssessmentReport", new { });
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                int.TryParse(userInput.DirectAssessmentType, out int directAssessmentType);

                DirectAssessmentSearchParams searchParams = new DirectAssessmentSearchParams
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    TIN = userInput.TIN,
                    InvoiceNumber = userInput.InvoiceNo,
                    DirectAssessmentType = directAssessmentType,
                    InvoiceStatus = userInput.InvoiceStatus,
                    DontPageData = true
                };

                var model = _directAssessmentReportHandler.GetDirectAssessmentReport(searchParams);

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "DirectAssessment_Report_from_" + startDate.ToString("dd/MM/yyyy") + "-" + endDate.ToString("dd/MM/yyyy") + ".xlsx";
                        await DoExcelAsync(model.DirectAssessmentReportItems, fileName, SettingsFileNames.DirectAssessmentReport.ToString());
                        break;

                    case ExportFormat.PDF:
                        string PDFfileName = "DirectAssessment_Report_from_" + startDate.ToString("dd-MM-yyyy") + "_to_" + endDate.ToString("dd-MM-yyyy") + ".pdf";
                        StateConfig siteConfig = Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();

                        if (node != null)
                        {
                            model.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                        }

                        model.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        await DoPDFAsync(model, PDFfileName, SettingsFileNames.DirectAssessmentReport.ToString());
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
                return RedirectToAction("DirectAssessmentRequestReport", "DirectAssessmentReport", new { });
            }

            #endregion Catch clauses

            return null;
        }

        /// <summary>
        /// Prepare for excel download
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="fileName"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Prepare for PDF download
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="fileName"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
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
    }
}