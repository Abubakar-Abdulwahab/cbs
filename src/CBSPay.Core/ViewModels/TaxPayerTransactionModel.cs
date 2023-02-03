using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBSPay.Core.APIModels;

namespace CBSPay.Core.ViewModels
{
    public class TaxPayerTransactionModel
    {
    }
    public class ServiceItemFilter
    {

    }
    public class MDAfilter
    {

    }
    public class ServiceBillfilterParams
    {

    }
    public class AssessmentItemFilterParams
    {

    }
    public class AssetFilterParams
    {

    }
    public class ProfileFilterParams
    {

    }
    public class TaxPayerAssessmentRuleFilterParams
    {
        public int? AssetTypeId { get; set; }
        //public string AssetRIN { get; set; }
        //public string ProfileDescription { get; set; }
        //public string AssessmentRuleName { get; set; }
        public int? TaxYear { get; set; }
        public decimal? minAssessmentRuleAmount { get; set; }
        public decimal? maxAssessmentRuleAmount { get; set; }
        public decimal? minSettledAmount { get; set; }
        public decimal? maxSettledAmount { get; set; }
    }
    public class TaxPayerAssessmentFilterParams
    {
        public DateTime? fromRange { get; set; }
        public DateTime? endRange { get; set; }
        public string AssessmentRefNo { get; set; }
        public string TaxPayerRIN { get; set; }
        public int? minAmount { get; set; }
        public int? maxAmount { get; set; }
        public int? SettlementStatusID { get; set; }
        public bool? Active { get; set; }
        public DateTime? settlementFromRange { get; set; }
        public DateTime? settlementEndRange { get; set; }
        public bool? due { get; set; }
    }
    public class TaxPayerFilterParams
    {
        public int? TaxPayerTypeID { get; set; }
        public string TaxPayerRIN { get; set; }
        public string TaxPayerMobileNumber { get; set; }
    }
    public class FilterParams
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RIN { get; set; }
        public string ReferenceNumber { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentChannel { get; set; }

    }

    public class TaxPayerTransaction
    {  
        public string TaxPayerName { get; set; }
        public string TaxPayerRIN { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public decimal ReferenceAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentIdentifer { get; set; }
        public DateTime PaymentDate { get; set; }
        public bool IsSyncWithRDM { get; set; }
    }

    public class SettlementTransaction : TaxPayerTransaction
    {
        public long AssessmentId { get; set; }
        public long ServiceBillId { get; set; }
        public string SettlementMethodName { get; set; }
        public string PaymentTransactionRef { get; set; }
        public DateTime SettlementDate { get; set; }
        public string Notes { get; set; }
        public List<SettlementItem> SettlementItems { get; set; }
    }
    public class Asset
    {
        public string AssetRIN { get; set; }
        public string AssetType { get; set; }
        public string Profile { get; set; }
        public decimal? TotalAmountBilled { get; set; }
        public decimal? TotalAmountSettled { get; set; }
    }
    public class Profile
    {
        public string ProfileName { get; set; }
        public decimal? ProfileAmountBilled { get; set; }
        public decimal? ProfileAmountSettled { get; set; }
    }
}
