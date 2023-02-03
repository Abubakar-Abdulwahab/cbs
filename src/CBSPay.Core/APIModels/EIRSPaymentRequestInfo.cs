using CBSPay.Core.Helpers;
using CBSPay.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.APIModels
{
    public class EIRSPaymentRequestInfo
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string TaxPayerName { get; set; }
        //public string ReferenceId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string Description { get; set; }
        public string PaymentIdentifier { get; set; }
        public string ReferenceNumber { get; set; }
        public long TaxPayerID { get; set; }
        public long TaxPayerTypeID { get; set; }
        /// <summary>
        /// AssessmentID (e.g 10009) for assessment and ServiceBillID (e.g 10007) for service bill
        /// </summary>
        public long ReferenceId { get; set; }
        public string PhoneNumber { get; set; }
        public string TaxPayerTIN { get; set; }
        public string TaxPayerRIN { get; set; }
        public ICollection<PaymentRequestItem> PaymentRequestItems { get; set; }

        //NO RIN Capture objects
        public  string TaxPayerType { get; set; }
        public  string TaxPayerTypeName { get; set; }
        public  string Email { get; set; }
        public  string EconomicActivity { get; set; }
        public  string Address { get; set; }
        public  string RevenueStream { get; set; }
        public  string RevenueSubStream { get; set; }
        public  string OtherInformation { get; set; }

        //Inner Bill Details
        public string TemplateType { get; set; }
        public DateTime Date { get; set; }
        public string SettlementStatus { get; set; }
        public string Notes { get; set; }
        public string RefNumber { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal TotalAmountToPay { get; set; }
        public int SettlementMethod { get; set; }
        public string AddNotes { get; set; }
        public ICollection<RefRule> RefRules { get; set; }
        public ICollection<RefItem> RuleItems { get; set; }
    }



    public class PaymentRequestItem
    {
        /// <summary>
        /// AAIID(e.g 1000) for assessment rule Item and SBSIID( e.g 1011) for service bill Item
        /// </summary>
        public long ItemId { get; set; }
        public long ItemRef { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemAmount { get; set; }
        public decimal AmountPaid { get; set; }
        
        //
        //public string RuleName { get; set; }
        //public long RefRuleID { get; set; }
        //public decimal RuleAmount { get; set; }
        //public decimal SettledAmount { get; set; }
        //public decimal OutstandingAmount { get; set; }
        //public int TaxYear { get; set; }
    }
}
