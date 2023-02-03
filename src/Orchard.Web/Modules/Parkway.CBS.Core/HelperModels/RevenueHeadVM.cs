namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int MdaId { get; set; }

        public string InvoiceGenerationRedirectURL { get; set; }

        public long CashflowProductId { get; set; }
        public string Address { get; set; }
    }
}