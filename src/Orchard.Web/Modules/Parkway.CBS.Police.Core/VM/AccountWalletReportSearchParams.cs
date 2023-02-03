namespace Parkway.CBS.Police.Core.VM
{
    public class AccountWalletReportSearchParams
    {
        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public int BankId { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}