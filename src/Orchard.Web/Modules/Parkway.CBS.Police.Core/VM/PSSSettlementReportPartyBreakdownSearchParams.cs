namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportPartyBreakdownSearchParams
    {
        public long FeePartyBatchAggregateId { get; set; }

        public int FeePartyId { get; set; }

        public string BatchRef { get; set; }

        public int CommandId { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public bool PageData { get; set; }
    }
}