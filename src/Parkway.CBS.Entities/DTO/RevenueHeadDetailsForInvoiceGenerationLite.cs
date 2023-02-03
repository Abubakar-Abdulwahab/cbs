using System;

namespace Parkway.CBS.Entities.DTO
{
    public class RevenueHeadDetailsForInvoiceGenerationLite
    {
        public string SMEKey { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string JSONBillingPenalties { get; set; }

        public string JSONBillingDiscounts { get; set; }

        public string JSONDueDate { get; set; }

        public string RevenueHeadNameAndCode { get; set; }

        public Int64 CashFlowProductId { get; set; }

        public DateTime? NextBillingDate { get; set; }

        public string InvoiceDescription { get; set; }

        public decimal Amount { get; set; }

        public string ExternalRefNumber { get; set; }

        public int StateId { get; set; }

        public int BillingModelId { get; set; }

        public int RevenueHeadId { get; set; }

        public int MDAId { get; set; }

        public int ExpertSystemId { get; set; }
    }
}