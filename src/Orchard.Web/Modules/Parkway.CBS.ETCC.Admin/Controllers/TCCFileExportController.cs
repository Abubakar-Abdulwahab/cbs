using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.StateConfig;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.ETCC.Admin.VM;
using Parkway.CBS.ETCC.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Core.HelperModels;
using System.IO;

namespace Parkway.CBS.ETCC.Admin.Controllers
{
    public class TCCFileExportController : Controller
    {

        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAuthenticationService _authenticationService;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        private readonly ITCCReportHandler _tccReportHandler;
        private Localizer T { get; }
        int maximumReportMonths = 6;
        int oneMonth = 1;


        public TCCFileExportController(IOrchardServices orchardServices, IAuthenticationService authenticationService, IShapeFactory shapeFactory, ITCCReportHandler tccReportHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Shape = shapeFactory;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            _tccReportHandler = tccReportHandler;
        }


        /// <summary>
        /// Export TCCRequest report to excel or pdf depending on the specified format
        /// <para>43th iteration.</para>
        /// </summary>
        /// <param name="requestStatus"></param>
        /// <param name="From"></param>
        /// <param name="End"></param>
        /// <param name="format"></param>
        /// <param name="selectedServiceId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> TCCRequestReportDownload(TCCRequestReportVM userInput, ExportFormat? format)
        {
            try
            {
                _tccReportHandler.CheckForPermission(Permissions.CanViewTCCRequests);

                DateTime startDate = DateTime.Now.AddMonths(-3);
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
                            return RedirectToAction("RequestReport", "TCCReport", new { });
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                TCCReportSearchParams searchParams = new TCCReportSearchParams
                {
                    EndDate = endDate,
                    StartDate = startDate,
                    ApplicationNumber = userInput.ApplicationNumber,
                    ApplicantName = userInput.ApplicantName,
                    PayerId = userInput.PayerId,
                    SelectedStatus = userInput.Status,
                    DontPageData = true
                };

                TCCRequestReportVM model = _tccReportHandler.GetRequestReport(searchParams);

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "TCCRequests_Report_from_" + startDate.ToString("dd/MM/yyyy") + "-" + endDate.ToString("dd/MM/yyyy") + ".xlsx";
                        await DoExcelAsync(model.Requests, fileName, SettingsFileNames.TCCRequestReport.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "TCCRequests_Report_from_" + startDate.ToString("dd-MM-yyyy") + "_to_" + endDate.ToString("dd-MM-yyyy") + ".pdf";
                        StateConfig siteConfig = Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            model.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", model.LogoURL);
                        }
                        model.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        await DoPDFAsync(model, PDFfileName, SettingsFileNames.TCCRequestReport.ToString());
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
                return RedirectToAction("RequestReport", "TCCReport", new { });
            }
            #endregion
            return null;
        }


        /// <summary>
        /// Prep for PDF download
        /// </summary>
        /// <param name="searchData"></param>
        /// <param name="viewModel"></param>
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
        /// Download a particular using the filename, path and content type
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TCCAttachmentDownload(string fileName, string path, string contentType)
        {
            try
            {
                System.Web.HttpContext.Current.Response.ClearHeaders();
                System.Web.HttpContext.Current.Response.ContentType = contentType;
                System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName.Replace(" ", "_") + Path.GetExtension(path));
                System.Web.HttpContext.Current.Response.TransmitFile(path);
                System.Web.HttpContext.Current.Response.End();
            }
            #region Catch clauses
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return Redirect("~/Admin");
            }
            #endregion
            return null;
        }
    }
}