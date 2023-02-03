using System;
using System.Collections.Generic;

namespace CBSPay.Core.APIModels
{
    public class EIRSSettlementDetails
    {
        public long AssessmentID { get; set; }
        public long ServiceBillID { get; set; }
        public int SettlementMethod { get; set; }
        public string TransactionRefNo { get; set; }
        public string SettlementDate { get; set; }
        public List<SettlementItemDetail> lstSettlementItems { get; set; }
    }
    public class EIRSSettlementInfo
    {
        public string TaxPayerName { get; set; }
        public string ReferenceNumber { get; set; } 
        public string TaxPayerRIN { get; set; }
        public string SettlementMethodName { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ReceiptPath { get; set; }
        public byte[] ReceiptFile { get; set; }
        public string PhoneNumber { get; set; }

        public long AssessmentID { get; set; }
        public long ServiceBillID { get; set; }
        public int SettlementMethod { get; set; }
        public string TransactionRefNo { get; set; }
        public DateTime SettlementDate { get; set; }
        public string Notes { get; set; }
        public List<SettlementItem> lstSettlementItems { get; set; }
    }
}
