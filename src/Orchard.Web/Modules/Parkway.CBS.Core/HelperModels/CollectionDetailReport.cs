using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class CollectionDetailReport
    {
        public DateTime PaymentDate { get; set; }

        public DateTime TransactionDate { get; set; }

        public DateTime CreateAtUtc { get; set; }

        public string TaxPayerName { get; set; }

        public string TaxPayerTIN { get; set; }

        public string RevenueHeadName { get; set; }

        public decimal Amount { get; set; }

        public string PaymentRef { get; set; }


        public string Channel { get; set; }

        public string PaymentProvider { get; set; }


        public string InvoiceNumber { get; set; }

        public string ReceiptNumber { get; set; }

        public string MDAName { get; set; }

        public int MDAId { get; set; }

        public int RevenueHeadId { get; set; }

        public string InvoiceDescription { get; set; }

        public string Bank { get; set; }

        public string BankBranchName { get; set; }

        public string BankCode { get; set; }

        public string PaymentDateStringVal { get; set; }

        public string PayerId { get; set; }

        public Int64 InvoiceItemCode { get; set; }

        public int PaymentProviderCode { get; set; }
    }
}