namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CommandWalletDetailsVM
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; }

        public string Name { get; set; }

        public string BankCode { get; set; }

        public bool IsActive { get; set; }
    }

}