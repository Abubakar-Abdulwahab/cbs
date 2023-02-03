using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Parkway.CBS.Core.DataFilters.TaxPayerReport;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Core.Lang;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PSSDeploymentReportExportController : Controller
    {

        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAuthenticationService _authenticationService;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        public IRequestListHandler _handler;
        private readonly IPSSCollectionReportHandler _pssReportHandler;
        private readonly IDeploymentReportHandler _deploymentReportHandler;
        private Localizer T { get; }
        int maximumReportMonths = 6;
        int oneMonth = 1;


        public PSSDeploymentReportExportController(IOrchardServices orchardServices, IAuthenticationService authenticationService, IShapeFactory shapeFactory, IRequestListHandler handler, ITaxPayerReportFilter taxPayerReportFilter, IPSSCollectionReportHandler pssReportHandler, IDeploymentReportHandler deploymentReportHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Shape = shapeFactory;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            _handler = handler;
            T = NullLocalizer.Instance;
            _pssReportHandler = pssReportHandler;
            _deploymentReportHandler = deploymentReportHandler;
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
        /// Export PSS deployment report to excel or pdf depending on the specified format
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PSSDeployedOfficersReportDownload(DeploymentReportVM userInput, ExportFormat? format)
        {
            try
            {
                _handler.CheckForPermission(Permissions.CanViewDeployments);

                DateTime startDate = DateTime.Now.Date.AddMonths(-3);
                DateTime endDate = DateTime.Now.Date;

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
                            return RedirectToAction("PSSDeployedOfficers", "PSSDeployedOfficersReport", new { });
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                var searchData = new OfficerDeploymentSearchParams
                {
                    Take = 0,
                    Skip = 0,
                    EndDate = endDate,
                    StartDate = startDate,
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                    SelectedCommand = userInput.SelectedCommand,
                    CustomerName = userInput.CustomerName,
                    InvoiceNumber = userInput.InvoiceNumber,
                    IPPISNo = userInput.IPPISNo,
                    APNumber = userInput.APNumber,
                    RequestRef = userInput.RequestRef,
                    Rank = userInput.Rank,
                    LGA = userInput.LGA,
                    State = userInput.State,
                    OfficerName = userInput.OfficerName,
                    DontPageData = true
                };

                //CollectionReportVM viewModel = _pssReportHandler.GetVMForRequestReport(searchData);
                DeploymentReportVM viewModel = _deploymentReportHandler.GetVMForReports(searchData);

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "PSSDeployedOfficers_Report_from_" + startDate.ToString("dd/MM/yyyy") + "-" + endDate.ToString("dd/MM/yyyy") + ".xlsx";
                        await DoExcelAsync(viewModel.ReportRecords, fileName, PSSRequestSettingsName.PssDeployedOfficersReport.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "PSSDeployedOfficers_Report_from_" + startDate.ToString("dd-MM-yyyy") + "_to_" + endDate.ToString("dd-MM-yyyy") + ".pdf";
                        StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor(PSSRequestSettingsName.PssDeployedOfficersReport.ToString());

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
                return RedirectToAction("PSSRequests", "PoliceRequest", new { });
            }
            #endregion
            return null;
        }

    }
}