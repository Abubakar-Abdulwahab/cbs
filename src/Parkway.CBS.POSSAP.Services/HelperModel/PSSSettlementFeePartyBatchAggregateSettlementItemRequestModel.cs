namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSSettlementFeePartyBatchAggregateSettlementItemRequestModel
    {
        public string AccountName { get; set; }

        public decimal Amount { get; set; }

        public string Narration { get; set; }

        public string AccountNumber { get; set; }

        public string BankCode { get; set; }
    }
}
