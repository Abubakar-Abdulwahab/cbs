using CBSPay.Core.APIModels;
using CBSPay.Core.Helpers;
using CBSPay.Core.Interfaces;
using CBSPay.Core.Models;
using CBSPay.Core.Services;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
namespace EIRSPaymentSettlementTask
{
    public class TaskImpl
    {
        private ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private SchedulerHelper _taskUtil;
        private readonly IPaymentService _paymentService = new PaymentService();
        private readonly ITaxPayerService _taxPayerService = new TaxPayerService();
        private readonly IRestService _restService = new RestService();

        public TaskImpl(SchedulerHelper taskUtil)
        {
            _taskUtil = taskUtil;
            Logger = _taskUtil.Logger;
        }
        
        public void Process()
        {
            try
            {
                Logger.Debug("About to begin processing for EIRS Payment Settlement Task");
                EIRSSettlementDetails settlementInfo;
                var configValues = ConfigValues();
                Logger.Debug("fetch paymentHistory details that has not been synchronized to EIRS Settlement");
                var unSyncedPaymentRecords = _restService.GetUnsyncedPaymentRecords(configValues);
                if (unSyncedPaymentRecords.Count() <= 0)
                {
                    Logger.Error("There is nothing to process this time!!");
                    return;
                }
                Logger.Info("The config values from the task: =>>"+JsonConvert.SerializeObject(configValues));
                if (configValues == null)
                {
                    Logger.Error("The Config Values has not been added to the task, please add them to the neccessary config file");
                    return;
                }
                Logger.Debug("Build and process EIRSSettlementObject");
                foreach (var record in unSyncedPaymentRecords)
                {
                    #region Pay On Account Settlement
                    Logger.Debug("check if it is the for Pay On Account or Pay With Reference");
                    if (record.IsCustomerDeposit == true)
                    {
                        //according to EIRS web api documentation - settlement method for bank transfer = 2; for internet web transfer = 1
                        Logger.Debug("It is a Pay On Account Settlement");
                        var rin = record.TaxPayerRIN; var phone = record.TaxPayerMobileNumber;
                        if (rin != null)
                        {
                            var taxPayerRecord = _taxPayerService.FetchPOATaxPayerInfo(rin, phone);
                            if (taxPayerRecord !=null)
                            {
                                var payOnSettlement = new PayOnAccountSettlement
                                {
                                    Amount = record.AmountPaid,
                                    Notes = $"{taxPayerRecord.TaxPayerName} pay on account settlement",
                                    PaymentDate = Convert.ToDateTime(record.PaymentDate),
                                    PaymentMethodID = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                                    TaxPayerID = taxPayerRecord.TaxPayerID,
                                    TaxPayerTypeID = taxPayerRecord.TaxPayerTypeID,
                                    TransactionRefNo = record.PaymentIdentifier
                                };
                                var response = _restService.NotifyEIRSOfPOASettlement(payOnSettlement, configValues);
                                Logger.Debug("if web call is successful, update the paymentHistory table");
                                if (response)
                                {
                                    Logger.Debug($"Successfully notified EIRS of settelement info for {payOnSettlement.TransactionRefNo} ");
                                    _paymentService.UpdatePaymentHistoryRecords(record);
                                    Logger.Debug($"Successfully updated the records in Payment History table for {payOnSettlement.TransactionRefNo} ");
                                    continue;
                                }
                                else
                                {
                                    Logger.Debug($"Could not successfully notify EIRS of settelement info for {payOnSettlement.TransactionRefNo} ");
                                    continue;
                                }
                            }
                            else
                            {
                                Logger.Error($"Could not find a taxpayer record with this RIn - {rin} and phone number - {phone}");
                                continue;
                            }
                        }
                        else
                        {
                            Logger.Error($"TaxPayer RIn value cannot be null");
                            continue;
                        }
                    }
                    #endregion

                    #region Pay With Reference Settlement
                    Logger.Debug("It is a Pay with Reference Settlement");
                    var refNo = record.ReferenceNumber;
                    if (!string.IsNullOrWhiteSpace(refNo))
                    {
                        var rNo = refNo.Trim();
                        var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();

                        switch (firstTwoChars)
                        {
                            //according to EIRS web api documentation - settlement method for bank transfer = 2; for internet web transfer = 1
                            #region Assessment Details
                            case "AB":
                                AssessmentDetailsResult assessment;
                                bool isSuccess;
                                Logger.Debug($"It is an assessment details with reference number {refNo}");

                                #region NetPay
                                if (record.PaymentChannel.ToUpperInvariant() == "NETPAY")
                                {
                                    settlementInfo = new EIRSSettlementDetails
                                    {
                                        AssessmentID = record.ReferenceID,
                                        SettlementMethod = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                                        TransactionRefNo = record.PaymentIdentifier,
                                        SettlementDate = record.DateModified.ToString("yyyy-MM-dd"),
                                        //lstSettlementItems = record.PaymentItemsHistory.Select(x =>new SettlementItemDetail {TaxAmount=Convert.ToInt32(x.ItemAmount), TBPKID = Convert.ToInt32(x.ItemId), ToSettleAmount = Convert.ToInt32(x.AmountPaid)}).ToList()
                                        lstSettlementItems = record.PaymentItemsHistory.Select(x =>new SettlementItemDetail {TaxAmount=Convert.ToInt32(x.ItemAmount).ToString(), TBPKID = Convert.ToInt32(x.ItemId), ToSettleAmount = Convert.ToInt32(x.AmountPaid).ToString()}).ToList()
                                    };
                                    //notify EIRS of payment
                                    Logger.Debug($"Trying to notify EIRS of payment settlement for {record.PaymentIdentifier} ");
                                    Logger.Info(JsonConvert.SerializeObject(settlementInfo));
                                    isSuccess = _restService.NotifyEIRSOfSettlementPayment(settlementInfo, configValues, Logger);

                                    if (isSuccess)
                                    {
                                        //if web call is successful, update the paymentHistory table
                                        Logger.Debug($"Successfully notified EIRS of settelement info for {record.PaymentIdentifier} with Reference Number {refNo}");
                                        _restService.UpdatePaymentHistoryRecords(record,configValues);
                                        Logger.Debug($"Successfully updated the records in Payment History table for {record.PaymentIdentifier}");
                                        continue;
                                    }
                                    Logger.Error($"Could not Successfully update the records in Payment History table for {record.PaymentIdentifier}");
                                    continue;
                                }
                                #endregion

                                //assessment = _taxPayerService.GetAssessmentDetails(rNo, configValues);
                                else
                                {
                                    assessment = _paymentService.GetAssessmentDetails(rNo);
                                    if (assessment != null)
                                    {
                                        var assessmentRuleItems = assessment.AssessmentRuleItems;
                                        if (assessmentRuleItems.Count() > 0)
                                        {
                                            settlementInfo = new EIRSSettlementDetails
                                            {
                                                AssessmentID = assessment.AssessmentID,
                                                SettlementMethod = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                                                TransactionRefNo = record.PaymentIdentifier,
                                                SettlementDate = record.DateModified.ToString("yyyy-MM-dd"),
                                                //lstSettlementItems = assessmentRuleItems.Select(x => new SettlementItemDetail { TaxAmount = Convert.ToInt32(x.TaxAmount), TBPKID = Convert.ToInt32(x.AAIID), ToSettleAmount = Convert.ToInt32(x.AmountPaid)}).ToList()
                                                lstSettlementItems = assessmentRuleItems.Select(x => new SettlementItemDetail { TaxAmount = Convert.ToInt32(x.TaxAmount).ToString(), TBPKID = Convert.ToInt32(x.AAIID), ToSettleAmount = Convert.ToDecimal(x.AmountPaid).ToString()}).ToList()
                                            };
                                            //notify EIRS of payment
                                            Logger.Debug($"Trying to notify EIRS of payment settlement");
                                            isSuccess = _restService.NotifyEIRSOfSettlementPayment(settlementInfo, configValues, Logger);

                                            if (isSuccess)
                                            {
                                                //if web call is successful, update the paymentHistory table
                                                Logger.Debug($"Successfully notified EIRS of settelement with Reference Number {refNo}");
                                                _restService.UpdatePaymentHistoryRecords(record,configValues);
                                                Logger.Debug($"Successfully updated the records in Payment History table");
                                                continue;
                                            }
                                            else
                                            {
                                                Logger.Error($"Could not successfully update the records in Payment History table");
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            Logger.Error($"Could not find assessment rule items with this assessment Id - {assessment.Id}");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        Logger.Error($"Could not find an assessment with this ref number - {rNo}");
                                        continue;
                                    }
                                }
                            #endregion

                            #region Service Bill
                            case "SB":
                                Logger.Debug($"It is a service bill details with reference number {refNo}");
                                ServiceBillResult serviceBill;
                                bool Success;
                                //build the object
                                #region NetPay
                                if (record.PaymentChannel.ToUpperInvariant() == "NETPAY")
                                {
                                    settlementInfo = new EIRSSettlementDetails
                                    {
                                        ServiceBillID = record.ReferenceID,
                                        SettlementMethod = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                                        TransactionRefNo = record.PaymentIdentifier,
                                        SettlementDate = record.DateModified.ToString("yyyy-MM-dd"),
                                        //lstSettlementItems = record.PaymentItemsHistory.Select(x => new SettlementItemDetail { TaxAmount = Convert.ToInt32(x.ItemAmount), TBPKID = Convert.ToInt32(x.ItemId), ToSettleAmount = Convert.ToInt32(x.AmountPaid)}).ToList()
                                        lstSettlementItems = record.PaymentItemsHistory.Select(x => new SettlementItemDetail { TaxAmount = Convert.ToInt32(x.ItemAmount).ToString(), TBPKID = Convert.ToInt32(x.ItemId), ToSettleAmount = Convert.ToInt32(x.AmountPaid).ToString()}).ToList()
                                    };
                                    //notify EIRS of payment
                                    Logger.Debug($"Trying to notify EIRS of payment settlement");
                                    Logger.Info(JsonConvert.SerializeObject(settlementInfo));
                                    Success = _restService.NotifyEIRSOfSettlementPayment(settlementInfo, configValues, Logger);

                                    if (Success)
                                    {
                                        //if web call is successful, update the paymentHistory table
                                        Logger.Debug($"Successfully notified EIRS of settelement info with Reference Number {refNo}");
                                        _restService.UpdatePaymentHistoryRecords(record,configValues);
                                        Logger.Debug("Successfully updated the records in Payment History table");
                                        continue;
                                    }
                                    else
                                    {
                                        Logger.Debug("Could not Successfully update the records in Payment History table");
                                        continue;
                                    }
                                }
                                #endregion

                                else
                                {
                                    serviceBill = _paymentService.GetServiceBillDetails(rNo);
                                    if (serviceBill != null)
                                    {
                                        var serviceBillItems = serviceBill.ServiceBillItems;
                                        if (serviceBillItems.Count() > 0)
                                        {
                                            settlementInfo = new EIRSSettlementDetails
                                            {
                                                ServiceBillID = serviceBill.ServiceBillID,
                                                SettlementMethod = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
                                                TransactionRefNo = record.PaymentIdentifier,
                                                SettlementDate = record.DateModified.ToString("yyyy-MM-dd"),
                                                //lstSettlementItems = serviceBillItems.Select(x => new SettlementItemDetail { ToSettleAmount = Convert.ToInt32(x.AmountPaid), TBPKID = Convert.ToInt32(x.SBSIID), TaxAmount = Convert.ToInt32(x.ServiceAmount)}).ToList()
                                                lstSettlementItems = serviceBillItems.Select(x => new SettlementItemDetail { ToSettleAmount = Convert.ToInt32(x.AmountPaid).ToString(), TBPKID = Convert.ToInt32(x.SBSIID), TaxAmount = Convert.ToInt32(x.ServiceAmount).ToString() }).ToList()
                                            };
                                            //notify EIRS of payment
                                            Logger.Debug("Trying to notify EIRS of payment");
                                            var success = _restService.NotifyEIRSOfSettlementPayment(settlementInfo, configValues, Logger);
                                            if (success)
                                            {
                                                //if web call is successful, update the paymentHistory table
                                                Logger.Debug($"Successfully notified EIRS of settelement info with Reference Number {refNo}");
                                                _restService.UpdatePaymentHistoryRecords(record,configValues);
                                                Logger.Debug("Successfully updated the records in Payment History table");
                                                continue;
                                            }
                                            else
                                            {
                                                Logger.Debug("Could not Successfully update the records in Payment History table");
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            Logger.Error("Could not find service bill items with this service bill");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        Logger.Error($"Could not find a service bill with this ref number - {rNo}");
                                        continue;
                                    }
                                }                                
                                #endregion
                        }
                    }
                    
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Could not complete the process of synchronization");
                Logger.Error(ex.StackTrace, ex);
                throw;
            }
        }
        //public void TestProcess()
        //{
        //    try
        //    {
        //        Logger.Debug("About to begin processing for EIRS Payment Settlement Task");
        //        EIRSSettlementInfo settlementInfo;
        //        //check and get from the db paymentHistory details that has not been synchronized to EIRS Settlement
        //        Logger.Debug("fetch paymentHistory details that has not been synchronized to EIRS Settlement");
        //        var unSyncedPaymentRecords = _paymentService.GetUnsyncedPaymentRecords();

        //        if (unSyncedPaymentRecords.Count() <= 0)
        //        {
        //            Logger.Error("An error occurred or there is nothing to process at this time");
        //            return;
        //        }

        //        //Build and process EIRSSettlementObject
        //        foreach (var record in unSyncedPaymentRecords)
        //        {
        //            var configValues = ConfigValues();
        //            if (configValues == null)
        //            {
        //                Logger.Error("The Config Values has not been added to the task, please add them to the neccessary config file");
        //                return;
        //            }

        //            #region Pay On Account Settlement
        //            //check if it is the for Pay On Account or Pay With Reference
        //            if (record.IsCustomerDeposit == true)
        //            {
        //                //according to EIRS web api documentation - settlement method for bank transfer = 2; for internet web transfer = 1
        //                Logger.Debug("It is a Pay On Account Settlement");
        //                var rin = record.TaxPayerRIN; var phone = record.TaxPayerMobileNumber;
        //                var taxPayerRecord = _taxPayerService.RetrieveTaxPayerInfo(rin, phone);
        //                var payOnSettlement = new PayOnAccountSettlement
        //                {
        //                    Amount = record.AmountPaid,
        //                    Notes = "",
        //                    PaymentDate = Convert.ToDateTime(record.PaymentDate),
        //                    PaymentMethodID = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
        //                    TaxPayerID = taxPayerRecord.TaxPayerID,
        //                    TaxPayerTypeID = taxPayerRecord.TaxPayerTypeID
        //                };
        //                var response = _restService.NotifyEIRSOfPOASettlement(payOnSettlement, configValues);
        //                if (response)
        //                {
        //                    //if web call is successful, update the paymentHistory table
        //                    Logger.Debug("Successfully notified EIRS of settelement info");
        //                    _paymentService.UpdatePaymentHistoryRecords(record);
        //                    Logger.Debug("Successfully updated the records in Payment History table");
        //                    return;
        //                }
        //            }
        //            #endregion

        //            #region Pay With Reference Settlement
        //            Logger.Debug("It is a Pay with Reference Settlement");
        //            var refNo = record.ReferenceNumber;
        //            var rNo = refNo.Trim();
        //            var firstTwoChars = rNo.Length <= 2 ? "" : rNo.Substring(0, 2).ToUpperInvariant();

        //            switch (firstTwoChars)
        //            {
        //                //according to EIRS web api documentation - settlement method for bank transfer = 2; for internet web transfer = 1
        //                #region Assessment Details
        //                case "AB":
        //                    AssessmentDetailsResult assessment;
        //                    bool isSuccess;
        //                    Logger.Debug($"It is an assessment details with reference number {refNo}");
        //                    //build the object


        //                        assessment = _paymentService.GetAssessmentDetails(rNo);
        //                        if (assessment == null)
        //                        {
        //                            Logger.Error("Could not find an assessment with this ref number");
        //                            return;
        //                        }
        //                        var assessmentRuleItems = assessment.AssessmentRuleItems;
        //                        if (assessmentRuleItems.Count() < 0)
        //                        {
        //                            Logger.Error("Could not find assessment rule items with this assessment");
        //                            return;
        //                        }
        //                        settlementInfo = new EIRSSettlementInfo
        //                        {
        //                            AssessmentID = assessment.AssessmentID,
        //                            SettlementMethod = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
        //                            SettlementDate = record.DateModified,
        //                            Notes = "",
        //                            lstSettlementItems = assessmentRuleItems.Select(x => new SettlementItem { TaxAmount = x.TaxAmount, TBPKID = x.AAIID, ToSettleAmount = Convert.ToDecimal(x.AmountPaid) }
        //                                 ).ToList()
        //                        };
        //                        //notify EIRS of payment
        //                        Logger.Debug("Trying to notify EIRS of payment settlement");
        //                        isSuccess = _restService.NotifyEIRSOfSettlementPayment(settlementInfo, configValues, Logger);

        //                        if (isSuccess)
        //                        {
        //                            //if web call is successful, update the paymentHistory table
        //                            Logger.Debug("Successfully notified EIRS of settelement info");
        //                            _paymentService.UpdatePaymentHistoryRecords(record);
        //                            Logger.Debug("Successfully updated the records in Payment History table");
        //                            return;
        //                        }
        //                        Logger.Debug("Could not Successfully update the records in Payment History table");
        //                        return;

        //                #endregion

        //                #region Service Bill
        //                case "SB":
        //                    Logger.Debug($"It is a service bill details with reference number {refNo}");
        //                    ServiceBillResult serviceBill;
        //                    bool Success;
        //                    //build the object

        //                        serviceBill = _paymentService.GetServiceBillDetails(rNo);

        //                        if (serviceBill == null)
        //                        {
        //                            Logger.Error("Could not find a service bill with this ref number");
        //                            return;
        //                        }
        //                        var serviceBillItems = serviceBill.ServiceBillItems;
        //                        if (serviceBillItems.Count() <= 0)
        //                        {
        //                            Logger.Error("Could not find service bill items with this service bill");
        //                            return;
        //                        }
        //                        settlementInfo = new EIRSSettlementInfo
        //                        {
        //                            ServiceBillID = serviceBill.ServiceBillID,
        //                            SettlementMethod = record.PaymentChannel.ToUpperInvariant() == "NETPAY" ? 1 : 2,
        //                            SettlementDate = record.DateModified,
        //                            Notes = "",
        //                            lstSettlementItems = serviceBillItems.Select(x => new SettlementItem { ToSettleAmount = Convert.ToDecimal(x.AmountPaid), TBPKID = x.SBSIID, TaxAmount = Convert.ToDecimal(x.ServiceAmount) }).ToList()
        //                        };
        //                        //notify EIRS of payment
        //                        Logger.Debug("Trying to notify EIRS of payment");
        //                        var success = _restService.NotifyEIRSOfSettlementPayment(settlementInfo, configValues, Logger);
        //                        if (success)
        //                        {
        //                            //if web call is successful, update the paymentHistory table
        //                            Logger.Debug("Successfully notified EIRS of settelement info");
        //                            _paymentService.UpdatePaymentHistoryRecords(record);
        //                            Logger.Debug("Successfully updated the records in Payment History table");
        //                            return;
        //                        }
        //                        Logger.Debug("Could not Successfully update the records in Payment History table");
        //                        return;
        //                    //serviceBill = _taxPayerService.GetServiceBillDetails(rNo, configValues);

        //                    #endregion
        //            }
        //            #endregion
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error("Could not complete the process of synchronization");
        //        Logger.Error(ex.StackTrace, ex);
        //        throw;
        //    }

        //}
        protected EIRSTaskConfigValues ConfigValues()
        {
            var obj = new EIRSTaskConfigValues
            {
                EIRSAddSettlementUrl = _taskUtil.GetConfig("EIRSAddSettlementUrl"),
                EIRSInsertPayOnAccountUrl = _taskUtil.GetConfig("EIRSAddPOAUrl"),
                EIRSAPILoginAccept = _taskUtil.GetConfig("EIRSAPILoginAccept"),
                EIRSAPILoginContentType = _taskUtil.GetConfig("EIRSAPILoginContentType"),
                EIRSAPILoginGrantType = _taskUtil.GetConfig("EIRSAPILoginGrantType"),
                EIRSAPILoginPassword = _taskUtil.GetConfig("EIRSAPILoginPassword"),
                EIRSAPILoginUsername = _taskUtil.GetConfig("EIRSAPILoginUsername"),
                EIRSBaseUrl = _taskUtil.GetConfig("EIRSBaseUrl"),
                LoginUrl = _taskUtil.GetConfig("LoginUrl"),
                GetServiceBillItemsUrl = _taskUtil.GetConfig("GetServiceBillItemsUrl"),
                GetAssessmentRuleItemsUrl = _taskUtil.GetConfig("GetAssessmentRuleItemsUrl"),
                GetServiceBillDetailsUrl = _taskUtil.GetConfig("GetServiceBillDetailsUrl"),
                GetAssessmentDetailUrl = _taskUtil.GetConfig("GetAssessmentDetailUrl"),
                ClientId = _taskUtil.GetConfig("ClientId"),
                ClientSecret = _taskUtil.GetConfig("ClientSecret"),
                UnsyncedPaymentUrl = _taskUtil.GetConfig("UnsyncedPaymentUrl"),
                AppBaseUrl = _taskUtil.GetConfig("AppBaseUrl"),
                UpdatePaymentUrl = _taskUtil.GetConfig("UpdatePaymentUrl"),
                
            };
            return obj;
        }
    }
}