namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportBatchBreakdownSearchParams
    {
        public string BatchRef { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public bool PageData { get; set; }
    }
}