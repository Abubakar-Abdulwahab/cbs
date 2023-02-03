namespace Parkway.CBS.Police.Core.VM
{
    public class CommandWalletReportSearchParams
    {
        public string AccountNumber { get; set; }

        public string CommandName { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int SelectedAccountType { get; set; }
    }
}