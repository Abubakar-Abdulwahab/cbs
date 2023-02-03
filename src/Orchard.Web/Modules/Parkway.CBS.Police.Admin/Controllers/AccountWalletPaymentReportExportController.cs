using Orchard;
using Orchard.Logging;
using Orchard.UI.Navigation;
using Orchard.UI.Notify;
using Parkway.CBS.Core.Exceptions;
using Parkway.CBS.Core.Lang;
using Parkway.CBS.Core.Models.Enums;
using Parkway.CBS.Core.StateConfig;
using Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts;
using Parkway.CBS.Police.Admin.RouteName;
using Parkway.CBS.Police.Admin.VM;
using Parkway.CBS.Police.Core.Lang;
using Parkway.CBS.Police.Core.Models.Enums;
using Parkway.CBS.Police.Core.VM;
using Parkway.DataExporter.Implementations;
using Parkway.DataExporter.Implementations.Contracts;
using Parkway.DataExporter.Implementations.Util;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Parkway.CBS.Police.Admin.Controllers
{
    public class AccountWalletPaymentReportExportController : Controller
    {
        private readonly IOrchardServices _orchardServices;
        private readonly IAccountWalletPaymentReportHandler _accountWalletPaymentReportHandler;
        public ILogger Logger { get; set; }
        private readonly INotifier _notifier;

        readonly int maximumReportMonths = 6;
        readonly int oneMonth = 1;

        public AccountWalletPaymentReportExportController(IOrchardServices orchardServices, IAccountWalletPaymentReportHandler accountWalletPaymentReportHandler, INotifier notifier)
        {
            _orchardServices = orchardServices;
            _accountWalletPaymentReportHandler = accountWalletPaymentReportHandler;
            _notifier = notifier;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        ///  Export Account wallet payment report to excel or pdf depending on the specified format
        /// </summary>
        /// <param name="userInputModel"></param>
        /// <param name="pagerParameters"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Download(AccountWalletPaymentReportVM userInputModel, PagerParameters pagerParameters, ExportFormat? format)
        {
            try
            {
                _accountWalletPaymentReportHandler.CheckForPermission(Permissions.CanViewWalletPaymentReport);

                var pager = new Pager(_orchardServices.WorkContext.CurrentSite, pagerParameters);

                DateTime startDate = DateTime.Now.AddMonths(-3);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(userInputModel.From) && !string.IsNullOrEmpty(userInputModel.End))
                {
                    try
                    {
                        startDate = DateTime.ParseExact(userInputModel.From, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        endDate = DateTime.ParseExact(userInputModel.End, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        int selectedMonth = 12 * (endDate.Year - startDate.Year) + (endDate.Month - startDate.Month) + oneMonth; //Added a month to balance the substraction e.g Jannuary to April, 4-1 will be 3 months instead of 4
                        if (selectedMonth > maximumReportMonths)
                        {
                            _notifier.Add(NotifyType.Error, ErrorLang.maximumreportmonthsrexceeded());
                            return RedirectToRoute(AccountWalletPaymentReport.Report);
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                //get the date up until the final sec
                endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);

                AccountWalletPaymentSearchParams searchParams = new AccountWalletPaymentSearchParams
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Skip = 0,
                    Take = 0,
                    DontPageData = true,
                    UserPartRecordId = _orchardServices.WorkContext.CurrentUser.Id,
                    PaymentId = userInputModel.PaymentId?.Trim(),
                    SourceAccountName = userInputModel.SourceAccount?.Trim(),
                    Status = userInputModel.Status,
                    BeneficiaryAccoutNumber = userInputModel.BeneficiaryAccountNumber?.Trim(),
                    ExpenditureHeadId = userInputModel.ExpenditureHeadId
                };

                var viewModel = _accountWalletPaymentReportHandler.GetAccountWalletPaymentReportVM(searchParams);

                switch (format)
                {
                    case ExportFormat.Excel:
                        string fileName = "Wallet_Approval_Payment_Report.xlsx";
                        await DoExcelAsync(viewModel.AccountWalletPaymentReports, fileName, PSSRequestSettingsName.PSSWalletPaymentReport.ToString());
                        break;
                    case ExportFormat.PDF:
                        string PDFfileName = "Wallet_Approval_Payment_Report.pdf";
                        StateConfig siteConfig = CBS.Core.Utilities.Util.GetTenantConfigBySiteName(_orchardServices.WorkContext.CurrentSite.SiteName);
                        Node node = siteConfig.Node.Where(x => x.Key == TenantConfigKeys.LogoPath.ToString()).FirstOrDefault();
                        Logger.Error(string.Format("BASE URL {0}", _orchardServices.WorkContext.CurrentSite.BaseUrl));
                        if (node != null)
                        {
                            viewModel.LogoURL = _orchardServices.WorkContext.CurrentSite.BaseUrl + node.Value;
                            Logger.Error("NODE FOUND {0}", viewModel.LogoURL);
                        }
                        viewModel.TenantName = _orchardServices.WorkContext.CurrentSite.SiteName;
                        var template = TemplateUtil.RazorTemplateFor(PSSRequestSettingsName.PSSWalletPaymentReport.ToString());

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

            return null;
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
    }
}