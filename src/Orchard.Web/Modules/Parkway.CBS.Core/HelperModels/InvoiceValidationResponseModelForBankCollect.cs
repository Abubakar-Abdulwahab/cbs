namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceValidationResponseModelForBankCollect
    {
        public string ResponseCode { get; set; }

        public string ResponseDescription { get; set; }

        public string Recipient { get; set; }

        public decimal Amount { get; set; }

        public string InvoiceNumber { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

    }
}