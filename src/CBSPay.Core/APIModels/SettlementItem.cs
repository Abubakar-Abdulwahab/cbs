namespace CBSPay.Core.APIModels
{
    public class SettlementItem
    {
        public long TBPKID { get; set; }
        public decimal ToSettleAmount { get; set; }
        public decimal TaxAmount { get; set; }
    }
    public class SettlementItemDetail
    {
        public int TBPKID { get; set; }
        public string ToSettleAmount { get; set; }
        public string TaxAmount { get; set; }
    }
}