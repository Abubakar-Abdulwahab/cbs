using CBSPay.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBSPay.Core.ViewModels;
using log4net;
using System.Net;
using CBSPay.Core.Helpers;
using CBSPay.Core.APIModels;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Entities;
using Newtonsoft.Json;
using NHibernate.Criterion;
using System.Globalization;
using NHibernate;
using System.Data.SqlTypes;
using PagedList;
namespace CBSPay.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBaseRepository<PaymentHistory> _paymentHistoryRepo;
        private readonly IBaseRepository<PaymentHistoryItem> _paymentHistoryItemRepo;
        private readonly IBaseRepository<EIRSPaymentRequest> _eirsPaymentRequestRepo;
        private readonly IBaseRepository<EIRSPaymentRequestItem> _eirsPaymentRequestItemRepo;
        private readonly IBaseRepository<AssessmentDetailsResult> _assessmentDetailsRepo;
        private readonly IBaseRepository<ServiceBillResult> _serviceBillRepo;
        private readonly IBaseRepository<AssessmentRuleItem> _assessmentRuleItemRepo;
        private readonly IBaseRepository<ServiceBillItem> _serviceBillItemRepo;
        private readonly ITaxPayerService _taxPayerService;
        private readonly IRestService _restService;
        private ILog Logger { get { return LogManager.GetLogger("CBSPay"); } }

        public PaymentService()
        {
            _paymentHistoryRepo = new Repository<PaymentHistory>();
            _paymentHistoryItemRepo = new Repository<PaymentHistoryItem>();
            _eirsPaymentRequestRepo = new Repository<EIRSPaymentRequest>();
            _restService = new RestService();
            _eirsPaymentRequestItemRepo = new Repository<EIRSPaymentRequestItem>();
            _assessmentDetailsRepo = new Repository<AssessmentDetailsResult>();
            _serviceBillRepo = new Repository<ServiceBillResult>();
            _assessmentRuleItemRepo = new Repository<AssessmentRuleItem>();
            _serviceBillItemRepo = new Repository<ServiceBillItem>();
            _taxPayerService = new TaxPayerService();
        }
        /// <summary>
        /// Updates and synchronizes payment information in the db
        /// </summary>
        /// <param name = "paymentDetails" ></ param >
        /// < returns > APIResponse </ returns >
        public APIResponse SavePaymentNotificationInfo(PaymentDetails paymentDetails)//not used
        {
            var result = new APIResponse();
            try
            {
                var paymentHistory = new PaymentHistory();

                #region Populate and Save PaymentHistory object
                //check if this is a new payment or not
                var paymentInfo = _paymentHistoryRepo.Find(x => x.ReferenceNumber == paymentDetails.ReferenceNumber);
                if (paymentInfo == null)
                {
                    Logger.InfoFormat("This is the first payment for reference number: {0}", paymentDetails.ReferenceNumber);
                    //populate PaymentHistory object
                    Logger.Info("Populating PaymentHistory object");
                    //confirm if i need to do anyother thing here before saving
                    paymentHistory = new PaymentHistory
                    {
                        ReferenceAmount = paymentDetails.ReferenceAmount,
                        AmountPaid = paymentDetails.AmountPaid,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        TotalAmountPaid = paymentDetails.AmountPaid,
                        PaymentDate = paymentDetails.DatePaid,
                        PaymentChannel = paymentDetails.PaymentChannel.ToString(),
                        IsDeleted = false,
                        ReferenceNumber = paymentDetails.ReferenceNumber,
                        TaxPayerMobileNumber = paymentDetails.PhoneNumber,
                        TaxPayerName = paymentDetails.TaxPayerName,
                        PaymentIdentifier = $"{paymentDetails.PaymentChannel.ToString()}_{paymentDetails.ReferenceNumber}_{DateTime.Now.Day}_{DateTime.Now.TimeOfDay}",
                        IsCustomerDeposit = false,
                        IsSyncedWithEIRS = false,
                        SettlementDate = DateTime.Now,

                    };
                    //save the record in the db
                    Logger.Info("Saving PaymentHistory object");
                    _paymentHistoryRepo.Insert(paymentHistory);
                }
                else
                {
                    Logger.InfoFormat("This is not the first payment for reference number: {0}", paymentDetails.ReferenceNumber);
                    //populate PaymentHistory object
                    Logger.Info("Populating PaymentHistory object");
                    paymentHistory = new PaymentHistory
                    {
                        ReferenceAmount = paymentDetails.ReferenceAmount,
                        AmountPaid = paymentDetails.AmountPaid,
                        DateModified = DateTime.Now,
                        TotalAmountPaid = paymentInfo.TotalAmountPaid + paymentDetails.AmountPaid,
                        PaymentDate = paymentDetails.DatePaid,
                        UpdatedPaymentChannel = paymentDetails.PaymentChannel.ToString(),
                        ReferenceNumber = paymentDetails.ReferenceNumber,
                        TaxPayerMobileNumber = paymentDetails.PhoneNumber,
                        TaxPayerName = paymentDetails.TaxPayerName,
                        IsCustomerDeposit = false,
                        IsSyncedWithEIRS = false,
                        SettlementDate = paymentDetails.DatePaid

                    };
                    //update the record in the db
                    Logger.Info("Updating PaymentHistory object");
                    _paymentHistoryRepo.Update(paymentHistory);
                }
                #endregion

                result = new APIResponse { Success = true, ErrorMessage = "", Result = null, StatusCode = HttpStatusCode.OK };
                return result;
            }

            #region handle error
            catch (Exception ex)
            {
                var err = $"An error occured: {ex.Message}. please try again";
                result = new APIResponse { Success = false, ErrorMessage = err, Result = null, StatusCode = HttpStatusCode.InternalServerError };
                Logger.Error(err);
                Logger.Error(ex.StackTrace, ex);
                return result;
            }
            #endregion

        }
        /// <summary>
        /// Updates and synchronizes Pay Direct payment information in the db
        /// </summary>
        /// <param name="payments"></param>
        /// <returns>PayDirectResponse</returns>
        public APIResponse SynchronizePayDirectPayments(List<Payment> payments)//not used
        {
            try
            {
                Logger.Debug("About to Synchronize PayDirect's Payment Notification");
                //variables
                var paymentHistory = new PaymentHistory();
                var paymentInfo = new PaymentHistory();
                var payDirectResp = new PaymentNotificationResponse();
                APIResponse response;
                //string responseString;
                //begin synchronization
                foreach (var payment in payments)
                {
                    try
                    {
                        #region Populate and Save/Update PaymentHistory object
                        //custReference is RIN-phoneNumber

                        //check if it is a reversal
                        #region reversal call
                        if (string.Equals(payment.IsReversal.ToString(), "True", StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.DebugFormat("This is a reversal payment notification for customer with ref number: {0} and payment reference: {1}", payment.CustReference, payment.PaymentReference);
                            //check if it is repeated
                            // the OriginalPaymentReference and  OriginalPaymentLogId must be included in a reversal call
                            paymentInfo = _paymentHistoryRepo.Find(x => x.PaymentReference == payment.OriginalPaymentReference && x.PaymentLogId == Convert.ToInt32(payment.OriginalPaymentLogId));
                            //if not repeated reversal
                            if (paymentInfo == null)
                            {//not repeated reversal
                                // the OriginalPaymentReference and  OriginalPaymentLogId must be included in a reversal call

                                Logger.Error($"The reversal payment notification for customer with ref number: { payment.CustReference} should be sent with a new payment reference and payment log Id");
                                //return status 1 
                                payDirectResp = new PaymentNotificationResponse
                                {
                                    Payments = new List<PaymentResponse>
                                    {
                                      new PaymentResponse
                                      {
                                          PaymentLogId = payment.PaymentLogId,
                                          Status = 1,
                                      }
                                    }
                                };
                                Logger.Debug("Reversal nullified!!");
                                return response = new APIResponse { Success = true, Result = payDirectResp };
                            }
                            else
                            {
                                paymentInfo.PaymentReference = payment.PaymentReference;
                                paymentInfo.OriginalPaymentReference = payment.OriginalPaymentReference;
                                paymentInfo.PaymentLogId = Convert.ToInt32(payment.PaymentLogId);
                                paymentInfo.TotalAmountPaid = paymentInfo.AmountPaid + payment.Amount;//paymentInfo.AmountPaid - payment.Amount;
                                paymentInfo.OriginalPaymentLogId = Convert.ToInt32(payment.OriginalPaymentLogId);
                                paymentInfo.AmountPaid = payment.Amount;
                                paymentInfo.DateModified = DateTime.Now;
                                paymentInfo.IsSyncedWithEIRS = false;
                                paymentInfo.SettlementAmount = payment.Amount;
                                paymentInfo.ReceiptNo = payment.ReceiptNo;
                                //leave as empty for now till we confirm how we want to go about customer deposit for paydirect
                                paymentInfo.TaxPayerTIN = "";
                                paymentInfo.IsCustomerDeposit = false;
                                paymentInfo.TaxPayerRIN = "";
                                paymentInfo.TaxPayerName = "";
                                paymentInfo.TaxPayerMobileNumber = "";
                                paymentInfo.PaymentDate = Convert.ToDateTime(paymentInfo.PaymentDate);
                                paymentInfo.SettlementDate = Convert.ToDateTime(paymentInfo.SettlementDate);
                                //update the record in the db
                                Logger.Info("Updating PaymentHistory object");
                                _paymentHistoryRepo.Update(paymentInfo);
                                //return status 0
                                payDirectResp = new PaymentNotificationResponse
                                {
                                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 0,
                                  }
                                }
                                };
                                //Logger.Debug("Serialize the object to a string");
                                //responseString = Utils.SerializeToXML<PaymentNotificationResponse>(payDirectResp);
                                return response = new APIResponse { Success = true, Result = payDirectResp };
                            }
                        }
                        #endregion

                        #region duplicate call
                        //check if it is a duplicate call
                        if (string.Equals(payment.IsRepeated.ToString(), "True", StringComparison.OrdinalIgnoreCase))
                        {
                            paymentInfo = _paymentHistoryRepo.Find(x => x.PaymentReference == payment.PaymentReference && x.ReceiptNo == payment.ReceiptNo && x.PaymentLogId == Convert.ToInt32(payment.PaymentLogId) && x.AmountPaid == payment.Amount);
                            if (paymentInfo != null)
                            {
                                //return 0 and add a status message stating that the payment notification has been synchronized before
                                payDirectResp = new PaymentNotificationResponse
                                {
                                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 0,
                                     //StatusMessage = $"This payment notification has already been synchronized"
                                  }
                                }
                                };
                                //Logger.Debug("Serialize the object to a string");
                                //responseString = Utils.SerializeToXML<PaymentNotificationResponse>(payDirectResp);
                                return response = new APIResponse { Success = true, Result = payDirectResp };
                            }
                            //if it is repeated but somehow doesn't exist in our database, try to save it n our db
                        }
                        #endregion

                        #region first call
                        //if first non-reversal call
                        Logger.DebugFormat("This is the first payment for customer with ref number: {0} and payment reference: {1}", payment.CustReference, payment.PaymentReference);

                        var rNo = payment.CustReference.Trim();
                        var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();
                        bool saveValue;
                        switch (firstTwoChars)
                        {
                            case "AB":
                                //EIRSAPIResponse res = _restService.GetAssessmentDetailsByRefNumber(rNo);
                                //var assessmentDetails = res.Result;
                                var assessmentDetails = _assessmentDetailsRepo.Find(x => x.AssessmentRefNo == rNo);
                                if (assessmentDetails != null)
                                {
                                    //string output = JsonConvert.SerializeObject(assessmentDetails);
                                    //AssessmentDetailsResult deserializedAssessment = JsonConvert.DeserializeObject<AssessmentDetailsResult>(output);

                                    var paymentDetails = new PaymentDetails
                                    {
                                        AmountPaid = payment.Amount,
                                        DatePaid = Convert.ToDateTime(payment.PaymentDate),
                                        IsCustomerDeposit = false,
                                        PaymentChannel = PaymentChannel.PayDirect,
                                        ReferenceNumber = rNo,
                                        PaymentIdentifier = payment.PaymentLogId,
                                        PaymentReference = payment.PaymentReference,
                                        ReceiptNo = payment.ReceiptNo
                                    };
                                    saveValue = SaveAssessmentPaymentHistoryInfo(paymentDetails, assessmentDetails);
                                    if (saveValue == true)
                                    {
                                        payDirectResp = new PaymentNotificationResponse
                                        {
                                            Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 0,
                                      //StatusMessage = $"Payment notification was successfully synchronized"
                                  }
                                }
                                        };
                                        //responseString = Utils.SerializeToXML<PaymentNotificationResponse>(payDirectResp);
                                        return response = new APIResponse { Success = true, Result = payDirectResp };
                                    }
                                }

                                payDirectResp = new PaymentNotificationResponse
                                {
                                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 1,
                                     // StatusMessage = $"Payment notification failed to synchronize, please try again"
                                  }
                                }
                                };
                                return response = new APIResponse { Success = false, Result = payDirectResp };
                            case "SB":
                                //EIRSAPIResponse resp = _restService.GetServiceBillDetailsByRefNumber(rNo);
                                //var serviceBill = resp.Result;
                                var serviceBill = _serviceBillRepo.Find(x => x.ServiceBillRefNo == rNo);
                                //string billOutput = JsonConvert.SerializeObject(serviceBill);
                                //ServiceBillResult deserializedBill = JsonConvert.DeserializeObject<ServiceBillResult>(billOutput);
                                if (serviceBill != null)
                                {
                                    var paymentDetails = new PaymentDetails
                                    {
                                        AmountPaid = payment.Amount,
                                        DatePaid = Convert.ToDateTime(payment.PaymentDate),
                                        IsCustomerDeposit = false,
                                        PaymentChannel = PaymentChannel.PayDirect,
                                        ReferenceNumber = rNo,
                                        PaymentIdentifier = payment.PaymentLogId,
                                        PaymentReference = payment.PaymentReference,
                                        ReceiptNo = payment.ReceiptNo
                                    };

                                    saveValue = SaveServiceBillPaymentHistoryInfo(paymentDetails, serviceBill);
                                    if (saveValue == true)
                                    {
                                        payDirectResp = new PaymentNotificationResponse
                                        {
                                            Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 0,
                                      //StatusMessage = $"Payment notification was successfully synchronized"
                                  }
                                }
                                        };
                                        //responseString = Utils.SerializeToXML<PaymentNotificationResponse>(payDirectResp);
                                        return response = new APIResponse { Success = true, Result = payDirectResp };
                                    }
                                }
                                payDirectResp = new PaymentNotificationResponse
                                {
                                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 1,
                                      //StatusMessage = $"Payment notification failed to synchronize, please try again"
                                  }
                                }
                                };
                                return response = new APIResponse { Success = false, Result = payDirectResp };
                            default:
                                payDirectResp = new PaymentNotificationResponse
                                {
                                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 1,
                                      //StatusMessage = $"Payment notification failed to synchronize, please try again"
                                  }
                                }
                                };
                                return response = new APIResponse { Success = false, Result = payDirectResp };
                        }
                        #region old code
                        ////populate PaymentHistory object
                        //Logger.Debug("Populating PaymentHistory object");
                        ////confirm if i need to do anyother thing here before saving
                        //paymentHistory = new PaymentHistory
                        //{
                        //    //ReferenceAmount = ,
                        //    AmountPaid = payment.Amount,
                        //    DateCreated = DateTime.Now,
                        //    DateModified = DateTime.Now,
                        //    TotalAmountPaid = payment.Amount,
                        //    PaymentDate = Convert.ToDateTime(payment.PaymentDate),
                        //    PaymentChannel = "PayDirect",
                        //    IsDeleted = false,
                        //    ReferenceNumber = payment.CustReference,
                        //    //TaxPayerMobileNumber = phoneNumber,
                        //    //TaxPayerName = paymentDetails.TaxPayerName,
                        //    PaymentLogId = Convert.ToInt32(payment.PaymentLogId),
                        //    AlternateCustReference = payment.AlternateCustReference,
                        //    BankName = !String.IsNullOrWhiteSpace(payment.BankName) ? payment.BankName : "",
                        //    BranchName = !String.IsNullOrWhiteSpace(payment.BranchName) ? payment.BranchName : "",
                        //    ChannelName = payment.ChannelName,
                        //    CustReference = payment.CustReference,
                        //    FeeName = !String.IsNullOrWhiteSpace(payment.FeeName) ? payment.FeeName : "",
                        //    InstitutionId = payment.InstitutionId,
                        //    InstitutionName = payment.InstitutionName,
                        //    Location = !String.IsNullOrWhiteSpace(payment.Location) ? payment.Location : "",
                        //    PaymentCurrency = Convert.ToInt32(payment.PaymentCurrency),
                        //    PaymentIdentifier = $"PayDirect_{payment.CustReference}_{DateTime.Now.Day}_{DateTime.Now.TimeOfDay}",
                        //    PaymentMethod = payment.PaymentMethod,
                        //    PaymentReference = payment.PaymentReference,
                        //    ReceiptNo = payment.ReceiptNo,
                        //    SettlementDate = Convert.ToDateTime(payment.SettlementDate),
                        //    Teller = !String.IsNullOrWhiteSpace(payment.Teller) ? payment.Teller : "",
                        //    IsSyncedWithEIRS = false,
                        //    SettlementAmount = payment.Amount,
                        //    //leave as empty for now till we confirm how we want to go about customer deposit for paydirect
                        //    TaxPayerTIN = "",
                        //    IsCustomerDeposit = false,
                        //    TaxPayerRIN = "",
                        //    TaxPayerName = "",
                        //    TaxPayerMobileNumber = ""
                        //};
                        ////save the record in the db
                        //Logger.Debug("Saving PaymentHistory object");
                        //_paymentHistoryRepo.Insert(paymentHistory);
                        ////return status 0
                        //payDirectResp = new PaymentNotificationResponse
                        //{
                        //    Payments = new List<Payment>
                        //            {
                        //              new Payment
                        //              {
                        //                  PaymentLogId = payment.PaymentLogId,
                        //                  Status = 0,
                        //                  StatusMessage = $"Payment notification was successfully synchronized"
                        //              }
                        //            }
                        //};
                        ////responseString = Utils.SerializeToXML<PaymentNotificationResponse>(payDirectResp);
                        //return response = new APIResponse { Success = true, Result = payDirectResp };
                        #endregion

                        #endregion
                    }
                    #endregion

                    catch (Exception ex)
                    {
                        Logger.Error("An error occurred while trying to synchronize payDirects' payment Notification");
                        Logger.Error(ex.StackTrace, ex);
                        payDirectResp = new PaymentNotificationResponse
                        {
                            Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 1,
                                  }
                                }
                        };
                        return response = new APIResponse { Success = false, Result = payDirectResp };
                    }
                }

                Logger.Error("Could not synchronize payDirects' payment Notification");
                payDirectResp = new PaymentNotificationResponse
                {
                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      Status = 1,
                                      PaymentLogId = payments.FirstOrDefault().PaymentLogId
                                  }
                                }
                };
                return response = new APIResponse { Success = false, Result = payDirectResp };
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
            
        }
        public NetPayPaymentModel GetNetPayModel(EIRSPaymentRequest model)//not used
        {
            string MerchantSecretKey = Configuration.MerchantSecret;
            string MerchantUniqueId = Configuration.MerchantId;

            NetPayRequestModel modelToConvert = new NetPayRequestModel();

            NetPayPaymentModel returnModel = new NetPayPaymentModel();

            //modelToConvert.Amount = model.TotalAmount;
            modelToConvert.Currency = "NGN";
            modelToConvert.MerchantUniqueId = MerchantUniqueId;
            modelToConvert.ReturnUrl = "http://192.168.1.14:2018/api/PaymentNotification";
            modelToConvert.MerchantSecretKey = MerchantSecretKey;
            modelToConvert.TransactionReference = model.PaymentIdentifier;

            var requestDictionary = modelToConvert.ToDictionary().OrderBy(x => x.Key).ToDictionary(k => k.Key, v => v.Value);

            string concatenatedValues = string.Join("", requestDictionary.Select(kvp => string.Format("{0}", kvp.Value)));

            string HMAC = Utils.ComputeHMAC(concatenatedValues, MerchantSecretKey);

            //to be adj
            return new NetPayPaymentModel
            {
                Amount = modelToConvert.Amount,
                MerchantUniqueId = modelToConvert.MerchantUniqueId,
                Currency = modelToConvert.Currency,
                CustomerName = model.TaxPayerName,
                HMAC = HMAC,
                Description = model.Description,
                ReturnUrl = modelToConvert.ReturnUrl,
                TransactionReference = modelToConvert.TransactionReference
            };

        }
        /// <summary>
        /// processes payment notification from NETPAY
        /// </summary>
        /// <param <see cref="PaymentDetails"/> name="paymentDetails"></param>
        /// <returns><see cref="APIResponsev"/></returns>
        public PaymentStatusModel TestProcessNetPayPaymentNotification(string paymentTransactionRef, decimal amountpaid)//not used
        {
            bool saveValue = false;
            try
            {
                Logger.Debug("Trying to process netpay payment notification");
                var paymentHistory = new PaymentHistory();
                var paymentHistoryItems = new List<PaymentHistoryItem>();
                var paymentRequestItemList = new List<EIRSPaymentRequestItem>();

                var paymentRequest = GetPaymentRequestDetails(paymentTransactionRef);
                if (paymentRequest == null)
                {
                    Logger.Error($"Record with payment reference {paymentTransactionRef} not found");
                    //throw new Exception($"Record with payment reference {paymentTransactionRef} not found");
                    return new PaymentStatusModel { Success = false, Message = $"An Error occurred - Record with payment reference { paymentTransactionRef } not found" };
                }

                //determine if it is a pay on account or pay with reference number
                #region customer deposit
                if (string.IsNullOrWhiteSpace(paymentRequest.ReferenceNumber))
                {
                    Logger.Debug("it is a customer deposit (pay on account) payment notification");
                    Logger.Debug("try to save paymentHistory object");

                    //populate the payment History Object
                    paymentHistory = new PaymentHistory
                    {
                        ReferenceNumber = "",
                        AmountPaid = amountpaid,
                        IsSyncedWithEIRS = false,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        PaymentIdentifier = paymentRequest.PaymentIdentifier,
                        TaxPayerName = paymentRequest.TaxPayerName,
                        TaxPayerRIN = paymentRequest.TaxPayerRIN,
                        TaxPayerTIN = paymentRequest.TaxPayerTIN,
                        TaxPayerMobileNumber = paymentRequest.PhoneNumber,
                        TotalAmountPaid = amountpaid,
                        PaymentChannel = PaymentChannel.NetPay.ToString(),
                        SettlementDate = DateTime.Now,
                        IsCustomerDeposit = true,
                        PaymentDate = DateTime.Now,
                    };
                    //save the object
                    _paymentHistoryRepo.Insert(paymentHistory);

                    //update the paymentRequest record
                    paymentRequest.DateModified = DateTime.Now;
                    paymentRequest.IsPaymentSuccessful = true;
                    _eirsPaymentRequestRepo.Update(paymentRequest);
                    Logger.Info($"Successfully updated payment request table");

                    Logger.Info($"Successfully synchronized payment notification from NETPAY ");
                    return new PaymentStatusModel { Success = true, Message = $"Payment Successfully Synchronize for the RIN {paymentRequest.TaxPayerRIN}", TaxPayerName = paymentRequest.TaxPayerName, TotalAmountPaid = paymentRequest.TotalAmountPaid, RIN = paymentRequest.TaxPayerRIN };
                }
                #endregion

                //if it is pay with reference
                #region pay with reference
                Logger.Debug("It is a pay with reference notification");

                //for test sake, get the ref info from the api; and save it to the appropriate tabless
                //first determine if it is assessment or service bill
                var rNo = paymentRequest.ReferenceNumber.Trim();
                var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();

                var paymentDetails = new PaymentDetails
                {
                    AmountPaid = paymentRequest.TotalAmountPaid,
                    DatePaid = DateTime.Now,
                    IsCustomerDeposit = false,
                    PaymentChannel = PaymentChannel.NetPay,
                    ReferenceNumber = rNo,
                    PaymentIdentifier = paymentRequest.PaymentIdentifier,
                };

                //var paymentItems = paymentRequest.PaymentRequestItems;
                //foreach (var item in paymentItems)
                //{
                //    item.AmountPaid = item.ItemAmount;
                //    item.TotalAmountPaid = +item.AmountPaid;
                //    item.DateModified = DateTime.Now;
                //    paymentRequestItemList.Add(item);
                //}
                ////update the paymentRequestItems record
                //_eirsPaymentRequestItemRepo.UpdateRange(paymentRequestItemList);

                //update the paymentRequest record
                paymentRequest.DateModified = DateTime.Now;
                paymentRequest.IsPaymentSuccessful = true;
                _eirsPaymentRequestRepo.Update(paymentRequest);


                //create a paymentHistory Record
                switch (firstTwoChars)
                {
                    case "AB":
                        var assessment = _taxPayerService.ProcessAssessmentDetails(paymentRequest.ReferenceNumber);
                        if (assessment != null && assessment.AssessmentRuleItems.Count() > 0)
                        {
                            saveValue = SaveAssessmentPaymentHistoryInfo(paymentDetails, assessment);
                        }
                        break;
                    case "SB":
                        var serviceBill = _taxPayerService.ProcessServiceBill(paymentRequest.ReferenceNumber);
                        if (serviceBill != null && serviceBill.ServiceBillItems.Count() > 0)
                        {
                            saveValue = SaveServiceBillPaymentHistoryInfo(paymentDetails, serviceBill);
                        }
                        break;
                }
                if (saveValue == true)
                {

                    Logger.Info($"Successfully synchronized payment notification from NETPAY ");
                    return new PaymentStatusModel { Success = true, Message = $"Payment Successfully Synchronize for the Reference Number {paymentRequest.ReferenceNumber}", TaxPayerName = paymentRequest.TaxPayerName, TotalAmountPaid = paymentRequest.TotalAmountPaid, ReferenceNumber = paymentRequest.ReferenceNumber };
                }

                Logger.Error("An error occurred, could not synchronize payment notification");
                return new PaymentStatusModel { Success = false, Message = $"An Error occurred while concluding payment on CBSPay Engine." };


                ////create a paymentHistory object and save to the db
                //paymentHistoryItems = paymentRequestItemList.Select(x => new PaymentHistoryItem()
                //{
                //    AmountPaid = x.AmountPaid,
                //    DateCreated = DateTime.Now,
                //    DateModified = DateTime.Now,
                //    ItemAmount = x.ItemAmount,
                //    ItemDescription = x.ItemDescription,
                //    ItemId = x.ItemId, //AAIID for assessment and SBSIID for serice bill
                //}).ToList();
                ////save the paymentHistoryItem ojects
                //// _paymentHistoryItemRepo.SaveBundle(paymentHistoryItems);

                //Logger.Debug("try to save paymentHistory object");
                //paymentHistory.ReferenceNumber = paymentRequest.ReferenceNumber;
                //paymentHistory.AmountPaid = amountpaid;
                //paymentHistory.DateCreated = DateTime.Now;
                //paymentHistory.DateModified = DateTime.Now;
                //paymentHistory.IsCustomerDeposit = false;
                //paymentHistory.IsSyncedWithEIRS = false;
                //paymentHistory.PaymentIdentifier = paymentRequest.PaymentIdentifier;
                //paymentHistory.TaxPayerMobileNumber = paymentRequest.PhoneNumber;
                //paymentHistory.TaxPayerName = paymentRequest.TaxPayerName;
                //paymentHistory.TaxPayerRIN = paymentRequest.TaxPayerRIN;
                //paymentHistory.TaxPayerTIN = paymentRequest.TaxPayerTIN;
                //paymentHistory.SettlementDate = DateTime.Now;
                //paymentHistory.PaymentItemsHistory = paymentHistoryItems;
                //paymentHistory.PaymentDate = DateTime.Now;
                //paymentHistory.PaymentChannel = PaymentChannel.NetPay.ToString();
                //paymentHistory.ReferenceID = paymentRequest.ReferenceID;
                ////then save the paymentHistoryObject
                //_paymentHistoryRepo.Insert(paymentHistory);


                #endregion

            }
            catch (Exception ex)
            {
                if (saveValue == true)
                {
                    return new PaymentStatusModel { Success = true, Message = $"Payment " };
                }
                Logger.Error("An error occurred, could not synchronize payment notification");
                Logger.Error(ex.StackTrace, ex);
                return new PaymentStatusModel { Success = false, Message = $"An Error occurred while concluding payment on CBSPay Engine. See details {ex.Message}" };
            }
        }
        /// <summary>
        /// processes payment notification from BANKCOLLECT
        /// </summary>
        /// <param <see cref="PaymentDetails"/> name="paymentDetails"></param>
        /// <returns><see cref="APIResponsev"/></returns>
        public APIResponse ProcessBankCollectPaymentNotification(PaymentDetails paymentDetails)//not used
        {
            try
            {
                //for payments with ref number, if assessment items > 1, do split the amount paid across the different items
                Logger.Debug("Processing Bank Collect Payment Notification");

                #region save per each payment history items logic
                var rNo = paymentDetails.ReferenceNumber.Trim();
                var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();
                bool result;

                //for any payment i am notified of, i should have first been called to validate the customer
                //hence, i'll check the assessment table for the record instead of calling the API again
                switch (firstTwoChars)
                {
                    case "AB":
                        Logger.Debug("It is an assessment");
                        var assessmentDetails = _assessmentDetailsRepo.Find(x => x.AssessmentRefNo == rNo);

                        if (assessmentDetails == null)
                        {
                            //not sure whether I should leave this one here or not...removing it for now
                            //EIRSAPIResponse res = _restService.GetAssessmentDetailsByRefNumber(rNo);
                            //assessmentDetails = res.Result;
                            Logger.Error($"Could not find the assessment for the ref no {rNo}");
                            return new APIResponse { ErrorMessage = $"Could not find the assessment for the ref no {rNo}", Success = false, StatusCode = HttpStatusCode.NotFound };
                        }
                        if (assessmentDetails.AssessmentRuleItems.Count() > 0)
                        {
                            Logger.Debug("Trying to process and save asessment details/payment history oject");
                            result = SaveAssessmentPaymentHistoryInfo(paymentDetails, assessmentDetails);
                            if (result == true)
                            {
                                return new APIResponse { ErrorMessage = "", Success = true, StatusCode = HttpStatusCode.OK };
                            }
                            Logger.Error($"An error occurred while saving the payment info, please try again");
                            return new APIResponse { ErrorMessage = $"An error occurred while saving the payment info, please try again", Success = false, StatusCode = HttpStatusCode.NotFound };
                        }

                        Logger.Error($"Could not find the assessment rule items for assessment with the ref no {rNo}");
                        return new APIResponse { ErrorMessage = $"Could not find the assessment rule items for assessment with the ref no {rNo}", Success = false, StatusCode = HttpStatusCode.NotFound };
                    case "SB":
                        Logger.Debug("It is a service bill");
                        var serviceBill = _serviceBillRepo.Find(x => x.ServiceBillRefNo == rNo);
                        if (serviceBill == null)
                        {
                            //not sure whether I should leave this one here or not...removing it for now
                            //EIRSAPIResponse resp = _restService.GetServiceBillDetailsByRefNumber(rNo).Result;
                            //ServiceBillResult serviceBill = resp.Result;
                            Logger.Error($"Could not find the service bill for the ref no {rNo}");
                            return new APIResponse { ErrorMessage = $"Could not find the service bill for the ref no {rNo}", Success = false, StatusCode = HttpStatusCode.NotFound };
                        }
                        if (serviceBill.ServiceBillItems.Count() > 0)
                        {
                            Logger.Debug("Trying to process and save asessment details/payment history oject");
                            result = SaveServiceBillPaymentHistoryInfo(paymentDetails, serviceBill);
                            if (result == true)
                            {
                                return new APIResponse { ErrorMessage = "", Success = true, StatusCode = HttpStatusCode.OK };
                            }
                            //confirm what to do here
                            Logger.Error($"An error occurred while saving the payment info, please try again");
                            return new APIResponse { ErrorMessage = $"An error occurred while saving the payment info, please try again", Success = false, StatusCode = HttpStatusCode.NotFound };
                        }
                        //confirm what to do here
                        Logger.Error($"Could not find the service bill items for service bill with the ref no {rNo}");
                        return new APIResponse { ErrorMessage = $"Could not find the service bill items for service bill with the ref no {rNo}", Success = false, StatusCode = HttpStatusCode.NotFound };


                    default:
                        return new APIResponse { ErrorMessage = $"Invalid reference number - {rNo}", Success = false, StatusCode = HttpStatusCode.BadRequest };
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not synchronize BankCollect payment notification");
                Logger.Error(ex.StackTrace, ex);
                return new APIResponse { ErrorMessage = "An error occurred, could not synchronize payment notification, please try again", Success = false, StatusCode = HttpStatusCode.InternalServerError };
            }
        }
        /// <summary>
        /// method to save service payment history info
        /// </summary>
        /// <param name="paymentDetails"></param>
        /// <param name="serviceBill"></param>
        /// <returns><see cref="bool"/></returns>
        private bool SaveServiceBillPaymentHistoryInfo(PaymentDetails paymentDetails, ServiceBillResult serviceBill)//not really used
        {
            try
            {
                var paymentHistory = new PaymentHistory();
                IEnumerable<PaymentHistoryItem> paymentHistoryItems;
                List<ServiceBillItem> serviceBillItems = new List<ServiceBillItem>();
                var serviceBilItems = serviceBill.ServiceBillItems;
                #region Populate and Save PaymentHistory object

                Logger.InfoFormat("About to save payment history info for service bill with ref number: {0}", paymentDetails.ReferenceNumber);


                //first update the service bill item dettails
                var amountLeft = paymentDetails.AmountPaid;
                foreach (var item in serviceBilItems)
                {
                    if (amountLeft > 0 && amountLeft > item.PendingAmount)
                    {
                        item.AmountPaid = item.PendingAmount;
                        item.PendingAmount = 0;
                        item.SettlementAmount += item.AmountPaid;
                        amountLeft = Convert.ToDecimal(amountLeft) - Convert.ToDecimal(item.PendingAmount);
                        item.DateModified = DateTime.Now;
                    }
                    if (amountLeft > 0 && amountLeft < item.PendingAmount)
                    {
                        item.AmountPaid = amountLeft;
                        item.PendingAmount = item.PendingAmount - amountLeft;
                        item.SettlementAmount += item.AmountPaid;
                        amountLeft = 0;
                        item.DateModified = DateTime.Now;
                    }
                    serviceBillItems.Add(item);
                }

                //if there is still some money left, add it to the first item.
                if (amountLeft > 0)
                {
                    serviceBillItems.First().AmountPaid += amountLeft;
                }
                _serviceBillItemRepo.UpdateRange(serviceBillItems);
                Logger.Debug("Successfully updated service bill rule items");

                Logger.Debug("Save Payment History Items");
                paymentHistoryItems = serviceBilItems.Select(x => new PaymentHistoryItem
                {
                    AmountPaid = x.AmountPaid,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    ItemAmount = x.ServiceAmount,
                    ItemDescription = x.MDAServiceItemName,
                    ItemId = x.SBSIID, //not sure of which to use, so i am using SBSSIID, this is liable to change
                    //PaymentHistory = paymentHistory
                }).ToList();

                // _paymentHistoryItemRepo.SaveBundle(paymentHistoryItems);

                //populate PaymentHistory object
                Logger.Info("Populating PaymentHistory object");
                //confirm if i need to do anyother thing here before saving
                paymentHistory = new PaymentHistory
                {
                    ReferenceAmount = paymentDetails.ReferenceAmount,
                    AmountPaid = paymentDetails.AmountPaid,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    TotalAmountPaid = paymentDetails.AmountPaid,
                    PaymentDate = paymentDetails.DatePaid,
                    PaymentChannel = paymentDetails.PaymentChannel.ToString(),
                    IsDeleted = false,
                    ReferenceNumber = paymentDetails.ReferenceNumber,
                    TaxPayerMobileNumber = paymentDetails.PhoneNumber,
                    TaxPayerName = paymentDetails.TaxPayerName,
                    PaymentIdentifier = $"{paymentDetails.PaymentChannel.ToString()}_{paymentDetails.ReferenceNumber}_{DateTime.Now.Day}_{DateTime.Now.TimeOfDay}",
                    IsCustomerDeposit = false,
                    IsSyncedWithEIRS = false,
                    SettlementDate = DateTime.Now,
                    PaymentLogId = string.IsNullOrWhiteSpace(paymentDetails.PaymentIdentifier) ? 0 : Convert.ToInt32(paymentDetails.PaymentIdentifier),
                    PaymentReference = paymentDetails.PaymentReference,
                    ReceiptNo = paymentDetails.ReceiptNo,
                    PaymentItemsHistory = paymentHistoryItems
                };
                //save the record in the db
                Logger.Info("Saving PaymentHistory object");
                _paymentHistoryRepo.Insert(paymentHistory);

                #endregion
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not save the service bill-related payment history record");
                Logger.Error(ex.StackTrace, ex);
                return false;
            }
        }
        /// <summary>
        /// method to save assessment payment history info
        /// </summary>
        /// <param name="paymentDetails"></param>
        /// <param name="assessmentDetails"></param>
        /// <returns><see cref="bool"/></returns>
        private bool SaveAssessmentPaymentHistoryInfo(PaymentDetails paymentDetails, AssessmentDetailsResult assessmentDetails)//not really used
        {
            try
            {
                Logger.Debug($"Trying to save assessment payment history info for {assessmentDetails.AssessmentRefNo}");
                var paymentHistory = new PaymentHistory();
                IEnumerable<PaymentHistoryItem> paymentHistoryItems;
                List<AssessmentRuleItem> assessmentRuleItems = new List<AssessmentRuleItem>();
                var assessmentItems = assessmentDetails.AssessmentRuleItems;
                //var assessmentItems = _assessmentRuleItemRepo.Fetch(x => x.AssessmentItemReferenceNo == assessmentDetails.AssessmentRefNo);
                #region Populate and Save PaymentHistory object

                Logger.InfoFormat("About to save payment history info for asessment with ref number: {0}", paymentDetails.ReferenceNumber);

                //first update the assessment dettails
                var amountLeft = paymentDetails.AmountPaid;
                if (assessmentItems.Count() > 0)
                {

                    foreach (var item in assessmentItems)
                    {
                        if (amountLeft > 0 && amountLeft > item.PendingAmount)
                        {
                            item.AmountPaid = item.PendingAmount;
                            item.PendingAmount = 0;
                            item.SettlementAmount += item.AmountPaid;
                            amountLeft = amountLeft - item.AmountPaid;
                            item.DateModified = DateTime.Now;
                        }
                        if (amountLeft > 0 && amountLeft < item.PendingAmount)
                        {
                            item.AmountPaid = amountLeft;
                            item.PendingAmount = item.PendingAmount - amountLeft;
                            item.SettlementAmount += item.AmountPaid;
                            amountLeft = 0;
                            item.DateModified = DateTime.Now;
                        }
                        assessmentRuleItems.Add(item);
                    }

                    //if there is still some money left, add it to the first item.
                    if (amountLeft > 0)
                    {
                        assessmentRuleItems.First().AmountPaid += amountLeft;
                    }

                    _assessmentRuleItemRepo.UpdateRange(assessmentRuleItems);
                    Logger.Debug("Successfully updated assessment rule items");

                    Logger.Debug("Populate Payment History Items");
                    paymentHistoryItems = assessmentItems.Select(x => new PaymentHistoryItem
                    {
                        AmountPaid = x.AmountPaid,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        ItemAmount = x.TaxAmount,
                        ItemDescription = x.AssessmentItemName,
                        ItemId = x.AAIID, //not sure of which to use, so i am using AAIID, this is liable to change
                        //PaymentHistory = paymentHistory
                    }).ToList();

                    _paymentHistoryItemRepo.SaveBundle(paymentHistoryItems);

                    //populate PaymentHistory object
                    Logger.Info("Populating PaymentHistory object");
                    //confirm if i need to do any other thing here before saving
                    paymentHistory = new PaymentHistory
                    {
                        ReferenceAmount = paymentDetails.ReferenceAmount,
                        AmountPaid = paymentDetails.AmountPaid,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        TotalAmountPaid = paymentDetails.AmountPaid,
                        PaymentDate = paymentDetails.DatePaid,
                        PaymentChannel = paymentDetails.PaymentChannel.ToString(),
                        IsDeleted = false,
                        ReferenceNumber = paymentDetails.ReferenceNumber,
                        TaxPayerMobileNumber = paymentDetails.PhoneNumber,
                        TaxPayerName = paymentDetails.TaxPayerName,
                        PaymentIdentifier = $"{paymentDetails.PaymentChannel.ToString()}_{paymentDetails.ReferenceNumber}_{DateTime.Now.Day}_{DateTime.Now.TimeOfDay}",
                        IsCustomerDeposit = false,
                        IsSyncedWithEIRS = false,
                        SettlementDate = DateTime.Now,
                        PaymentLogId = string.IsNullOrWhiteSpace(paymentDetails.PaymentIdentifier) ? 0 : Convert.ToInt32(paymentDetails.PaymentIdentifier),
                        PaymentReference = paymentDetails.PaymentReference,
                        PaymentItemsHistory = paymentHistoryItems,
                        ReceiptNo = paymentDetails.ReceiptNo
                    };
                    //save the record in the db
                    Logger.Info("Saving PaymentHistory object");
                    _paymentHistoryRepo.Insert(paymentHistory);
                    Logger.Debug($"Successfully  saved payment history object for {assessmentDetails.AssessmentRefNo}");
                    #endregion
                    return true;
                }
                Logger.Error("An error occurred, could not save the assessment-related payment history record");
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not save the assessment-related payment history record");
                Logger.Error(ex.StackTrace, ex);
                return false;
            }

        }
        public PaymentStatusModel ProcessQuicktellerWebPayment(string requestReference, decimal trueAmount)//not used
        {
            //return null;
            try
            {
                Logger.Debug("Trying to process netpay payment notification");
                var paymentHistory = new PaymentHistory();
                var paymentHistoryItems = new List<PaymentHistoryItem>();
                var paymentRequestItemList = new List<EIRSPaymentRequestItem>();

                var paymentRequest = GetPaymentRequestDetails(requestReference);
                if (paymentRequest == null)
                {
                    Logger.Error($"Record with payment reference {requestReference} not found");
                    //throw new Exception($"Record with payment reference {paymentTransactionRef} not found");
                    return new PaymentStatusModel { Success = false, Message = $"An Error occurred - Record with payment reference { requestReference } not found" };
                }

                //determine if it is a pay on account or pay with reference number
                #region customer deposit
                if (string.IsNullOrWhiteSpace(paymentRequest.ReferenceNumber))
                {
                    Logger.Debug("it is a customer deposit (pay on account) payment notification");
                    Logger.Debug("try to save paymentHistory object");

                    //populate the payment History Object
                    paymentHistory = new PaymentHistory
                    {
                        ReferenceNumber = "",
                        AmountPaid = trueAmount == 0 ? paymentRequest.TotalAmountPaid : trueAmount, //this is because net pay returns 0 for now
                        IsSyncedWithEIRS = false,
                        DateCreated = DateTime.Now,
                        DateModified = DateTime.Now,
                        PaymentIdentifier = paymentRequest.PaymentIdentifier,
                        TaxPayerName = paymentRequest.TaxPayerName,
                        TaxPayerRIN = paymentRequest.TaxPayerRIN,
                        TaxPayerTIN = paymentRequest.TaxPayerTIN,
                        TaxPayerMobileNumber = paymentRequest.PhoneNumber,
                        TotalAmountPaid = trueAmount == 0 ? paymentRequest.TotalAmountPaid : trueAmount, //this is because net pay returns 0 for now
                        PaymentChannel = PaymentChannel.NetPay.ToString(),
                        SettlementDate = DateTime.Now,
                        IsCustomerDeposit = true,
                        PaymentDate = DateTime.Now,
                        TaxPayerID = paymentRequest.TaxPayerID,
                        TaxPayerTypeID = paymentRequest.TaxPayerTypeID,
                        EconomicActivity = paymentRequest.EconomicActivity,
                        Email = paymentRequest.Email,
                        RevenueStream = paymentRequest.RevenueStream,
                        RevenueSubStream = paymentRequest.RevenueSubStream,
                        OtherInformation = paymentRequest.OtherInformation,
                        Address = paymentRequest.Address,
                        TaxPayerType = paymentRequest.TaxPayerType
                    };
                    //save the object
                    _paymentHistoryRepo.Insert(paymentHistory);

                    //update the paymentRequest record
                    paymentRequest.DateModified = DateTime.Now;
                    paymentRequest.IsPaymentSuccessful = true;
                    _eirsPaymentRequestRepo.Update(paymentRequest);
                    Logger.Info($"Successfully updated payment request table");

                    Logger.Info($"Successfully synchronized payment notification from NETPAY ");
                    return new PaymentStatusModel { Success = true, Message = $"Payment Successfully Synchronize for the TaxPayer -  {paymentRequest.TaxPayerName}", TaxPayerName = paymentRequest.TaxPayerName, TotalAmountPaid = paymentHistory.AmountPaid, RIN = paymentRequest.TaxPayerRIN };
                }
                #endregion

                //if it is pay with reference
                #region pay with reference
                Logger.Debug("It is a pay with reference notification");

                var paymentItems = paymentRequest.PaymentRequestItems;
                foreach (var item in paymentItems)
                {
                    item.AmountPaid = item.AmountPaid;
                    item.TotalAmountPaid = +item.AmountPaid;
                    item.DateModified = DateTime.Now;
                    paymentRequestItemList.Add(item);
                }
                //update the paymentRequestItems record
                _eirsPaymentRequestItemRepo.UpdateRange(paymentRequestItemList);

                //update the paymentRequest record
                paymentRequest.DateModified = DateTime.Now;
                paymentRequest.IsPaymentSuccessful = true;
                _eirsPaymentRequestRepo.Update(paymentRequest);

                //create a paymentHistory object and save to the db
                paymentHistoryItems = paymentRequestItemList.Select(x => new PaymentHistoryItem()
                {
                    AmountPaid = x.AmountPaid,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    ItemAmount = x.ItemAmount,
                    ItemDescription = x.ItemDescription,
                    ItemId = x.ItemId, //AAIID for assessment and SBSIID for serice bill
                }).ToList();
                //save the paymentHistoryItem ojects
                _paymentHistoryItemRepo.SaveBundle(paymentHistoryItems);

                Logger.Debug("try to save paymentHistory object");
                paymentHistory.ReferenceNumber = paymentRequest.ReferenceNumber;
                paymentHistory.AmountPaid = trueAmount == 0 ? paymentRequest.TotalAmountPaid : trueAmount; //this is because net pay returns 0 for now
                paymentHistory.DateCreated = DateTime.Now;
                paymentHistory.DateModified = DateTime.Now;
                paymentHistory.IsCustomerDeposit = false;
                paymentHistory.IsSyncedWithEIRS = false;
                paymentHistory.PaymentIdentifier = paymentRequest.PaymentIdentifier;
                paymentHistory.TaxPayerMobileNumber = paymentRequest.PhoneNumber;
                paymentHistory.TaxPayerName = paymentRequest.TaxPayerName;
                paymentHistory.TaxPayerRIN = paymentRequest.TaxPayerRIN;
                paymentHistory.TaxPayerTIN = paymentRequest.TaxPayerTIN;
                paymentHistory.SettlementDate = DateTime.Now;
                paymentHistory.PaymentItemsHistory = paymentHistoryItems;
                paymentHistory.PaymentDate = DateTime.Now;
                paymentHistory.PaymentChannel = PaymentChannel.NetPay.ToString();
                paymentHistory.ReferenceID = paymentRequest.ReferenceID;
                paymentHistory.TotalAmountPaid += paymentRequest.TotalAmountPaid;
                //then save the paymentHistoryObject
                _paymentHistoryRepo.Insert(paymentHistory);

                foreach (var item in paymentHistoryItems)
                {
                    item.PaymentHistory = paymentHistory;
                }
                _paymentHistoryItemRepo.UpdateRange(paymentHistoryItems);

                Logger.Info($"Successfully synchronized payment notification from NETPAY ");
                return new PaymentStatusModel { Success = true, Message = $"Payment Successfully Synchronize for the Reference Number {paymentRequest.ReferenceNumber}", TaxPayerName = paymentRequest.TaxPayerName, TotalAmountPaid = paymentHistory.AmountPaid, ReferenceNumber = paymentRequest.ReferenceNumber };
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not synchronize payment notification");
                Logger.Error(ex.StackTrace, ex);
                return new PaymentStatusModel { Success = false, Message = $"An Error occurred while concluding payment on CBSPay Engine. See details {ex.Message}" };
            }
        }
        /// <summary>
        /// Saves Payment Request Info from EIRS Web (for Pay With Reference) and returns a payment identifier
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="string"/></returns>
        public string SaveEIRSBillRefPaymentRequestInfo(EIRSPaymentRequestInfo model)
        {
            EIRSPaymentRequest paymentRequest;
            var paymentRequestItemList = new List<EIRSPaymentRequestItem>();
            try
            {
                Logger.Debug($"About to save EIRS Bill Ref Payment Request Data for {model.RefNumber}");

                //var refNo = string.IsNullOrWhiteSpace(model.ReferenceNumber) ? string.IsNullOrWhiteSpace(model.TaxPayerRIN) ? model.PhoneNumber : model.TaxPayerRIN : model.ReferenceNumber;
                var refNo = model.RefNumber;
                //generate identifier
                var ident = Utils.GetPaymentIdentifier( $"{model.RefRules.FirstOrDefault().RuleItemRef}_{ model.SettlementMethod}", refNo);

                //populate DAO object

                //first save the paymentRequestItems
                Logger.Debug("Saving the payment request items.");
                //if (model.PaymentRequestItems != null)
                if (model.RefRules != null)
                {
                    //foreach (var paymentRequestItem in model.PaymentRequestItems)
                    foreach (var paymentRequestItem in model.RefRules)
                    {
                        paymentRequestItemList.Add(new EIRSPaymentRequestItem
                        {
                            ItemId = paymentRequestItem.TBPKID,
                            //ItemAmount = paymentRequestItem.ItemAmount,
                            //ItemDescription = paymentRequestItem.ItemDescription,
                            AmountPaid = paymentRequestItem.AmountPaid,
                            OutstandingAmount = paymentRequestItem.OutstandingAmount,
                            RefRuleID = paymentRequestItem.RefRuleID,
                            RuleItemRef = paymentRequestItem.RuleItemRef,
                            RuleAmount = paymentRequestItem.RuleAmount,
                            RuleAmountToPay = paymentRequestItem.RuleAmountToPay,
                            RuleComputation = paymentRequestItem.RuleComputation,
                            RuleID = paymentRequestItem.RuleID,
                            RuleItemID = paymentRequestItem.RuleItemID,
                            RuleItemName = paymentRequestItem.RuleItemName,
                            RuleName = paymentRequestItem.RuleName,
                            SettledAmount = paymentRequestItem.SettledAmount,
                            TaxYear = paymentRequestItem.TaxYear,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now,
                            IsDeleted = false,
                            
                        });
                    }
                    _eirsPaymentRequestItemRepo.SaveBundle(paymentRequestItemList);
                }

                //var totalAmountPaid = model.RefRules.Sum(item => Convert.ToDecimal(item.RuleAmountToPay));
                var totalAmountPaid = model.TotalAmountToPay;
                //then save the paymentRequestInfo
                Logger.Debug("Saving the payemnt request info");
                paymentRequest = new EIRSPaymentRequest
                {
                    PhoneNumber = model.PhoneNumber,
                    TaxPayerTIN = model.TaxPayerTIN,
                    TaxPayerName = model.TaxPayerName,
                    PaymentIdentifier = ident,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Description = $"Payment for assessment with reference number {refNo} that has {paymentRequestItemList.Count()} item(s)",
                    ReferenceNumber = refNo,
                    TaxPayerRIN = model.TaxPayerRIN,
                    TotalAmountPaid = totalAmountPaid,
                    ReferenceID = model.ReferenceId,
                    TemplateType = model.TemplateType,
                    AddNotes = model.AddNotes,
                    ReferenceDate = model.Date,
                    SettlementMethod = model.SettlementMethod,
                    SettlementStatusName = model.SettlementStatus,
                    ReferenceNotes = model.Notes,
                    TotalAmount = model.TotalAmount,
                    TotalOutstandingAmount = model.TotalOutstandingAmount,
                    TotalAmountToPay = totalAmountPaid,
                    PaymentRequestItems = paymentRequestItemList,
                    
                };
                Logger.Debug($"Payment for assessment with reference number {refNo} that has {paymentRequestItemList.Count()} item(s)");
                //save to db
                _eirsPaymentRequestRepo.Insert(paymentRequest);
                Logger.Debug($"Successfully saved payment request info for {model.RefNumber} ");
                //is this necessary?
                foreach (var item in paymentRequestItemList)
                {
                    item.PaymentRequest = paymentRequest;
                }
               
                _eirsPaymentRequestItemRepo.UpdateRange(paymentRequestItemList);
                Logger.Debug($"Successfully updated payment request info for {model.RefNumber} ");
                //return identifier
                return paymentRequest.PaymentIdentifier;

            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred - could not save payment request information at this time");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        /// <summary>
        /// Update Payment History Table after Notifying EIRS of payment
        /// </summary>
        /// <param <see cref="PaymentHistory"/> name="paymentIdentifier"></param>
        /// <returns></returns>
        public void UpdatePaymentHistoryRecords(PaymentHistory record)
        {
            try
            {
                Logger.Debug("Trying to update the Payment History Object");
                record.IsSyncedWithEIRS = true;
                record.DateModified = DateTime.Now;
                //update the record in the db
                Logger.Info("Updating PaymentHistory object");
                _paymentHistoryRepo.Update(record);
                Logger.Info("Successfully updated PaymentHistory object");
                foreach (var item in record.PaymentItemsHistory)
                {
                    item.PaymentHistory = record;
                }
                _paymentHistoryItemRepo.UpdateRange(record.PaymentItemsHistory);
            }
            catch (Exception ex)
            {
                Logger.Error("Could not update the Payment History Object");
                Logger.Error(ex.StackTrace, ex);
                try
                {
                    Logger.Error("trying again");
                    UpdatePaymentHistoryRecords(record);
                }
                catch (Exception)
                {
                    Logger.Error("Could not update the Payment History Object on retry");
                    throw;
                }
                throw;
            }
        }
        /// <summary>
        /// Gets a list of Payment History records from the db that has not been synchronized
        /// for EIRS Settlement
        /// </summary>
        /// <returns><see cref="IEnumerable<PaymentHistory>"/></returns>
        public IEnumerable<PaymentHistory> GetUnsyncedPaymentRecords()
        {
            try
            {
                Logger.Debug("Trying to fetch unsynced Payment History Records");
                var res = _paymentHistoryRepo.Fetch(x => x.IsSyncedWithEIRS == false).ToList();
                Logger.Debug("Successfully fetched unsynced Payment History Records from the db");
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error("Could not retrieve unsynced Payment History Records from the DB");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        /// <summary>
        /// processes payment notification 
        /// </summary>
        /// <param name="amountpaid"></param>
        /// <param  name="paymentTransactionRef"></param>
        /// <returns><see cref="PaymentStatusModel"/></returns>
        public PaymentStatusModel ProcessWebPaymentNotification(string paymentTransactionRef, decimal amountpaid, string responseModel)
        {
            try
            {
                Logger.Debug("Trying to process web payment notification");
                var paymentHistory = new PaymentHistory();
                var paymentHistoryItems = new List<PaymentHistoryItem>();
                var paymentRequestItemList = new List<EIRSPaymentRequestItem>();

                Logger.Debug($"Find payment request details for transaction ref - {paymentTransactionRef}");
                var paymentRequest = GetPaymentRequestDetails(paymentTransactionRef);

                if (paymentRequest == null)
                {
                    Logger.Error($"Record with payment reference {paymentTransactionRef} not found");
                    //throw new Exception($"Record with payment reference {paymentTransactionRef} not found");
                    return new PaymentStatusModel { Success = false, Message = $"An Error occurred - Record with payment reference { paymentTransactionRef } not found" };
                }

                //determine if it is a pay with reference number or not
                if (!string.IsNullOrWhiteSpace(paymentRequest.ReferenceNumber))
                {
                //if it is pay with reference
                #region pay with reference
                Logger.Debug("It is a pay with reference notification");

                var paymentItems = paymentRequest.PaymentRequestItems;
                if (paymentItems.Count() <= 0)
                {
                    Logger.Error("Could not fetch the payment request items of the payment object");
                    return new PaymentStatusModel { Success = false, Message = "Payment request items returned null" };
                }

                foreach (var item in paymentItems)
                {
                    item.AmountPaid = item.AmountPaid;
                    item.TotalAmountPaid = +item.AmountPaid;
                    item.DateModified = DateTime.Now;
                    paymentRequestItemList.Add(item);
                }
                //update the paymentRequestItems record
                _eirsPaymentRequestItemRepo.UpdateRange(paymentRequestItemList);
                Logger.Debug("Updated payment request items.");

                //update the paymentRequest record
                paymentRequest.DateModified = DateTime.Now;
                paymentRequest.IsPaymentSuccessful = true;
                _eirsPaymentRequestRepo.Update(paymentRequest);
                Logger.Debug("Updated payment request object.");


                // item amount to pay
                //create a paymentHistory object and save to the db
                Logger.Debug("create a paymentHistory object and save to the db");

                Logger.Debug("first,create a paymentHistory items and save to the db");

                paymentHistoryItems = paymentRequestItemList.Select(x => new PaymentHistoryItem()
                {
                    AmountPaid = x.RuleAmountToPay,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    ItemAmount = x.OutstandingAmount,
                    ItemDescription = x.ItemDescription,
                    ItemId = x.ItemId, //AAIID for assessment and SBSIID for serice bill
                }).ToList();

                Logger.Debug("try to save paymentHistory object");
                paymentHistory.ReferenceNumber = paymentRequest.ReferenceNumber;
                paymentHistory.AmountPaid = amountpaid == 0 ? paymentRequest.TotalAmountPaid : amountpaid; //this is because net pay returns 0 for now
                paymentHistory.DateCreated = DateTime.Now;
                paymentHistory.DateModified = DateTime.Now;
                paymentHistory.IsCustomerDeposit = false;
                paymentHistory.IsSyncedWithEIRS = false;
                paymentHistory.PaymentIdentifier = paymentRequest.PaymentIdentifier;
                paymentHistory.TaxPayerMobileNumber = paymentRequest.PhoneNumber;
                paymentHistory.TaxPayerName = paymentRequest.TaxPayerName;
                paymentHistory.TaxPayerRIN = paymentRequest.TaxPayerRIN;
                paymentHistory.TaxPayerTIN = paymentRequest.TaxPayerTIN;
                paymentHistory.SettlementDate = DateTime.Now;
                paymentHistory.PaymentItemsHistory = paymentHistoryItems;
                paymentHistory.PaymentDate = DateTime.Now;
                paymentHistory.PaymentChannel = PaymentChannel.NetPay.ToString();
                paymentHistory.ReferenceID = paymentRequest.ReferenceID;
                paymentHistory.TotalAmountPaid += paymentRequest.TotalAmountPaid;
                paymentHistory.OtherInformation = responseModel;
                paymentHistory.Trials = 0;
                //then save the paymentHistoryObject
                _paymentHistoryRepo.Insert(paymentHistory);
                //save the paymentHistoryItem ojects
                //_paymentHistoryItemRepo.SaveBundle(paymentHistoryItems);
                foreach (var item in paymentHistoryItems)
                {
                    item.PaymentHistory = paymentHistory;
                }
                _paymentHistoryItemRepo.UpdateRange(paymentHistoryItems);

                    //when web payment channel becomes more than one, this should be updated to reflect where the payment notification was made from
                Logger.Info($"Successfully synchronized payment notification ");
                return new PaymentStatusModel { Success = true, Message = $"Payment Successfully Synchronize for the Reference Number {paymentRequest.ReferenceNumber}", TaxPayerName = paymentRequest.TaxPayerName, TotalAmountPaid = paymentHistory.AmountPaid, ReferenceNumber = paymentRequest.ReferenceNumber };
                    #endregion
                }
                
                #region customer deposit

                Logger.Debug("it is a customer deposit (pay on account) payment notification");
                    Logger.Debug("try to save paymentHistory object");

                //populate the payment History Object
                paymentHistory = new PaymentHistory
                {
                    ReferenceNumber = "",
                    AmountPaid = amountpaid == 0 ? paymentRequest.TotalAmountPaid : amountpaid, //this is because net pay returns 0 for now
                    IsSyncedWithEIRS = false,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    PaymentIdentifier = paymentRequest.PaymentIdentifier,
                    TaxPayerName = paymentRequest.TaxPayerName,
                    TaxPayerRIN = paymentRequest.TaxPayerRIN,
                    TaxPayerTIN = paymentRequest.TaxPayerTIN,
                    TaxPayerMobileNumber = paymentRequest.PhoneNumber,
                    TotalAmountPaid = amountpaid == 0 ? paymentRequest.TotalAmountPaid : amountpaid, //this is because net pay returns 0 for now
                    PaymentChannel = PaymentChannel.NetPay.ToString(),
                    SettlementDate = DateTime.Now,
                    IsCustomerDeposit = true,
                    PaymentDate = DateTime.Now,
                    TaxPayerID = paymentRequest.TaxPayerID,
                    TaxPayerTypeID = paymentRequest.TaxPayerTypeID,
                    EconomicActivity = paymentRequest.EconomicActivity,
                    Email = paymentRequest.Email,
                    RevenueStream = paymentRequest.RevenueStream,
                    RevenueSubStream = paymentRequest.RevenueSubStream,
                    OtherInformation = paymentRequest.OtherInformation,
                    Address = paymentRequest.Address,
                    TaxPayerType = paymentRequest.TaxPayerType,
                    SettlementMethodID = paymentRequest.SettlementMethod,
                    Trials = 0
                };
                    //save the object
                    _paymentHistoryRepo.Insert(paymentHistory);

                    //update the paymentRequest record
                    paymentRequest.DateModified = DateTime.Now;
                    paymentRequest.IsPaymentSuccessful = true;
                    _eirsPaymentRequestRepo.Update(paymentRequest);
                    Logger.Info($"Successfully updated payment request table");

                    Logger.Info($"Successfully synchronized payment notification from NETPAY ");
                    return new PaymentStatusModel { Success = true, Message = $"Payment Successfully Synchronize for the TaxPayer -  {paymentRequest.TaxPayerName}", TaxPayerName = paymentRequest.TaxPayerName, TotalAmountPaid = paymentHistory.AmountPaid, RIN = paymentRequest.TaxPayerRIN };
                
                #endregion

               

            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not synchronize payment notification");
                Logger.Error(ex.StackTrace, ex);
                return new PaymentStatusModel { Success = false, Message = $"An Error occurred while concluding payment on CBSPay Engine. See details {ex.Message}" };
            }

        }
        /// <summary>
        /// Get payment reequest details based on the payment ref
        /// </summary>
        /// <param name="paymentRef"></param>
        /// <returns></returns>
        public EIRSPaymentRequest GetPaymentRequestDetails(string paymentRef)
        {
            try
            {
                Logger.Debug($"About to fetch Payment Request Details for paymentRef - {paymentRef}");
                var paymentRequest = _eirsPaymentRequestRepo.Find(x => x.PaymentIdentifier == paymentRef);
                return paymentRequest;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not fetch Payment Request Details");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        /// <summary>
        /// Get assessment details usiing the ref number
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        public AssessmentDetailsResult GetAssessmentDetails(string referenceNumber)
        {
            try
            {
                Logger.Debug("trying to get assessment details using the ref number");
                var res = _assessmentDetailsRepo.Find(x => x.AssessmentRefNo == referenceNumber);
                Logger.Debug("Successfully fetched the assessment details");
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not fetch assessment details");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        /// <summary>
        /// Get service bill details usiing the ref number
        /// </summary>
        /// <param name="referenceNumber"></param>
        /// <returns></returns>
        public ServiceBillResult GetServiceBillDetails(string referenceNumber)
        {
            try
            {
                Logger.Debug("trying to get service bill details using the ref number");
                var res = _serviceBillRepo.Find(x => x.ServiceBillRefNo == referenceNumber);
                Logger.Debug("Successfully got the service bill details");
                return res;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred, could not fetch service bill details");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        /// <summary>
        /// saves EIRS Pay On Account Payment Request, returns identifier if successful and null if not successful
        /// </summary>
        /// <param name="RIN"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="AmountPaid"></param>
        /// <returns>PaymentIdentifier<see cref="string"/></returns>
        public string SaveEIRSPOAPaymentRequestInfo(EIRSPaymentRequestInfo model)
        {
            EIRSPaymentRequest paymentRequest;
            try
            {
                Logger.Debug("About to save EIRS Payment Request Data for Pay on Account");

                if (model.TaxPayerRIN == null)
                {
                    return null;
                }

                //generate identifier
                var ident = Utils.GetPaymentIdentifier("EIRSWebPayment", model.TaxPayerRIN);

                //populate DAO object
                paymentRequest = new EIRSPaymentRequest
                {
                    PhoneNumber = model.PhoneNumber,
                    PaymentIdentifier = ident,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Description = $"Payment for assessment with RIN {model.TaxPayerRIN}",
                    TaxPayerRIN = model.TaxPayerRIN,
                    TotalAmountPaid = model.AmountPaid,
                    TaxPayerID = model.TaxPayerID,
                    TaxPayerTypeID = model.TaxPayerTypeID,
                    TaxPayerName = model.TaxPayerName,
                    ReferenceDate = SqlDateTime.MinValue.Value,
                    OtherInformation = model.OtherInformation,
                    RevenueStream = model.RevenueStream,
                    RevenueSubStream = model.RevenueSubStream,
                    
                };

                //save to db
                Logger.Debug("trying to save POA payment notification ");
                _eirsPaymentRequestRepo.Insert(paymentRequest);
                //return identifier
                Logger.Debug("successfully saved POA payment notification ");
                return paymentRequest.PaymentIdentifier;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred - could not save payment request information at this time");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        /// <summary>
        /// Synchronize pay On Account Payment Notification from Pay Direct
        /// </summary>
        /// <param name="payments"><see cref="List<Payment>"/></param>
        /// <returns></returns>
        public APIResponse SynchronizePayDirectPOAPayments(List<Payment> payments)
        {
            //variables
            var paymentHistory = new PaymentHistory();
            var paymentInfo = new PaymentHistory();
            var payDirectResp = new PaymentNotificationResponse();
            APIResponse response;
            try
            {
                Logger.Debug("About to Synchronize PayDirect's Pay On Account Payment Notification");

                //begin synchronization
                foreach (var payment in payments)
                {
                    try
                    {
                        #region Populate and Save/Update PaymentHistory object

                        //custReference is {RIN-MobileNumber}
                        // Logger.Debug("Split custReference to get RIN and phone Number ");
                        var RIN = Utils.GetRINFromCustReference(payment.CustReference);
                        var phoneNumber = Utils.GetPhoneNumberFromCustReference(payment.CustReference);

                        //check if it is a reversal
                        #region reversal call
                        if (payment.IsReversal != null)
                        {
                            if (string.Equals(payment.IsReversal.ToString(), "True", StringComparison.OrdinalIgnoreCase))
                            {
                                Logger.DebugFormat("This is a reversal payment notification for customer with CustReference: {0} and payment reference: {1}", payment.CustReference, payment.PaymentReference);
                                paymentInfo = _paymentHistoryRepo.Find(x => x.PaymentReference == payment.OriginalPaymentReference && x.PaymentLogId == Convert.ToInt32(payment.OriginalPaymentLogId));
                                //if not repeated reversal
                                if (paymentInfo == null)
                                {

                                    Logger.Error($"The reversal payment notification for customer with ref number: { payment.CustReference} should be sent with a new payment reference and payment log Id");
                                    //return status 1 
                                    payDirectResp = new PaymentNotificationResponse
                                    {
                                        Payments = new List<PaymentResponse>
                                        {
                                          new PaymentResponse
                                          {
                                              PaymentLogId = payment.PaymentLogId,
                                              Status = 1,
                                          }
                                        }
                                    };

                                    return response = new APIResponse { Success = true, Result = payDirectResp };                                    
                                }
                                if (paymentInfo != null)
                                {
                                    
                                    paymentInfo.PaymentReference = payment.PaymentReference;
                                    paymentInfo.OriginalPaymentReference = payment.OriginalPaymentReference;
                                    paymentInfo.PaymentLogId = Convert.ToInt32(payment.PaymentLogId);
                                    paymentInfo.TotalAmountPaid = paymentInfo.AmountPaid + payment.Amount;
                                    paymentInfo.OriginalPaymentLogId = Convert.ToInt32(payment.OriginalPaymentLogId);
                                    paymentInfo.AmountPaid = payment.Amount;
                                    paymentInfo.DateModified = DateTime.Now;
                                    paymentInfo.IsSyncedWithEIRS = false;
                                    paymentInfo.SettlementAmount = payment.Amount;
                                    //leave as empty for now till we confirm how we want to go about customer deposit for paydirect
                                    paymentInfo.TaxPayerTIN = "";
                                    paymentInfo.IsCustomerDeposit = true;
                                    paymentInfo.TaxPayerRIN = RIN;
                                    paymentInfo.TaxPayerName = "";
                                    paymentInfo.TaxPayerMobileNumber = "";
                                    paymentInfo.ReceiptNo = payment.ReceiptNo;
                                    paymentInfo.PaymentDate = Convert.ToDateTime(payment.PaymentDate);
                                    paymentInfo.SettlementDate = Convert.ToDateTime(payment.SettlementDate);
                                    //update the record in the db
                                    Logger.Info("Updating PaymentHistory object");
                                    _paymentHistoryRepo.Update(paymentInfo);
                                    //return status 0
                                    payDirectResp = new PaymentNotificationResponse
                                    {
                                        Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 0,
                                      //StatusMessage = "Reversal Payment Notification Successful"
                                  }
                                }
                                    };
                                    Logger.DebugFormat("The reversal payment notification for customer with CustReference: {0}  payment reference: {1} has already been synchronized", payment.CustReference, payment.PaymentReference);

                                    return response = new APIResponse { Success = true, Result = payDirectResp };

                                }
                            }
                        }

                        #endregion


                        #region duplicate call
                        //check if it is a duplicate call
                        if (payment.IsRepeated != null)
                        {
                            if (string.Equals(payment.IsRepeated.ToString(), "True", StringComparison.OrdinalIgnoreCase))
                            {
                                paymentInfo = _paymentHistoryRepo.Find(x => x.PaymentReference == payment.PaymentReference && x.ReceiptNo == payment.ReceiptNo && x.PaymentLogId == Convert.ToInt32(payment.PaymentLogId) && x.AmountPaid == payment.Amount);
                                if (paymentInfo != null)
                                {
                                    //return 0 and add a status message stating that the payment notification has been synchronized before
                                    payDirectResp = new PaymentNotificationResponse
                                    {
                                        Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 0,
                                      //StatusMessage = $"This payment notification has already been synchronized"
                                  }
                                }
                                    };
                                    //Logger.Debug("Serialize the object to a string");
                                    //responseString = Utils.SerializeToXML<PaymentNotificationResponse>(payDirectResp);
                                    return response = new APIResponse { Success = true, Result = payDirectResp };
                                }
                                //if it is repeated but somehow doesn't exist in our database, try to save it n our db
                            }
                        }
                        
                        #endregion

                        #region first call
                        //if first non-reversal call
                        Logger.DebugFormat("This is the first payment for customer with RIN: {0} and payment reference: {1}", RIN, payment.PaymentReference);

                        //populate PaymentHistory object
                        Logger.Info("Populating PaymentHistory object");
                        //confirm if i need to do any other thing here before saving
                        paymentHistory = new PaymentHistory
                        {
                            AmountPaid = payment.Amount,
                            DateCreated = DateTime.Now,
                            DateModified = DateTime.Now,
                            TotalAmountPaid = payment.Amount,
                            PaymentDate = Convert.ToDateTime(payment.PaymentDate),
                            PaymentChannel = PaymentChannel.PayDirect.ToString(),
                            IsDeleted = false,
                            TaxPayerRIN = RIN,
                            TaxPayerMobileNumber = phoneNumber,
                            TaxPayerName = "",
                            PaymentIdentifier = payment.PaymentLogId,
                            IsCustomerDeposit = true,
                            IsSyncedWithEIRS = false,
                            SettlementDate = DateTime.Now,
                            PaymentLogId = Convert.ToInt32(payment.PaymentLogId),
                            PaymentReference = payment.PaymentReference,
                            ReceiptNo = payment.ReceiptNo
                            //TaxPayerID = model.TaxPayerID,
                            //TaxPayerTypeID = model.TaxPayerTypeID,
                            //TaxPayerName = model.TaxPayerName
                        };
                        //save the record in the db
                        Logger.Debug("Saving PaymentHistory object");
                        _paymentHistoryRepo.Insert(paymentHistory);
                        Logger.Debug("Successfully saved PaymentHistory object");
                            payDirectResp = new PaymentNotificationResponse
                            {
                                Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 0,
                                      //StatusMessage = $"Payment notification was successfully synchronized"
                                  }
                                }
                            };
                            return response = new APIResponse { Success = true, Result = payDirectResp };

                        #endregion

                        #endregion
                    }


                    catch (Exception ex)
                    {
                        Logger.Error("An error occurred while trying to synchronize payDirects' Pay On Account payment Notification");
                        Logger.Error(ex.StackTrace, ex);
                        payDirectResp = new PaymentNotificationResponse
                        {
                            Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      PaymentLogId = payment.PaymentLogId,
                                      Status = 1,
                                      //StatusMessage = $"Payment notification failed to synchronize - {ex.Message}, please try again"
                                  }
                                }
                        };
                        return response = new APIResponse { Success = false, Result = payDirectResp };
                    }
                }

                Logger.Error("Could not synchronize payDirects' payment Notification");
                payDirectResp = new PaymentNotificationResponse
                {
                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      Status = 1,
                                      PaymentLogId = payments.FirstOrDefault().PaymentLogId
                                      //StatusMessage = $"Could not synchronize payment Notification, please try again"
                                  }
                                }
                };
                return response = new APIResponse { Success = false, Result = payDirectResp };
            }
            catch (Exception ex)
            {
                Logger.Error("Could not synchronize payDirects' payment Notification");
                Logger.Error(ex.StackTrace, ex);
                payDirectResp = new PaymentNotificationResponse
                {
                    Payments = new List<PaymentResponse>
                                {
                                  new PaymentResponse
                                  {
                                      Status = 1,
                                      PaymentLogId = payments.FirstOrDefault().PaymentLogId
                                      //StatusMessage = $"Could not synchronize payment Notification, please try again"
                                  }
                                }
                };
                return response = new APIResponse { Success = false, Result = payDirectResp };
            }
        }
         /// <summary>
        /// Saves No RIN Capture details to Payment Request Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public string SaveNoRINPOAPaymentRequestInfo(EIRSPaymentRequestInfo model)
        {
            EIRSPaymentRequest paymentRequest;
            try
            {
                Logger.Debug("About to save EIRS Payment Request Data for No RIN Capture");

                var identiferGen = model.TaxPayerType + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                //generate identifier
                var ident = Utils.GetPaymentIdentifier("EIRSWebPayment", identiferGen);

                //populate DAO object
                paymentRequest = new EIRSPaymentRequest
                {
                    PhoneNumber = model.PhoneNumber,
                    PaymentIdentifier = ident,
                    DateCreated = DateTime.Now,
                    DateModified = DateTime.Now,
                    Description = $"Payment for POA with TaxPaayer {model.TaxPayerName}",
                    Email = model.Email,
                    TotalAmountPaid = model.AmountPaid,
                    EconomicActivity = model.EconomicActivity,
                    TaxPayerType = model.TaxPayerType,
                    TaxPayerName = model.TaxPayerName,
                    Address = model.Address,
                    RevenueStream = model.RevenueStream,
                    RevenueSubStream = model.RevenueSubStream,
                    OtherInformation = model.OtherInformation,
                    ReferenceDate = SqlDateTime.MinValue.Value
                };

                //save to db
                Logger.Debug("trying to save No RIN CApture payment notification ");
                _eirsPaymentRequestRepo.Insert(paymentRequest);
                //return identifier
                Logger.Debug("successfully saved No RIN CApture payment notification ");
                return paymentRequest.PaymentIdentifier;
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred - could not save payment request information at this time");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        public List<TaxPayerTransaction> GetTransactionRecord(int page, int pageSize, string filterOption)
        {
            var returnmodels = new List<TaxPayerTransaction>();
            Logger.Info($"About to build query to get Transaction records");
            var query = BuildQuery(filterOption, false);
            var record = query.List<PaymentHistory>()
                                       .Skip(pageSize * (page - 1))
                                        .Take(pageSize).OrderByDescending(c=>c.PaymentDate);

            foreach (var rec in record)
            {
                returnmodels.Add(MapToTransactionModel(rec));
            }
            return returnmodels;
        }
        public List<TaxPayerTransaction> GetTransactionRecord(string filterOption)
        {
            var returnmodels = new List<TaxPayerTransaction>();
            Logger.Info($"About to build query to get Transaction records");
            var query = BuildQuery(filterOption);
            var record = query.List<PaymentHistory>().OrderByDescending(c => c.PaymentDate);

            foreach (var rec in record)
            {
                returnmodels.Add(MapToTransactionModel(rec));
            }
            return returnmodels;
        }
        public List<TaxPayerTransaction> GetUnsyncedSettlementTransaction(string filterOption)
        {
            var returnmodels = new List<TaxPayerTransaction>();
            Logger.Info($"About to build query to get Transaction records");
            var query = BuildQuery(filterOption);
            var record = query.List<PaymentHistory>().OrderByDescending(c => c.PaymentDate).Where(r => r.IsSyncedWithEIRS == false);

            foreach (var rec in record)
            {
                returnmodels.Add(MapToTransactionModel(rec));
            }
            return returnmodels;
        }
        public List<TaxPayerTransaction> GetFailedRequestTransaction(string filterOption)
        {
            var returnmodels = new List<TaxPayerTransaction>();
            Logger.Info($"About to build query to get Transaction records");
            var query = BuildQueryEIRSPaymentRequest(filterOption);
            var record = query.List<EIRSPaymentRequest>().OrderByDescending(c => c.DateCreated).Where(r => r.IsPaymentSuccessful == false && r.IsDeleted == false);
            foreach (var rec in record)
            {
                returnmodels.Add(MapToTransactionModel(rec));
            }
            return returnmodels;
        }
        public List<EIRSSettlementInfo> GetSyncedSettlementTransaction(string filterOption)
        {
            var returnmodels = new List<EIRSSettlementInfo>();
            Logger.Info($"About to build query to get Synced transactions");
            var query = BuildQuery(filterOption);
            var record = query.List<PaymentHistory>().OrderByDescending(c => c.PaymentDate).Where(r => r.IsSyncedWithEIRS == true);
            foreach (var rec in record)
            {
                returnmodels.Add(MapSettlementTransacton(rec));
            }
            return returnmodels;
        }
        public List<TaxPayerTransaction> GetPOATransactions(int page, int pageSize, string filterOption)
        {
            var returnmodels = new List<TaxPayerTransaction>();
            Logger.Info($"About to build query to get Transaction records");
            var query = BuildQuery(filterOption, true);
            var record = query.List<PaymentHistory>()
                        .Skip(pageSize * (page - 1))
                        .Take(pageSize).OrderByDescending(c=> c.PaymentDate);

            foreach (var rec in record)
            {
                returnmodels.Add(MapToTransactionModel(rec));
            }
            return returnmodels;
        }      
        public List<EIRSSettlementInfo> GetUnsyncedSettlementTransaction(int page, int pageSize, string filterOption)
        {
            var returnmodels = new List<EIRSSettlementInfo>();
            Logger.Info($"About to build query to get unsynced transactions with RDM");

            var query = BuildQuery(filterOption);

            var record = query.List<PaymentHistory>()
                        .Skip(pageSize * (page - 1))
                        .Take(pageSize).OrderByDescending(c=> c.PaymentDate).Where(r => r.IsSyncedWithEIRS == false);

            foreach (var rec in record)
            {
                returnmodels.Add(MapSettlementTransacton(rec));
            }
            return returnmodels;
        }
        
        private EIRSSettlementInfo MapSettlementTransacton(PaymentHistory rec)
        {
            EIRSSettlementInfo settlementInfo = null;
            var rNo = rec.ReferenceNumber.Trim();
            var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();

            switch (firstTwoChars)
            {
                case "AB":
                    settlementInfo = ProcesssAndGetAssessmentRecord(rec);
                    break;
                case "SB":
                    settlementInfo = ProcesssAndGetServiceBillRecord(rec);
                    break;
            }

            return settlementInfo;
        }
        public EIRSSettlementInfo ProcesssAndGetServiceBillRecord(PaymentHistory rec)
        {
            EIRSSettlementInfo settlementInfo = null;
            var serviceBill = GetServiceBillDetails(rec.ReferenceNumber); 
            if (serviceBill != null)
            {
                var serviceBillItems = serviceBill.ServiceBillItems;
                var billItems = serviceBillItems.ToList();
                if (billItems.Any())
                {
                    settlementInfo = new EIRSSettlementInfo
                    {
                        ServiceBillID = serviceBill.ServiceBillID,
                        SettlementMethod = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                        SettlementDate = rec.DateModified,
                        TransactionRefNo = rec.PaymentIdentifier,
                        TaxPayerName = rec.TaxPayerName,
                        SettlementMethodName = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? "Internet Web Pay" : rec.PaymentChannel.ToUpperInvariant(),
                        ReferenceNumber = rec.ReferenceNumber,
                        TaxPayerRIN = serviceBill.TaxpayerRIN, 
                        TotalAmountPaid = rec.AmountPaid,
                        PhoneNumber = rec.TaxPayerMobileNumber,
                        Notes = "",
                        PaymentDate = rec.PaymentDate.Value,
                        lstSettlementItems = billItems.Select(x => new SettlementItem
                        {
                            ToSettleAmount = x.AmountPaid ?? 0,
                            TBPKID = x.SBSIID,
                            TaxAmount = x.ServiceAmount ?? 0,
                        }).ToList()
                    };

                }
                else
                {
                    settlementInfo = new EIRSSettlementInfo
                    {
                        
                        ServiceBillID = serviceBill.ServiceBillID,
                        SettlementMethod = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                        SettlementDate = rec.DateModified,
                        TransactionRefNo = rec.PaymentIdentifier,
                        TaxPayerName = rec.TaxPayerName,
                        SettlementMethodName = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? "Internet Web Pay" : rec.PaymentChannel.ToUpperInvariant(),
                        ReferenceNumber = rec.ReferenceNumber,
                        TaxPayerRIN = serviceBill.TaxpayerRIN,
                        PhoneNumber = rec.TaxPayerMobileNumber,
                        Notes = "",
                        PaymentDate = rec.PaymentDate.Value,
                        TotalAmountPaid = rec.AmountPaid
                    };
 
                }
            }

            return settlementInfo;
        }
        public EIRSSettlementInfo ProcesssAndGetAssessmentRecord(PaymentHistory rec)
        {

            var assessmentDetail = GetAssessmentDetails(rec.ReferenceNumber);
            if (rec.PaymentChannel.ToUpperInvariant() == "NETPAY")
            {
                
                return new EIRSSettlementInfo
                {
                    AssessmentID = assessmentDetail != null ? assessmentDetail.AssessmentID : 0,
                     
                    SettlementMethod = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                    SettlementDate = rec.DateModified,
                    TransactionRefNo = rec.PaymentIdentifier,
                    TaxPayerName = rec.TaxPayerName,
                    SettlementMethodName = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? "Internet Web Pay" : rec.PaymentChannel.ToUpperInvariant(),
                    ReferenceNumber = rec.ReferenceNumber,
                    TaxPayerRIN = rec.TaxPayerRIN,
                    TotalAmountPaid = rec.AmountPaid,
                    PaymentDate = rec.PaymentDate.Value,
                    PhoneNumber = rec.TaxPayerMobileNumber,
                    Notes = "",
                    lstSettlementItems = rec.PaymentItemsHistory.Select(
                        x => new SettlementItem
                        {
                            TaxAmount = Convert.ToDecimal(x.ItemAmount),
                            TBPKID = x.ItemId,
                            ToSettleAmount = Convert.ToDecimal(x.AmountPaid)
                        }
                    ).ToList()
                };
            }
            else
            {
                
                var settlementInfo = new EIRSSettlementInfo();

                if (assessmentDetail != null)
                {
                    var assessmentRuleItems = assessmentDetail.AssessmentRuleItems; 
                    settlementInfo.AssessmentID = assessmentDetail != null ? assessmentDetail.AssessmentID : 0;
                    settlementInfo.SettlementMethod = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2;
                    settlementInfo.SettlementDate = rec.DateModified;
                    settlementInfo.TransactionRefNo = rec.PaymentIdentifier;
                    settlementInfo.TaxPayerName = rec.TaxPayerName;
                    settlementInfo.TotalAmountPaid = rec.AmountPaid;
                    settlementInfo.SettlementMethodName = rec.PaymentChannel.ToUpperInvariant() == "NETPAY"
                        ? "Internet Web Pay"
                        : rec.PaymentChannel.ToUpperInvariant();
                    settlementInfo.ReferenceNumber = rec.ReferenceNumber;
                    settlementInfo.TaxPayerRIN = rec.TaxPayerRIN;
                    settlementInfo.PaymentDate = rec.PaymentDate.Value;
                    settlementInfo.Notes = "";
                    settlementInfo.PhoneNumber = rec.TaxPayerMobileNumber;
                    settlementInfo.lstSettlementItems = assessmentRuleItems.Select(
                        x => new SettlementItem
                        {
                            TaxAmount = x.TaxAmount,
                            TBPKID = x.AAIID,
                            ToSettleAmount = Convert.ToDecimal(x.AmountPaid)
                        }
                    ).ToList(); 
                }
                else
                {
                    settlementInfo.AssessmentID = assessmentDetail != null ? assessmentDetail.AssessmentID : 0;
                    settlementInfo.SettlementMethod = rec.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2;
                    settlementInfo.SettlementDate = rec.DateModified;
                    settlementInfo.TransactionRefNo = rec.PaymentIdentifier;
                    settlementInfo.TaxPayerName = rec.TaxPayerName;
                    settlementInfo.TotalAmountPaid = rec.AmountPaid;
                    settlementInfo.SettlementMethodName = rec.PaymentChannel.ToUpperInvariant() == "NETPAY"
                        ? "Internet Web Pay"
                        : rec.PaymentChannel.ToUpperInvariant();
                    settlementInfo.ReferenceNumber = rec.ReferenceNumber;
                    settlementInfo.PaymentDate = rec.PaymentDate.Value;
                    settlementInfo.Notes = rec.SettlementNotes;
                }

                return settlementInfo;
            } 
        }       
        private TaxPayerTransaction MapToTransactionModel(PaymentHistory rec)
        {
            return new TaxPayerTransaction
            {
                TaxPayerName = rec.TaxPayerName,
                PaymentDate = rec.PaymentDate.Value,
                ReferenceAmount = rec.ReferenceAmount,
                ReferenceNumber = rec.ReferenceNumber,
                TaxPayerRIN = rec.TaxPayerRIN,
                PaymentIdentifer = rec.PaymentIdentifier,
                TotalAmount = rec.AmountPaid,
                IsSyncWithRDM = rec.IsSyncedWithEIRS,
                PhoneNumber = rec.TaxPayerMobileNumber
            };
        }
        private TaxPayerTransaction MapToTransactionModel(EIRSPaymentRequest rec)
        {
            return new TaxPayerTransaction
            {
                TaxPayerName = rec.TaxPayerName,
                PaymentDate = rec.DateCreated,
                ReferenceAmount = rec.TotalAmountToPay,
                ReferenceNumber = rec.ReferenceNumber,
                TaxPayerRIN = rec.TaxPayerRIN,
                PaymentIdentifer = rec.PaymentIdentifier,
                TotalAmount = rec.TotalAmountToPay,
                IsSyncWithRDM = false,
                PhoneNumber = rec.PhoneNumber
            };
        }
        
        private ICriteria BuildQuery(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(PaymentHistory));

            CultureInfo culInfo = CultureInfo.CreateSpecificCulture("en-GB");

            var filter = Deserilize(filterOption);

            if (filter != null)
            {
                //criteria.Add(Restrictions.Eq("IsSyncedWithEIRS", true));

                if (!String.IsNullOrEmpty(filter.ReferenceNumber))
                {
                    criteria.Add(Restrictions.Eq("ReferenceNumber", filter.ReferenceNumber));
                }
                if (!String.IsNullOrEmpty(filter.PaymentChannel))
                {
                    criteria.Add(Restrictions.Eq("PaymentChannel", filter.PaymentChannel));
                }
                if (!String.IsNullOrEmpty(filter.RIN))
                {
                    criteria.Add(Restrictions.Eq("TaxPayerRIN", filter.RIN));
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    criteria.Add(Restrictions.Between("PaymentDate", filter.StartDate.Value, filter.EndDate.Value));
                }
            }

            return criteria;
        }
        private ICriteria BuildQuery(string filterOption, bool custDepTrans)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(PaymentHistory));

            CultureInfo culInfo = CultureInfo.CreateSpecificCulture("en-GB");

            var filter = Deserilize(filterOption);

            if (filter != null)
            {
                criteria.Add(Restrictions.Eq("IsCustomerDeposit", custDepTrans));

                if (!String.IsNullOrEmpty(filter.ReferenceNumber))
                {
                    criteria.Add(Restrictions.Eq("ReferenceNumber", filter.ReferenceNumber));
                }
                if (!String.IsNullOrEmpty(filter.PaymentChannel))
                {
                    criteria.Add(Restrictions.Eq("PaymentChannel", filter.PaymentChannel));
                }
                if (String.IsNullOrEmpty(filter.RIN))
                {
                    criteria.Add(Restrictions.Eq("RIN", filter.RIN));
                }
                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    criteria.Add(Restrictions.Between("PaymentDate", filter.StartDate.Value, filter.EndDate.Value));
                }
            }
            return criteria;
        }
        private ICriteria BuildQueryCT(string filterOption)
        {

            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(PaymentHistory));

            CultureInfo culInfo = CultureInfo.CreateSpecificCulture("en-GB");

            var filter = Deserilize(filterOption);

            if (filter != null)
            { 
                if (!String.IsNullOrEmpty(filter.ReferenceNumber))
                {
                    criteria.Add(Restrictions.Eq("ReferenceNumber", filter.ReferenceNumber));
                }

                if (!String.IsNullOrEmpty(filter.PaymentChannel))
                {
                    criteria.Add(Restrictions.Eq("PaymentChannel", filter.PaymentChannel));
                }

                if (String.IsNullOrEmpty(filter.RIN))
                {
                    criteria.Add(Restrictions.Eq("TaxPayerRIN", filter.RIN));
                }

                if (filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    criteria.Add(Restrictions.Between("PaymentDate", filter.StartDate.Value, filter.EndDate.Value));
                }
            }
            return criteria;
        }
        private ICriteria BuildQueryEIRSPaymentRequest(string filterOption)
        {
            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(EIRSPaymentRequest));
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-GB");
            var filter = JsonConvert.DeserializeObject<FilterParams>(filterOption);
            if(filter != null)
            {
                if(!String.IsNullOrEmpty(filter.ReferenceNumber))
                {
                    criteria.Add(Restrictions.Eq("ReferenceNumber", filter.ReferenceNumber));
                }
                if(!String.IsNullOrEmpty(filter.PaymentChannel))
                {
                    //I'd need to take care of this also!!
                }
                if(!String.IsNullOrEmpty(filter.RIN))
                {
                    criteria.Add(Restrictions.Eq("TaxPayerRIN", filter.RIN));
                }
                if(filter.StartDate.HasValue && filter.EndDate.HasValue)
                {
                    criteria.Add(Restrictions.Between("DateCreated", filter.StartDate.Value, filter.EndDate.Value));
                }
            }
            return criteria;
        }
        public static FilterParams Deserilize(string whereCondition)
        {
            FilterParams filter = null;
            if (whereCondition != null)
            {
                filter = JsonConvert.DeserializeObject<FilterParams>(whereCondition);
            }
            return filter;
        }
        /// <summary>
        /// call the API to get payment transaction details using the transaction ref
        /// </summary>
        /// <param name="transRef"></param>
        /// <returns></returns>
        public List<KeyValuePair<string,object>> QuickTellerPaymentTransactionDetails(string transRef, string requestReference)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(transRef))
                {
                    Logger.Debug($"Getting transaction details for tx_ref - {transRef}");

                    var response = _restService.GetQuickTellerPaymentTransactionDetails(transRef, requestReference);
                    Logger.Debug($"Successfully returned  transaction details for tx_ref - {transRef}");
                    return response;
                    
                }
                Logger.Error("transaction ref is null");
                return null;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occured, could not successfully retrieve transaction details for {transRef}");
                Logger.Error(ex.StackTrace, ex);
                return null;
            }
        }
        public EIRSSettlementInfo GetSettlementReportDetails(string transactionRefNo)
        {
            var returnmodel = new EIRSSettlementInfo();
            Logger.Info($"About to build query to get settlement details "); 

            var _session = Utils.GetSession();
            var criteria = _session.CreateCriteria(typeof(PaymentHistory));

            var paymenthistrecord = criteria.List<PaymentHistory>().SingleOrDefault(c => c.PaymentIdentifier == transactionRefNo);
            if(paymenthistrecord != null)
            {
                returnmodel = MapSettlementTransacton(paymenthistrecord);
            }
            
            return returnmodel;
        }
        /*
         * var Median = "0.00";
    var sum = "0.00";
    var average = "0.00";
    var count = "0";
    var max = "0.00";
    var min = "0.00";
    var timespan = "---";
         * */
        public void TransactionReport(PagedList.IPagedList<CBSPay.Core.ViewModels.TaxPayerTransaction> Model, ref string Median, ref string sum, ref string average, ref string count, ref string max, ref string min, ref string timespan )
        {
            if (Model.Count != 0)
            {
                //rearrange the model by Total Amount
                var ArrangeMod = Model.OrderBy(m => m.TotalAmount);
                var str = JsonConvert.SerializeObject(ArrangeMod);
                List<TaxPayerTransaction> ArrangeModel = JsonConvert.DeserializeObject<List<TaxPayerTransaction>>(str);
                if(Model.Count % 2 == 0)//i.e. It's even
                {
                    var MedianNumbers = ArrangeModel[Model.Count / 2].TotalAmount + ArrangeModel[(Model.Count / 2) - 1].TotalAmount;
                    Median = (MedianNumbers/2).ToString("N2");
                }
                else
                {
                    Median = ArrangeModel[Model.Count / 2].TotalAmount.ToString("N2");
                }
                sum = Model.Sum(x => x.TotalAmount).ToString("N2");

                average = ((Model.Sum(x => x.TotalAmount) / Model.Count).ToString("N2"));

                count = Model.Count.ToString("N0");

                max = Model.Max(x => x.TotalAmount).ToString("N2");

                min = Model.Min(x => x.TotalAmount).ToString("N2");

                timespan = $"From {Model.LastOrDefault().PaymentDate} to {Model.FirstOrDefault().PaymentDate}";
            }
        }
        public void TransactionReport(PagedList.IPagedList<CBSPay.Core.APIModels.EIRSSettlementInfo> Model, ref string Median, ref string sum, ref string average, ref string count, ref string max, ref string min, ref string timespan)
        {
            var ArrangedMod = Model.OrderBy(m => m.TotalAmountPaid);
            var str = JsonConvert.SerializeObject(ArrangedMod);
            List<EIRSSettlementInfo> ArrangeModel = JsonConvert.DeserializeObject<List<EIRSSettlementInfo>>(str);
            if(Model.Count % 2 == 0)
            {
                var MedianNumbers = ArrangeModel[Model.Count / 2].TotalAmountPaid + ArrangeModel[(Model.Count / 2) - 1].TotalAmountPaid;
                Median = (MedianNumbers / 2).ToString("N2");
            }
            else
            {
                Median = ArrangeModel[Model.Count / 2].TotalAmountPaid.ToString("N2");
            }
            sum = Model.Sum(x => x.TotalAmountPaid).ToString("N2");

            average = ((Model.Sum(x => x.TotalAmountPaid) / Model.Count).ToString("N2"));

            count = Model.Count.ToString("N0");

            max = Model.Max(x => x.TotalAmountPaid).ToString("N2");

            min = Model.Min(x => x.TotalAmountPaid).ToString("N2");

            timespan = $"From {Model.LastOrDefault().PaymentDate} to {Model.FirstOrDefault().PaymentDate}";
        }
    }
}
