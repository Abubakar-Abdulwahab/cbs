namespace Parkway.CBS.Core.HelperModels
{
    public class TransactionLogInvoiceDetails
    {
        public string MDAName { get; set; }

        public string MDACode { get; set; }

        public string RevenueHeadName { get; set; }

        public string RevenueHeadCode { get; set; }

        public decimal AmountPaid { get; set; }
    }
}