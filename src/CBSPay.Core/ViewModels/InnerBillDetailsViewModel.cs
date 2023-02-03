using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.ViewModels
{
    public class InnerBillDetailsViewModel
    {
        public string TemplateType { get; set; }
        public DateTime Date { get; set; }
        public string TaxPayerType { get; set; }
        public string TaxPayerName { get; set; }
        public string SettlementStatus { get; set; }
        public string Notes { get; set; }
        public string RefNumber { get; set; }
        public IEnumerable<RefItem> RuleItems { get; set; }
        public IEnumerable<RefRule> RefRules { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal TotalAmountToPay { get; set; }
        public int SettlementMethod { get; set; }
        public string AddNotes { get; set; }
        public string PhoneNumber { get; set; }
        public long TaxPayerID { get; set; }
        public long TaxPayerTypeID { get; set; }
        public string TaxPayerRIN { get; set; }


        //////////////////////////////////Not Used
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public decimal AmountPaid { get; set; }
        public string Description { get; set; }
        public string PaymentIdentifier { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        /// <summary>
        /// AssessmentID (e.g 10009) for assessment and ServiceBillID (e.g 10007) for service bill
        /// </summary>
        public long ReferenceId { get; set; }
        public string TaxPayerTIN { get; set; }
        
        public string TaxPayerTypeName { get; set; }
        public string Email { get; set; }
        public string EconomicActivity { get; set; }
        public string Address { get; set; }
        public string RevenueStream { get; set; }
        public string RevenueSubStream { get; set; }
        //public string OtherInformation { get; set; }
    }

    public class RefRule
    {
        public int TaxYear { get; set; }
        public long RefRuleID { get; set; }
        public string RuleName { get; set; }
        public long RuleID { get; set; }
        public decimal RuleAmount { get; set; }
        public decimal SettledAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal RuleAmountToPay { get; set; }
        public string RuleItemRef { get; set; }
        public string RuleItemID { get; set; }
        public string RuleItemName { get; set; }
        public string RuleComputation { get; set; }
        public decimal AmountPaid { get; set; }
        public long TBPKID { get; set; }
    }

    public class RefItem
    {
        public string ItemRef { get; set; }
        public string ItemID { get; set; }
        public string ItemName { get; set; }
        public string Computation { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal PendingAmount { get; set; }
        public long RefRuleID { get; set; }
    }
}
