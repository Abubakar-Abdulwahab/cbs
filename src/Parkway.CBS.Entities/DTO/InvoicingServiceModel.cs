using Parkway.Cashflow.Ng.Models.Enums;

namespace Parkway.CBS.Entities.DTO
{
    public class InvoicingServiceInvoiceGenerationModel
    {
        public RevenueHeadDetailsForInvoiceGenerationLite RevenueHeadDetails { get; set; }

        public string Recipient { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public string ExternalRefNumber { get; set; }

        public decimal Amount { get; set; }

        public CashFlowCustomerType Type { get; set; }

        public string FootNotes { get; set; }

        public string UniqueInvoiceIdentifier { get; set; }

        public string InvoiceDescription { get; set; }

    }
}
