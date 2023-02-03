using System;

namespace Parkway.CBS.Entities.DTO
{
    public class IPPISGenerateInvoiceModel
    {
        public string Recipient { get; set; }

        public decimal Amount { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        /// <summary>
        /// tax profile ID. This is used to uniquely identify the customer on cashflow
        /// </summary>
        public Int64 TaxProfileId { get; set; }

        public int TaxProfileCategoryId { get; set; }

        public Int64 CashflowCustomerId { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public string AgencyCode { get; set; }

        public long IPPISTaxPayerSummaryId { get; set; }
    }
}
