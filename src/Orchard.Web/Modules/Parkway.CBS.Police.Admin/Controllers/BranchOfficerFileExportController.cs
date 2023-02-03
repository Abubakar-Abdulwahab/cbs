using Orchard;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class BranchOfficerFileExportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly INotifier _notifier;
        private readonly IAuthenticationService _authenticationService;
        dynamic Shape { get; set; }
        ILogger Logger { get; set; }
        private readonly IBranchOfficerHandler _branchOfficerHandler;
        private Localizer T { get; }

        public BranchOfficerFileExportController(IOrchardServices orchardServices, IAuthenticationService authenticationService, IShapeFactory shapeFactory, IBranchOfficerHandler branchOfficerHandler)
        {
            _orchardServices = orchardServices;
            _notifier = orchardServices.Notifier;
            Shape = shapeFactory;
            _authenticationService = authenticationService;
            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
            _branchOfficerHandler = branchOfficerHandler;
        }


        /// <summary>
        /// Export PSSRequest report to excel or pdf depending on the specified format
        /// <para>43th iteration.</para>
        /// </summary>
        /// <param name="userInput"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> OfficerRequestDownload(string batchId, string profileId, ExportFormat? format)
        {
            try
            {
                _branchOfficerHandler.CheckForPermission(Permissions.CanDownloadBranchProfileOfficerRequest);

                if (!int.TryParse(batchId, out int batchIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid batch id"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = batchId });
                }

                if (!int.TryParse(profileId, out int profileIdResult))
                {
                    _notifier.Add(NotifyType.Error, ErrorLang.ToLocalizeString("Invalid profile id"));
                    return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = batchId });
                }

                var viewModel = _branchOfficerHandler.GetEscortRequestDetailVM(new PSSBranchOfficersUploadBatchItemsStagingReportSearchParams
                {
                    ProfileLocationId = profileIdResult,
                    BatchId = batchIdResult,
                    DontPageData = true
                });

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "PSS_EGS_Regularization_Officers_Request.xlsx";
                        await DoExcelAsync(viewModel.BranchOfficers, fileName, nameof(PSSRequestSettingsName.PSSBranchOfficers));
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "PSS_EGS_Regularization_Officers_Request.pdf";
                        StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        //viewModel.ReportRecords = detailReports;
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor(nameof(PSSRequestSettingsName.PSSBranchOfficers));

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
                _notifier.Add(NotifyType.Warning, PoliceErrorLang.usernotauthorized());
                return Redirect("~/Admin");
            }
            catch (Exception exception)
            {
                Logger.Error(exception, exception.Message);
                _notifier.Add(NotifyType.Error, ErrorLang.genericexception());
                return RedirectToRoute(BranchOfficer.BranchProfileDetail, new { id = batchId });
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