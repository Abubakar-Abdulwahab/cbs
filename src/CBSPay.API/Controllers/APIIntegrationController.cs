using CBSPay.Core.ViewModels;
using CBSPay.Core.Services;
using System;
using System.Net;
using System.Web.Http;
using CBSPay.Core.Helpers;
using log4net;
using System.Web;
using CBSPay.Core.Interfaces;
using RetryLib;
using CBSPay.Core.APIModels;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Collections.Generic;
using CBSPay.Core.Models;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using PagedList;
using CBSPay.Core.Entities;

namespace CBSPay.API.Controllers
{
    //[JSONControllerConfig]
    /// <summary>
    /// controller for handling API Integration
    /// </summary>
    public class APIIntegrationController : ApiController
    {
        private readonly ITaxPayerService _taxPayerService;
        private readonly IPaymentService _paymentService;
        private readonly IRestService _restService;
        private readonly IBaseRepository<AssessmentDetailsResult> _assessmentRepo;
        private readonly IBaseRepository<ServiceBillResult> _serviceBillRepo;
        private APIResponse result;
        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }

        public APIIntegrationController()
        {
            _taxPayerService = new TaxPayerService();
            _paymentService = new PaymentService();
            _restService = new RestService();
            _assessmentRepo = new Repository<AssessmentDetailsResult>();
            _serviceBillRepo = new Repository<ServiceBillResult>();
        }
        
        /// <summary>
        /// Updates the payment history table with payment information from BankCollect
        /// </summary>
        /// <param name="paymentDetails"></param>
        /// <returns >APIResponse</returns>
        /// GET: api/Integration/PaymentNotification
        [HttpPost]
        [Route("api/PaymentNotification")]
        public APIResponse BankCollectPaymentNotification(PaymentDetails paymentDetails)
        {
            try
            {
                var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
                var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");

                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret))
                {
                    result = _paymentService.ProcessBankCollectPaymentNotification(paymentDetails);
                    //result = _paymentService.SavePaymentNotificationInfo(paymentDetails);
                }
                else
                {
                    Logger.Error($"Invalid Client Id : {ClientId}, or Merchant Secret : {ClientSecret}");
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, ErrorMessage = "You are not authorized to call this endpoint" };
                }
                return result;
            } 
             catch (Exception ex)
            {
                Logger.Error($"An error occurred {ex.Message} - {ex.InnerException}, could not process NetPay payment Notification Request");
                Logger.Error(ex.StackTrace, ex);

                return new APIResponse { StatusCode = HttpStatusCode.InternalServerError, ErrorMessage = "An error occured, please while processing payment request pls try again" };
            }
                         
        }
        
        /// <summary>
        /// Endpoint for banks(in-branch) to validate get the details of the taxpayer using the ref number
        /// </summary>
        /// <param name="refNumber"></param>
        /// <returns>APIResponse</returns>
        [HttpGet]
        [Route("api/GetTaxPayerDataByRefNumber")]
        public IHttpActionResult RetrieveTaxPayerData(string refNumber)
        //public APIResponse RetrieveTaxPayerData(string refNumber)
        {

            var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
            var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");

            if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret) && refNumber != null)
            {
                //determine what endpoint to call

                var response = _taxPayerService.RetrieveTaxPayerInfo(refNumber);
                if (response != null)
                {
                    result = new APIResponse { Success = true, ErrorMessage = "", Result = response, StatusCode = HttpStatusCode.OK };
                    //log the info
                   // return result;
                    //return Content(HttpStatusCode.OK, result);
                    return Content(HttpStatusCode.OK, result);

                }
                Logger.Error($"An error occurred - could not retrieve taxpayer data");
                //return new APIResponse { StatusCode = HttpStatusCode.InternalServerError, ErrorMessage = "An error occurred - could not retrieve taxpayer data" };
                return Content(HttpStatusCode.InternalServerError, "");

            }
            else
            {
                Logger.Error($"Invalid Merchant Id : {ClientId}, or Merchant Secret : {ClientSecret}");
                //return new APIResponse { StatusCode = HttpStatusCode.Forbidden, ErrorMessage = "You are not authorized to call this endpoint" };
                return Content(HttpStatusCode.Forbidden, "");
            }
        }

        /// <summary>
        /// Endpoint for banks(in-branch) to validate get the details of the taxpayer using the RIN and phone number
        /// </summary>
        /// <param name="RIN"></param>
        /// <param name="phoneNumber"></param>
        /// <returns>APIResponse</returns>
        [HttpGet]
        [Route("api/GetTaxPayerDataByRIN")]
        public APIResponse RetrieveTaxPayerData(string RIN,string mobileNumber)
        {
            try
            {
                var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
                var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");

                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret) && RIN != null)
                {
                    //determine what endpoint to call

                    var response = _taxPayerService.RetrieveTaxPayerInfo(RIN, mobileNumber);
                    if (response != null)
                    {
                        result = new APIResponse { Success = true, ErrorMessage = "", Result = response, StatusCode = HttpStatusCode.OK };
                        //log the info
                        return result;
                    }
                    Logger.Error($"An error occurred - could not retrieve taxpayer data");
                    return new APIResponse { StatusCode = HttpStatusCode.InternalServerError, ErrorMessage = "An error occurred - could not retrieve taxpayer data" };
                }
                else
                {
                    Logger.Error($"Invalid Merchant Id : {ClientId}, or Merchant Secret : {ClientSecret}");
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, ErrorMessage = "You are not authorized to call this endpoint" };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve tax payer info at this time, please try again");
                Logger.Error(ex.StackTrace, ex);
                result = new APIResponse { Success = false, ErrorMessage = "Could not retrieve tax payer info at this time, please try again", Result = null, StatusCode = HttpStatusCode.InternalServerError };
                //log the error
                return result;
                throw;
            }
            

            
        }

        [HttpGet]
        [Route("api/GetConsolidatedTransaction")]
        [ResponseType(typeof(APIResponse))]
        public APIResponse GetConsolidatedTransactions(int page, int pageSize, string filteroption)
        {
            var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
            var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");

            try
            {
                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret))
                {

                    var record = _paymentService.GetTransactionRecord(page, pageSize, filteroption);

                    var response = new APIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Success = true,
                        Result = record
                    };
                    return response;

                }
                else
                {
                    Logger.Error($"Invalid Client Id : {ClientId}, or Merchant Secret : {ClientSecret}");
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, ErrorMessage = "You are not authorized to call this endpoint" };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve Consolidated Transactions , please try again");
                Logger.Error(ex.StackTrace, ex);
                return new APIResponse
                {
                    Success = false,
                    ErrorMessage = "Could not retrieve Consolidated Transactions at this time, please try again",
                    Result = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                throw;
            }
        }
        [HttpGet]
        [Route("api/GetPOATransactions")]
        [ResponseType(typeof(APIResponse))]
        public APIResponse GetPOATransactions(int page, int pageSize, string filteroption)
        {

            var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
            var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");
            try
            {
                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret))
                {
                    var record = _paymentService.GetPOATransactions(page, pageSize, filteroption);
                    var response = new APIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Success = true,
                        Result = record
                    };
                    return response;
                }
                else
                {
                    Logger.Error($"Invalid Client Id : {ClientId}, or Merchant Secret : {ClientSecret}");
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, ErrorMessage = "You are not authorized to call this endpoint" };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve Pay On Account Transactions , please try again");
                Logger.Error(ex.StackTrace, ex);
                return new APIResponse
                {
                    Success = false,
                    ErrorMessage = "Could not retrieve POA Transactions at this time, please try again",
                    Result = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                throw;
            }

        }
        [HttpGet]
        [Route("api/GetUnsyncedSettlementTransaction")]
        [ResponseType(typeof(APIResponse))]
        public APIResponse GetUnsyncedSettlementTransaction(int page, int pageSize, string filteroption)
        {
            var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
            var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");

            try
            {
                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret))
                {
                    // get the unsyncedSettlement transaction with RDM
                    var data = _paymentService.GetUnsyncedSettlementTransaction(page, pageSize, filteroption);

                    var response = new APIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Success = true,
                        Result = data
                    };
                    return response;
                }

                else
                {
                    Logger.Error($"Invalid Client Id : {ClientId}, or Merchant Secret: {ClientSecret}");
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, ErrorMessage = "You are not Authorized to call this endpoint" };

                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve Pay On Account Transactions , please try again");
                Logger.Error(ex.StackTrace, ex);
                return new APIResponse
                {
                    Success = false,
                    ErrorMessage = "Could not retrieve Unsync settlement transactions at this time, please try again",
                    Result = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                throw;

            }

        }
        [HttpGet]
        [Route("api/GetSettlementReportDetails")]
        [ResponseType(typeof(APIResponse))]
        public APIResponse GetSettlementReportDetails(string transactionRefNo)
        {
            var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
            var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");

            try
            {
                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret))
                {
                    // get the unsyncedSettlement transaction with RDM
                    var data = _paymentService.GetSettlementReportDetails(transactionRefNo);

                    var response = new APIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Success = true,
                        Result = data
                    };
                    return response;
                }

                else
                {
                    Logger.Error($"Invalid Client Id : {ClientId}, or Merchant Secret: {ClientSecret}");
                    return new APIResponse { StatusCode = HttpStatusCode.Forbidden, ErrorMessage = "You are not Authorized to call this endpoint" };

                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve Pay On Account Transactions , please try again");
                Logger.Error(ex.StackTrace, ex);
                return new APIResponse
                {
                    Success = false,
                    ErrorMessage = "Could not retrieve Unsync settlement transactions at this time, please try again",
                    Result = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
                throw;

            }

        }

        /// <summary>
        /// serialize to JSON
        /// </summary>
        internal class JSONControllerConfigAttribute : Attribute, IControllerConfiguration
        {
            public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
            {
                var config = GlobalConfiguration.Configuration;
                //var xmlFormatter = GlobalConfiguration.Configuration.Formatters.XmlFormatter;
                //xmlFormatter.UseXmlSerializer = true;
                //controllerSettings.Formatters.Clear();
                //controllerSettings.Formatters.Add(xmlFormatter);

                //config.Formatters.Clear();
                //config.Formatters.Add(new JsonNetFormatter());

            }
        }
        [HttpGet]
        [Route("api/pageList")]
        public List<RevenueStream> PageList()
        {
            IPagedList<RevenueStream> revStream = new ConstantAPIModelService().FetchRevenueStreamList().ToPagedList(1, 3);
            var JsonRev = JsonConvert.SerializeObject(revStream);
            List<RevenueStream> revrev = JsonConvert.DeserializeObject<List<RevenueStream>>(JsonRev);
            return revrev;
        }
        [HttpGet]
        [Route("UnsyncedPayment/List")]
        public List<PaymentHistory> UnsyncedPaymentList()
        {
            var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
            var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");
            try
            {
                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret))
                {
                    IPagedList<PaymentHistory> IPagedListResponse = new PaymentService().GetUnsyncedPaymentRecords().ToPagedList(1,25);
                    var JsonResponse = JsonConvert.SerializeObject(IPagedListResponse);
                    List<PaymentHistory> response = JsonConvert.DeserializeObject<List<PaymentHistory>>(JsonResponse);
                    return response;
                }
                else
                {
                    Logger.Error($"Invalid Client Id : {ClientId}, or Merchant Secret : {ClientSecret}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve unsynced payments, please try again");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        [HttpPost]
        [Route("Payment/Update")]
        public APIResponse PaymentUpdate([FromBody] PaymentHistory record)
        {
            try
            {
                var ClientId = HttpContext.Current.Request.Headers.Get("ClientId");
                var ClientSecret = HttpContext.Current.Request.Headers.Get("ClientSecret");
                if (WebRequestValidationService.IsValidRequest(ClientId, ClientSecret))
                {
                    new PaymentService().UpdatePaymentHistoryRecords(record);
                    var response = new APIResponse
                    {
                        StatusCode = HttpStatusCode.OK,
                        Success = true,
                        Result = "Successfully updated!!"
                    };
                    return response;
                }
                else
                {
                    Logger.Error($"Invalid Client Id : {ClientId}, or Merchant Secret : {ClientSecret}");
                    return new APIResponse
                    {
                        Success = false,
                        ErrorMessage = "you are not authorized to call this endpoint",
                        Result = null,
                        StatusCode = HttpStatusCode.InternalServerError
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve unsynced payments, please try again");
                Logger.Error(ex.StackTrace, ex);
                return new APIResponse
                {
                    Success = false,
                    ErrorMessage = "Could not make update to PaymentHistory record with payment identifier: " + record.PaymentIdentifier,
                    Result = null,
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
        }
        [HttpGet]
        [Route("process")]
        public void Process()
        {
            //var patha = HttpContext.Current.Server.MapPath("~/config.json");
            //var pathb = HttpContext.Current.Server.MapPath("config.json");
            //using (StreamReader r = new StreamReader(patha))
            //{
            //    Dictionary<string, string> Configuration = JsonConvert.DeserializeObject<Dictionary<string,string>>(r.ReadToEnd());
            //    var appbaseurl = Configuration["AppBaseUrl"];
            //    var UpdatePaymentUrl = Configuration["UpdatePaymentUrl"];
            //}
        }
        [HttpGet]
        [Route("api/GetEIRSAPIToken")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPILoginResponse GetEIRSAPIToken()
        {
            return new RestService().GetEIRSAPIToken();
        }
        [HttpGet]
        [Route("api/GetAssessmentDetailsByRefNumber")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetAssessmentDetailsByRefNumber(string referenceNumber)
        {
            return new RestService().GetAssessmentDetailsByRefNumber(referenceNumber);
        }
        [HttpGet]
        [Route("api/GetServiceBillDetailsByRefNumber")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetServiceBillDetailsByRefNumber(string referenceNumber)
        {
            return new RestService().GetServiceBillDetailsByRefNumber(referenceNumber);
        }
        [HttpGet]
        [Route("api/GetAssessmentRuleItems")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetAssessmentRuleItems(long assessmentID)
        {
            return new RestService().GetAssessmentRuleItems(assessmentID);
        }
        [HttpGet]
        [Route("api/GetServiceBillItems")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetServiceBillItems(long serviceBillID)
        {
            return new RestService().GetServiceBillItems(serviceBillID);
        }
        [HttpGet]
        [Route("api/GetTaxPayerByRINAndMobile")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetTaxPayerByRINAndMobile(string rin, string mobileNumber)
        {
            return new RestService().GetTaxPayerByRINAndMobile(rin, mobileNumber);
        }
        [HttpGet]
        [Route("api/GetTaxPayerByBusinessName")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetTaxPayerByBusinessName(string business)
        {
            return new RestService().GetTaxPayerByBusinessName(business);
        }
        [HttpGet]
        [Route("api/GetTaxPayerByMobileNumber")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetTaxPayerByMobileNumber(string mobile)
        {
            return new RestService().GetTaxPayerByMobileNumber(mobile);
        }
        [HttpGet]
        [Route("api/GetTaxPayerByRIN")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetTaxPayerByRIN(string rin)
        {
            return new RestService().GetTaxPayerByRIN(rin);
        }
        [HttpGet]
        [Route("api/GetRevenueSubStreamList")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetRevenueSubStreamList()
        {
            return new RestService().GetRevenueSubStreamList();
        }
        [HttpGet]
        [Route("api/GetRevenueStreamList")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetRevenueStreamList()
        {
            return new RestService().GetRevenueStreamList();
        }
        [HttpGet]
        [Route("api/GetTaxPayerTypeList")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetTaxPayerTypeList()
        {
            return new RestService().GetTaxPayerTypeList();
        }
        [HttpGet]
        [Route("api/GetEconomicActivitiesList")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetEconomicActivitiesList(int taxPayerTypeID)
        {
            return new RestService().GetEconomicActivitiesList(taxPayerTypeID);
        }
        [HttpGet]
        [Route("api/GetAssessmentRules")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetAssessmentRules(long assessmentID)
        {
            return new RestService().GetAssessmentRules(assessmentID);
        }
        [HttpGet]
        [Route("api/GetServiceBillRules")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public EIRSAPIResponse GetServiceBillRules(long serviceBillID)
        {
            return new RestService().GetServiceBillRules(serviceBillID);
        }
        [HttpGet]
        [Route("api/GetQuickTellerPaymentTransactionDetails")]
        [ResponseType(typeof(EIRSAPIResponse))]
        public List<KeyValuePair<string, object>> GetQuickTellerPaymentTransactionDetails(string transRef, string requestReference)
        {
            return new RestService().GetQuickTellerPaymentTransactionDetails(transRef,requestReference);
        }
        //Admin services api//
        [HttpGet]
        [Route("api/GetTodayBillSettlementAmount")]
        public decimal? GetTodayBillSettlementAmount()
        {
            return new AdminService().GetTodayBillSettlementAmount();
        }
        [HttpGet]
        [Route("api/GetTodayPOAAmount")]
        public decimal? GetTodayPOAAmount()
        {
            return new AdminService().GetTodayPOAAmount();
        }
        [HttpGet]
        [Route("api/GetTodaysTotalTransaction")]
        public decimal? GetTodaysTotalTransaction()
        {
            return new AdminService().GetTodaysTotalTransaction();
        }
        [HttpGet]
        [Route("api/GetPaymentTransactionDetailsForOneWeek")]
        public IEnumerable<WeeklyPaymentTransaction> GetPaymentTransactionDetailsForOneWeek()
        {
            return new AdminService().GetPaymentTransactionDetailsForOneWeek();
        }
        [HttpGet]
        [Route("api/GetThisWeekBillSettlementAmount")]
        public List<PaymentHistory> GetThisWeekBillSettlementAmount()
        {
            return new AdminService().GetThisWeekBillSettlementAmount();
        }
        [HttpGet]
        [Route("api/GetThisWeekPOAAmount")]
        public List<PaymentHistory> GetThisWeekPOAAmount()
        {
            return new AdminService().GetThisWeekPOAAmount();
        }
        //constant API model service//
        [HttpGet]
        [Route("api/SaveTaxPayersTypeList")]
        public string SaveTaxPayersTypeList()
        {
            try
            {
                new ConstantAPIModelService().SaveTaxPayersTypeList();
                return "successful";
            }
            catch (Exception)
            {

                return "an error occured!!";
            }
        }
        //Tax Payer Service//
        [HttpGet]
        [Route("api/ProcessAssessmentDetails")]
        public string ProcessAssessmentDetails(string refNumber)
        {
            var ans = new TaxPayerService().ProcessAssessmentDetails(refNumber);
            //var result = JsonConvert.SerializeObject(ans);
            return "done!";
        }
        [HttpGet]
        [Route("api/ProcessServiceBill")]
        public string ProcessServiceBill(string refNumber)
        {
            var ans = new TaxPayerService().ProcessServiceBill(refNumber);
            return "Done!!";
        }
        [HttpGet]
        [Route("api/saveAllServiceBills")]
        public void saveAllServiceBills()
        {
            for(var i = 0; i <= 99999; i++)
            {
                if(i >= 0 && i <= 9)
                {
                    ProcessServiceBill($"sb0000{i}");
                    //ProcessAssessmentDetails($"ab0000{i}");
                }
                else if(i >= 10 && i <= 99)
                {
                    ProcessServiceBill($"sb000{i}");
                    //ProcessAssessmentDetails($"ab000{i}");
                }
                else if(i >= 100 && i <= 999)
                {
                    ProcessServiceBill($"sb00{i}");
                    //ProcessAssessmentDetails($"ab00{i}");
                }
                else if (i >= 1000 && i <= 9999)
                {
                    ProcessServiceBill($"sb0{i}");
                    //ProcessAssessmentDetails($"ab0{i}");
                }
                else if (i >= 10000 && i <= 99999)
                {
                    ProcessServiceBill($"sb{i}");
                    //ProcessAssessmentDetails($"ab{i}");
                }
            }
        }
        [HttpGet]
        [Route("api/saveAllAssessmentBills")]
        public void saveAllAssessmentBills()
        {
            for (var i = 0; i <= 99999; i++)
            {
                if (i >= 0 && i <= 9)
                {
                    //ProcessServiceBill($"sb0000{i}");
                    ProcessAssessmentDetails($"ab0000{i}");
                }
                else if (i >= 10 && i <= 99)
                {
                    //ProcessServiceBill($"sb000{i}");
                    ProcessAssessmentDetails($"ab000{i}");
                }
                else if (i >= 100 && i <= 999)
                {
                    //ProcessServiceBill($"sb00{i}");
                    ProcessAssessmentDetails($"ab00{i}");
                }
                else if (i >= 1000 && i <= 9999)
                {
                    //ProcessServiceBill($"sb0{i}");
                    ProcessAssessmentDetails($"ab0{i}");
                }
                else if (i >= 10000 && i <= 99999)
                {
                    //ProcessServiceBill($"sb{i}");
                    ProcessAssessmentDetails($"ab{i}");
                }
            }
        }
        public void TaxPayerFromAB()
        {
            var data = _assessmentRepo.Fetch(a => a.Id > 0);
            foreach(var d in data)
            {
                GetTaxPayerByRIN(d.TaxPayerRIN);
            }
        }
        [HttpGet]
        [Route("api/ProcessPOATaxPayerDetails")]
        public string ProcessPOATaxPayerDetails()
        {
            var allAssessment = _assessmentRepo.Fetch(x => x.AssessmentID > 0);
            var allService = _serviceBillRepo.Fetch(x => x.ServiceBillID > 0);

            List<POATaxPayerResponse> taxPayerDetails = new List<POATaxPayerResponse>();
            //var sbTaxPayers = "";var abTaxPayers = "";
            foreach (var x in allAssessment)
            {
                var abTaxPayer = _taxPayerService.RetrieveTaxPayerInfoByRIN(x.TaxPayerRIN);
                foreach(var xx in abTaxPayer)
                {
                    taxPayerDetails.Add(xx);
                }
                
            }
            foreach (var y in allService)
            {
                var sbTaxPayer = _taxPayerService.RetrieveTaxPayerInfoByRIN(y.TaxpayerRIN);
                foreach (var yy in sbTaxPayer)
                {
                    taxPayerDetails.Add(yy);
                }
            }
            //At this point, we have all the taxpayer details in "taxPayerDetails", so lets move on...
            foreach(var taxPayerDetail in taxPayerDetails)
            {
                new TaxPayerService().ProcessPOATaxPayerDetails(taxPayerDetail);
            }
            return "done";
        }
        //Payment Service//
        [HttpGet]
        [Route("api/GetPaymentRequestDetails")]
        public EIRSPaymentRequest GetPaymentRequestDetails(string paymentRef)
        {
            return new PaymentService().GetPaymentRequestDetails(paymentRef);
        }
        [HttpGet]
        [Route("api/TestProcessNetPayPaymentNotification")]
        public PaymentStatusModel TestProcessNetPayPaymentNotification(string paymentTransactionRef, decimal amountpaid)
        {
            //HttpWebRequest hwr = (HttpWebRequest)new WebRequest();
            //HttpRequest httpRequest = new HttpRequest(); HttpResponse httpResponse = new HttpResponse();//for further study
            //HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            var GetAppConfig = HttpContext.GetAppConfig("EIRSBaseUrl");
            var current = HttpContext.Current.Request.QueryString.AllKeys;
            var xcurrent = HttpContext.Current.Request.RawUrl;
            var ycurrent = HttpContext.Current.Request.Url;
            var zcurrent = HttpContext.Current.Request.Headers.AllKeys;
            var acurrent = HttpContext.Current.Request.ApplicationPath;
            var bcurrent = HttpContext.Current.Request.AcceptTypes;
            var ccurrent = HttpContext.Current.Request.Browser;
            var dcurrent = HttpContext.Current.Request.ServerVariables.AllKeys;

            var request = HttpContext.Current.Request.Cookies.AllKeys;
            var Items = HttpContext.Current.Items.Values;
            var Server = HttpContext.Current.Server.MapPath("");
            var response = HttpContext.Current.Response;
            var session = HttpContext.Current.Session.Keys;
            var application = HttpContext.Current.Application;
            var applicationInstance = HttpContext.Current.ApplicationInstance.User;
            var XapplicationInstance = HttpContext.Current.ApplicationInstance;
            HttpContext.Current.ApplicationInstance.ToDictionary();

            var requestNotification = HttpContext.Current.CurrentNotification;
            HttpContext.Current.GetOwinContext().Authentication.SignOut();
            HttpContext.Current.GetOwinContext().Authentication.SignIn();
            var GetHashCode = HttpContext.Current.GetHashCode();
            var handler = HttpContext.Current.Handler;
            var PageInstrumentation = HttpContext.Current.PageInstrumentation;
            var previousHandler = HttpContext.Current.PreviousHandler;
            var Profile = HttpContext.Current.Profile;
            var Timestamp = HttpContext.Current.Timestamp;
            var Trace = HttpContext.Current.Trace;
            var user = HttpContext.Current.User;
            var WebSocketNegotiatedProtocol = HttpContext.Current.WebSocketNegotiatedProtocol;
            var WebSocketRequestedProtocols = HttpContext.Current.WebSocketRequestedProtocols;
            //var x = new HttpContext(httpRequest, httpResponse);//for further study
            return new PaymentService().TestProcessNetPayPaymentNotification(paymentTransactionRef, amountpaid);
        }
        [HttpPost]
        [Route("api/NotifyEIRSOfSettlementPayment")]
        public string NotifyEIRSOfSettlementPayment([FromBody] EIRSSettlementInfo settlementRequest)
        {
            var tokenResult = GetEIRSAPIToken();
            var token = tokenResult.access_token;
            var tokenType = tokenResult.token_type;
            var header = $"{tokenType} {token}";

            var client = new RestClient("https://stage-api.eirsautomation.xyz/");
            var request = new RestRequest("RevenueData/Settlement/Add", Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddHeader("Authorization", header);
            request.AddJsonBody(settlementRequest);
            var response = client.Execute<EIRSAPIResponse>(request);
            return response.Content;
        }
    }
}