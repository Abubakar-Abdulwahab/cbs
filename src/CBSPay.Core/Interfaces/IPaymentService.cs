using System.Collections.Generic;
using CBSPay.Core.ViewModels;
using CBSPay.Core.APIModels;
using CBSPay.Core.Models;
using CBSPay.Core.Helpers;
using CBSPay.Core.Entities;

namespace CBSPay.Core.Interfaces
{
    public interface IPaymentService
    {
        APIResponse SynchronizePayDirectPayments(List<Payment> payments);
        IEnumerable<PaymentHistory> GetUnsyncedPaymentRecords();

        AssessmentDetailsResult GetAssessmentDetails(string referenceNumber);

        ServiceBillResult GetServiceBillDetails(string referenceNumber);
         
        string SaveEIRSBillRefPaymentRequestInfo(EIRSPaymentRequestInfo model);
        string SaveEIRSPOAPaymentRequestInfo(EIRSPaymentRequestInfo model);
        string SaveNoRINPOAPaymentRequestInfo(EIRSPaymentRequestInfo model);
        
        void UpdatePaymentHistoryRecords(PaymentHistory record);
        //APIResponse ProcessNetPayPaymentNotification(PaymentDetails paymentDetails);
        APIResponse ProcessBankCollectPaymentNotification(PaymentDetails paymentDetails);
        //PaymentStatusModel TestProcessNetPayPaymentNotification(string paymentTransactionRef, decimal amountpaid);
        PaymentStatusModel ProcessWebPaymentNotification(string paymentTransactionRef, decimal amountpaid, string responseModel);
        APIResponse SynchronizePayDirectPOAPayments(List<Payment> payments);

        List<KeyValuePair<string, object>> QuickTellerPaymentTransactionDetails(string transRef, string requestReference);

        List<TaxPayerTransaction> GetTransactionRecord(int page, int pageSize, string filterOption);

        List<TaxPayerTransaction> GetTransactionRecord(string filterOption);
        List<TaxPayerTransaction> GetUnsyncedSettlementTransaction(string filterOption);
        List<TaxPayerTransaction> GetFailedRequestTransaction(string filterOption);
        List<TaxPayerTransaction> GetPOATransactions(int page, int pageSize, string filterOption);

        List<EIRSSettlementInfo> GetUnsyncedSettlementTransaction(int page, int pageSize, string filterOption);

        List<EIRSSettlementInfo> GetSyncedSettlementTransaction(string filterOption);
        
        EIRSSettlementInfo GetSettlementReportDetails(string transactionRefNo);
        PaymentStatusModel ProcessQuicktellerWebPayment(string requestReference, decimal trueAmount);
    }
}