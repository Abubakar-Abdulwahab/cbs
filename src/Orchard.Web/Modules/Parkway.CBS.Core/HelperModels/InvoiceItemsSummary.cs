namespace Parkway.CBS.Core.HelperModels
{
    public class InvoiceItemsSummary
    {
        public int RevenueHeadId { get; set; }

        public string RevenueHeadName { get; set; }

        public string MDAName { get; set; }

        public decimal UnitAmount { get; set; }

        public int Quantity { get; set; }

        public int Status { get; set; }

        public long Id { get; set; }

        public decimal TotalAmount { get; set; }

        public int MDAId { get; set; }
    }
}