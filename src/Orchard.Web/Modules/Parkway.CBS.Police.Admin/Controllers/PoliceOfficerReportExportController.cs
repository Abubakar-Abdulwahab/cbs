using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class PoliceOfficerReportExportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAuthenticationService _authenticationService;
        dynamic Shape { get; set; }
        public ILogger Logger { get; set; }
        public IRequestListHandler _handler;
        private readonly IPoliceOfficerReportHandler _policeOfficerReportHandler;
        private Localizer T { get; }


        public PoliceOfficerReportExportController(IOrchardServices orchardServices, IAuthenticationService authenticationService, IShapeFactory shapeFactory, IRequestListHandler handler, IPoliceOfficerReportHandler policeOfficerReportHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Shape = shapeFactory;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            _handler = handler;
            T = NullLocalizer.Instance;
            _policeOfficerReportHandler = policeOfficerReportHandler;
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
        /// Export PSS officers report to excel or pdf depending on the specified format
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> PoliceOfficersReportDownload(PoliceOfficerReportVM userInput, ExportFormat? format)
        {
            try
            {
                _policeOfficerReportHandler.CheckForPermission(Permissions.CanViewOfficers);

                var searchData = new PoliceOfficerSearchParams
                {
                    Take = 0,
                    Skip = 0,
                    AdminUserId = _orchardServices.WorkContext.CurrentUser.Id,
                    SelectedCommand = userInput.SelectedCommand,
                    IPPISNo = userInput.IppisNumber,
                    Rank = userInput.SelectedRank,
                    LGA = userInput.SelectedLGA,
                    State = userInput.SelectedState,
                    OfficerName = userInput.OfficerName,
                    IdNumber = userInput.IdNumber,
                    DontPageData = true
                };

                //CollectionReportVM viewModel = _pssReportHandler.GetVMForRequestReport(searchData);
                PoliceOfficerReportVM viewModel = _policeOfficerReportHandler.GetVMForReports(searchData);

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "PSS_Officers_Report.xlsx";
                        await DoExcelAsync(viewModel.Reports, fileName, PSSRequestSettingsName.PssOfficersReport.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "PSS_Officers_Report.pdf";
                        StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor(PSSRequestSettingsName.PssOfficersReport.ToString());

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
                return RedirectToAction("PoliceOfficerReport", "PoliceOfficerReport", new { });
            }
            #endregion
            return null;
        }
    }
}