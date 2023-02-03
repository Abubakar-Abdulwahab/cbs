using CBSPay.Core.APIModels;
using CBSPay.Core.Entities;
using CBSPay.Core.Helpers;
using CBSPay.Core.Models;
using CBSPay.Core.ViewModels;
using System.Collections.Generic;

namespace CBSPay.Core.Interfaces
{
    public interface ITaxPayerService
    {
        APIResponse DoBasicValidation(string refNumber);
        string DoPayDirectReferenceBasicValidation(string XMLString, string methodName);
        APIResponse RetrievePayDirectTaxPayerInfo(string merchantReference, string custReference, string thirdPartyCode);
        TaxPayerPaymentInfo RetrieveTaxPayerInfo(string refNumber);
        IEnumerable<POATaxPayerResponse> RetrieveTaxPayerInfoByRIN(string RIN);
        IEnumerable<POATaxPayerResponse> RetrieveTaxPayerInfoByMobileNumber(string mobileNumber);
        IEnumerable<POATaxPayerResponse> RetrieveTaxPayerInfoByBusinessName(string businessName);
        APIResponse RetrievePayDirectPOATaxPayerInfo(string merchantReference, string custReference, string thirdPartyCode);
        POATaxPayerResponse RetrieveTaxPayerInfo(string RIN, string mobileNumber);
        POATaxPayerResponse FetchPOATaxPayerInfo(string RIN, string mobileNumber);
        string DoPayDirectPOABasicValidation(string txt, string methodName);
        AssessmentDetailsResult GetAssessmentDetails(string rNo, EIRSTaskConfigValues configValues);
        ServiceBillResult GetServiceBillDetails(string rNo, EIRSTaskConfigValues configValues);
        ServiceBillResult ProcessServiceBill(string refNumber);
        AssessmentDetailsResult ProcessAssessmentDetails(string refNumber);
        List<TaxPayerDetails> GetTaxPayerDetails(string filterOption);
        List<AssessmentDetailsResult> GetAssessmentDetailsResult(string filterOption);
        List<AssessmentRule> GetAssessmentRule(string filterOption);
        List<Asset> GetAsset(string filterOption);
        List<Profile> GetProfile(string filterOption);
        IEnumerable<AssessmentRuleItem> GetAssessmentItem(string filterOption);
        List<ServiceBillResult> GetServiceBill(string filterOption);
        List<MDAService> GetMDAService(string filterOption);
        List<ServiceBillItem> GetServiceBillItem(string filterOption);
        string ReportFilter(int? page, string fromRange, string endRange, string referenceNumber, string paymentChannel, string paymentDate, string TaxPayerRIN, int pageSize, out int pageIndex);
    }
}