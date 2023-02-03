using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CBSPay.Core.APIModels;
using CBSPay.Core.Entities;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Models;
using CBSPay.Core.Services;
using CBSPay.Core.ViewModels;
using log4net;
using Newtonsoft.Json;
using PagedList;
namespace CBSPay.API.Controllers
{
    [Authorize]
    public class ReportController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IAdminService _adminService;
        private readonly ITaxPayerService _taxPayerService;
        //private readonly IBaseRepository<TaxPayerDetails> _taxPayerRepo;
        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }
        public ReportController()
        {
            _paymentService = new PaymentService();
            _adminService = new AdminService();
            _taxPayerService = new TaxPayerService();
        }
        // GET: Report
        public ActionResult Index()
        {
            //ViewBag.Report = "SettlementReport";

            var todayPOAAmount = _adminService.GetTodayPOAAmount();
            var todayBillSettlementAmount = _adminService.GetTodayBillSettlementAmount();
            var todaysTotaltransaction = _adminService.GetTodaysTotalTransaction();
            var thisWeeksTransaction = _adminService.GetPaymentTransactionDetailsForOneWeek();

            var dashboardViewModel = new AdminDashboardViewModel
            {
                todaysBillSettlementAmount = todayBillSettlementAmount == null ? 0 : Convert.ToDecimal(todayBillSettlementAmount),
                todaysPOAAmount = todayPOAAmount == null ? 0 : Convert.ToDecimal(todayPOAAmount),
                todaysTotalAmount = todaysTotaltransaction == null ? 0 : Convert.ToDecimal(todaysTotaltransaction),
                weeklyPaymentTransactions = thisWeeksTransaction
            };

            List<DataPoint> billSettlementDataPoint = new List<DataPoint>();
            List<DataPoint> poaDataPoint = new List<DataPoint>();
            List<DataPoint> totalTransactionDataPoint = new List<DataPoint>();

            foreach (var item in thisWeeksTransaction)
            {
                billSettlementDataPoint.Add(new DataPoint(item.DayOfWeek.ToString(), Convert.ToDouble(item.BillSettlementAmount)));
                poaDataPoint.Add(new DataPoint(item.DayOfWeek.ToString(), Convert.ToDouble(item.POAAmount)));
                totalTransactionDataPoint.Add(new DataPoint(item.DayOfWeek.ToString(), Convert.ToDouble(item.TotalAmount)));
            }

            ViewBag.BillSettlementDataPoint = JsonConvert.SerializeObject(billSettlementDataPoint);
            ViewBag.POADataPoint = JsonConvert.SerializeObject(poaDataPoint);
            ViewBag.TotalTransactionDataPoint = JsonConvert.SerializeObject(totalTransactionDataPoint);

            return View("Dashboard", dashboardViewModel);
        }
        [Route("Report/SettlementReport")]
        public ActionResult SettlementReport(int? page, string fromRange, string endRange, string referenceNumber, string paymentChannel, string paymentDate,string TaxPayerRIN, int pageSize = 100)
        {
            Logger.Info("Now in the settlementReport action method and about to get settled payments!!");
            try
            {
                int pageIndex = 0;
                var filterData = _taxPayerService.ReportFilter(page, fromRange, endRange, referenceNumber, paymentChannel, paymentDate, TaxPayerRIN, pageSize, out pageIndex);
                var records = _paymentService.GetSyncedSettlementTransaction(filterData);
                IPagedList<EIRSSettlementInfo> pageRecord = records.ToPagedList(pageIndex, pageSize); 
                return View(pageRecord);
            }
            catch (Exception exception)
            { 
                Logger.Error(exception.StackTrace, exception);
                return RedirectToAction("SettlementReport", "Report", new { });
            }

        }
        [Route("Report/ConsolidatedReport")]
        public ActionResult ConsolidatedReport(int? page, string fromRange, string endRange, string referenceNumber, string paymentChannel, string paymentDate, string TaxPayerRIN, int pageSize = 100)
        {
            try
            {
                var pageIndex = 0;
                var filterData = _taxPayerService.ReportFilter(page, fromRange, endRange, referenceNumber, paymentChannel, paymentDate, TaxPayerRIN, pageSize, out pageIndex);
                var records = _paymentService.GetTransactionRecord(filterData);
                IPagedList<TaxPayerTransaction> pageRecord = records.ToPagedList(pageIndex, pageSize);
                return View(pageRecord);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.StackTrace, exception);
                return RedirectToAction("ConsolidateReport", "Report", new { });
            }

        }
        public ActionResult PendingReport(int? page, string fromRange, string endRange, string referenceNumber, string paymentChannel, string paymentDate, string TaxPayerRIN, int pageSize = 100)
        {
            try
            {
                var pageIndex = 0;
                var filterData = _taxPayerService.ReportFilter(page, fromRange, endRange, referenceNumber, paymentChannel, paymentDate, TaxPayerRIN, pageSize, out pageIndex);
                var records = _paymentService.GetUnsyncedSettlementTransaction(filterData);
                IPagedList<TaxPayerTransaction> pageRecord = records.ToPagedList(pageIndex, pageSize);
                return View(pageRecord);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.StackTrace, exception);
                return RedirectToAction("PendingReport", "Report", new { });
            }
        }
        public ActionResult FailedReport(int? page, string fromRange, string endRange, string referenceNumber, string paymentChannel, string paymentDate, string TaxPayerRIN, int pageSize = 100)
        {
            try
            {
                var pageIndex = 0;
                var filterData = _taxPayerService.ReportFilter(page, fromRange, endRange, referenceNumber, paymentChannel, paymentDate, TaxPayerRIN, pageSize, out pageIndex);
                var records = _paymentService.GetFailedRequestTransaction(filterData);
                IPagedList<TaxPayerTransaction> pageRecord = records.ToPagedList(pageIndex, pageSize);
                return View(pageRecord);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                return RedirectToAction("FailedReport", "Report", new { });
            }
        }
        public ActionResult TaxPayerReport(int? page, int? TaxPayerTypeID, string TaxPayerRIN, string TaxPayerMobileNumber, int pageSize = 100)
        {//with links to as many other data as possible
            //you can either get taxpayers from the assessment & MDAservice bill or iteratively call from EIRS, save then use
            try
            {
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var taxPayerFilterParams = new TaxPayerFilterParams
                {
                    TaxPayerTypeID = TaxPayerTypeID,
                    TaxPayerRIN = TaxPayerRIN,
                    TaxPayerMobileNumber = TaxPayerMobileNumber
                };
                var filter = JsonConvert.SerializeObject(taxPayerFilterParams);
                var records = _taxPayerService.GetTaxPayerDetails(filter);
                IPagedList<TaxPayerDetails> pageRecord = records.ToPagedList(pageIndex,pageSize);
                return View(pageRecord);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message,ex);
                return RedirectToAction("TaxPayerReport", "Report", new { });
            }
        }
        public ActionResult AssessmentDetailsReport(int? page, string fromRange, string endRange, string AssessmentRefNo, string TaxPayerRIN, int? minAmount, int? maxAmount, int? SettlementStatusID, bool? Active, string settlementFromRange, string settlementEndRange, bool? due, int pageSize = 100)
        {
            try
            {
                #region passing the filter parameters to the query
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var StartDate = new DateTime(2000, 01, 01);
                var EndDate = DateTime.Now.Date;
                var taxPayerAssessmentFilterParams = new TaxPayerAssessmentFilterParams
                {
                    fromRange = fromRange != null ? DateTime.Parse(fromRange) : StartDate,
                    endRange = endRange != null ? DateTime.Parse(endRange) : EndDate,
                    AssessmentRefNo = AssessmentRefNo,
                    TaxPayerRIN = TaxPayerRIN,
                    minAmount = minAmount != null ? minAmount : 0,
                    maxAmount = maxAmount != null ? maxAmount : int.MaxValue,
                    SettlementStatusID = SettlementStatusID,
                    Active = Active,
                    settlementFromRange = settlementFromRange != null ? DateTime.Parse(settlementFromRange) : StartDate,
                    settlementEndRange = settlementEndRange != null ? DateTime.Parse(settlementEndRange) : EndDate,
                    due = due
                };
                var filter = JsonConvert.SerializeObject(taxPayerAssessmentFilterParams);
                var records = _taxPayerService.GetAssessmentDetailsResult(filter);
                IPagedList<AssessmentDetailsResult> pageRecord = records.ToPagedList(pageIndex, pageSize);
                return View(pageRecord);
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message,ex);
                return RedirectToAction("AssessmentDetailsReport", "Report", new { });
            }
        }
        public ActionResult AssessmentRuleReport(int? page, int? AssetTypeId, int? TaxYear, decimal? minAssessmentRuleAmount, decimal? maxAssessmentRuleAmount, decimal? minSettledAmount, decimal? maxSettledAmount, int pageSize = 100)
        {
            try
            {
                #region passing the filter parameters to the query
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var taxPayerAssessmentRuleFilterParams = new TaxPayerAssessmentRuleFilterParams
                {
                    AssetTypeId = AssetTypeId,
                    TaxYear = TaxYear,
                    minAssessmentRuleAmount = minAssessmentRuleAmount,//minAssessmentRuleAmount != null ? minAssessmentRuleAmount : 0,
                    maxAssessmentRuleAmount = maxAssessmentRuleAmount,//maxAssessmentRuleAmount != null ? maxAssessmentRuleAmount : int.MaxValue,
                    minSettledAmount = minSettledAmount,//minSettledAmount != null ? minSettledAmount : 0,
                    maxSettledAmount = maxSettledAmount//maxSettledAmount != null ? maxSettledAmount : int.MaxValue
                };
                var filter = JsonConvert.SerializeObject(taxPayerAssessmentRuleFilterParams);
                var records = _taxPayerService.GetAssessmentRule(filter);
                IPagedList<AssessmentRule> pageRecord = records.ToPagedList(pageIndex, pageSize);
                return View(pageRecord);
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RedirectToAction("AssessmentRuleReport", "Report", new { });
            }
        }
        public ActionResult AssessmentAssetReport(int? page, int pageSize = 100)
        {
            try
            {
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var AssetFilterParams = new AssetFilterParams
                {

                };
                var filter = JsonConvert.SerializeObject(AssetFilterParams);
                var records = _taxPayerService.GetAsset(filter);
                IPagedList<Asset> pageRecord = records.ToPagedList(pageIndex, pageSize);
                return View(pageRecord);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RedirectToAction("AssessmentAssetReport", "Report", new { });
            }
        }
        public ActionResult AssessmentProfileReport(int? page, int pageSize = 100)
        {
            try
            {
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var ProfileFilterParams = new ProfileFilterParams
                {

                };
                var filter = JsonConvert.SerializeObject(ProfileFilterParams);
                var records = _taxPayerService.GetProfile(filter);
                IPagedList<Profile> pageRecord = records.ToPagedList(pageIndex, pageSize);
                return View(pageRecord);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RedirectToAction("AssessmentProfileReport", "Report", new { });
            }
        }
        public ActionResult AssessmentRuleItemReport(int? page, int pageSize = 100)
        {
            try
            {
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var AssessmentItemFilterParams = new AssessmentItemFilterParams
                {

                };
                var filter = JsonConvert.SerializeObject(AssessmentItemFilterParams);
                var records = _taxPayerService.GetAssessmentItem(filter);
                IPagedList<AssessmentRuleItem> pageRecord = records.ToPagedList(pageIndex,pageSize);
                return View(pageRecord);
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured within the AssessmentRuleItemReport() Action method");
                Logger.Error(ex.Message, ex);
                return RedirectToAction("AssessmentRuleItemReport", "Report", new { });
            }
        }
        public ActionResult ServiceBillReport(int? page, int pageSize = 100)
        {
            try
            {
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var ServiceBillfilterParam = new ServiceBillfilterParams
                {

                };
                var filter = JsonConvert.SerializeObject(ServiceBillfilterParam);
                var records = _taxPayerService.GetServiceBill(filter);
                IPagedList<ServiceBillResult> pageRecord = records.ToPagedList(pageIndex,pageSize);
                return View(pageRecord);
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured within the ServiceBillReport() Action method");
                Logger.Error(ex.Message, ex);
                return RedirectToAction("ServiceBillReport", "Report", new { });
            }
        }
        public ActionResult MDAServiceReport(int? page, int pageSize = 100)
        {
            try
            {
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var MDAfilter = new MDAfilter
                {

                };
                var filter = JsonConvert.SerializeObject(MDAfilter);
                var records = _taxPayerService.GetMDAService(filter);
                IPagedList<MDAService> pageRecord = records.ToPagedList(pageIndex,pageSize);
                return View(pageRecord);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RedirectToAction("MDAServiceReport", "Report", new { });
            }
        }
        public ActionResult ServiceBillItemReport(int? page, int pageSize = 100)
        {
            try
            {
                var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                var ServiceItemFilter = new ServiceItemFilter
                {

                };
                var filter = JsonConvert.SerializeObject(ServiceItemFilter);
                var records = _taxPayerService.GetServiceBillItem(filter);
                IPagedList<ServiceBillItem> pageRecord = records.ToPagedList(pageIndex,pageSize);
                return View(pageRecord);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RedirectToAction("ServiceBillItemReport", "Report", new { });
            }
        }
        [HttpPost, Route("Report/GetSettlementReportDetail")]
        public ActionResult GetSettlementReportDetail(string transactionRefNo)
        {
            try
            {
                var record = _paymentService.GetSettlementReportDetails(transactionRefNo);

                return PartialView("SettlementDetails", record);

            }
            catch (Exception exception)
            {
                Logger.Error(exception.StackTrace, exception);
                 
                return RedirectToAction("", "");
            }
        }
    }
    //public class FilterParams
    //{
    //    public DateTime? StartDate { get; set; }
    //    public DateTime? EndDate { get; set; }
    //    public string RIN { get; set; }
    //    public string ReferenceNumber { get; set; }
    //    public DateTime? PaymentDate { get; set; }
    //    public string PaymentChannel { get; set; }
    //}
}