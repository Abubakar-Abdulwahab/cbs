namespace Parkway.CBS.Police.Core.VM
{
    public class SettlementFeePartiesSearchParams
    {
        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int SettlementId { get; set; }
    }
}