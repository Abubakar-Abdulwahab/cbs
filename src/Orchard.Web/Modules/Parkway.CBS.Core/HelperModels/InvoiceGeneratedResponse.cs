namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceGeneratedResponse
    {
        public string InvoiceNumber { get; set; }

        public string InvoicePreviewUrl { get; set; }

        public decimal AmountDue { get; set; }
    }
}