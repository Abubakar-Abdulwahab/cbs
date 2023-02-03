namespace Parkway.CBS.Police.Core.DTO
{
    public class AccountWalletConfigurationDTO
    {
        public int BankId { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public int FlowDefinitionId { get; set; }

        public int CommandId { get; set; }
    }
}