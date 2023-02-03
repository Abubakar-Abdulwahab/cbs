
using CBSPay.Core.APIModels;
using CBSPay.Core.Helpers;
using CBSPay.Core.Models;
using CBSPay.Core.ViewModels;
using log4net;
using System.Collections.Generic;

namespace CBSPay.Core.Interfaces
{
    public interface IRestService
    { 
        EIRSAPIResponse GetAssessmentDetailsByRefNumber(string referenceNumber); 

        EIRSAPIResponse GetServiceBillDetailsByRefNumber(string referenceNumber);
        EIRSAPIResponse GetAssessmentDetailsByRefNumber(string referenceNumber, EIRSTaskConfigValues configValues);

        EIRSAPIResponse GetServiceBillDetailsByRefNumber(string referenceNumber, EIRSTaskConfigValues configValues);
        EIRSAPIResponse GetAssessmentRuleItems(long assessmentID, EIRSTaskConfigValues configValues);
        EIRSAPIResponse GetServiceBillItems(long serviceBillID, EIRSTaskConfigValues configValues);
        EIRSAPIResponse GetAssessmentRules(long assessmentID);
        EIRSAPIResponse GetServiceBillRules(long serviceBillID);
        APIResponse MakeNETPAYPayment(EIRSPaymentRequestInfo model);
        bool MakePaymentViaNetPay(EIRSPaymentRequestInfo model);
        //bool NotifyEIRSOfSettlementPayment(EIRSSettlementInfo settlementRequest, EIRSTaskConfigValues configValues);
        void UpdatePaymentHistoryRecords(PaymentHistory record, EIRSTaskConfigValues configValues);
        List<PaymentHistory> GetUnsyncedPaymentRecords(EIRSTaskConfigValues configValues);
        bool NotifyEIRSOfSettlementPayment(EIRSSettlementDetails settlementRequest, EIRSTaskConfigValues configValues, ILog log);
        //bool NotifyEIRSOfSettlementPayment(EIRSSettlementInfo settlementRequest, );
        EIRSAPIResponse GetAssessmentRuleItems(long assessmentID);
        EIRSAPIResponse GetServiceBillItems(long serviceBillID);
        EIRSAPIResponse GetTaxPayerByRINAndMobile(string rin, string mobileNumber);
        bool NotifyEIRSOfPOASettlement(PayOnAccountSettlement payOnSettlement, EIRSTaskConfigValues configValues);
        EIRSAPIResponse GetTaxPayerByBusinessName(string business);
        EIRSAPIResponse GetTaxPayerByMobileNumber(string mobile);
        EIRSAPIResponse GetTaxPayerByRIN(string rin);
        EIRSAPIResponse GetRevenueSubStreamList();
        EIRSAPIResponse GetRevenueStreamList();
        EIRSAPIResponse GetTaxPayerTypeList();
        EIRSAPIResponse GetEconomicActivitiesList(int TaxPayerTypeID);
        List<KeyValuePair<string,object>> GetQuickTellerPaymentTransactionDetails(string transRef, string requestReference);//QuicktellerAPIResponse
    }
}