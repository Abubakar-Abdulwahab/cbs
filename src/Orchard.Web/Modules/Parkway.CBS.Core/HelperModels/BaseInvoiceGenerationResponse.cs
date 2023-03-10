namespace Parkway.CBS.Core.HelperModels
{
    public abstract class BaseInvoiceGenerationResponse
    {
        public string InvoiceNumber { get; set; }

        public string InvoicePreviewUrl { get; set; }

        public decimal AmountDue { get; set; }
    }
}