namespace Parkway.CBS.Police.Core.VM
{
    public class SettlementFeePartyVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public string BankCode { get; set; }

        public bool AllowAdditionalCommandSplit { get; set; }

        public FeePartyAdapterConfigurationVM FeePartyAdapterConfiguration { get; set; }
    }
}