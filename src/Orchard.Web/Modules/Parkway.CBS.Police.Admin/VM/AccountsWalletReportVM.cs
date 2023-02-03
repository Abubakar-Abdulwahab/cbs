using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Admin.VM
{
    public class AccountsWalletReportVM
    {
        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public int BankId { get; set; }

        public List<BankViewModel> Banks { get; set; }

        public IEnumerable<Core.HelperModels.AccountsWalletReportVM> AccountsWalletReports { get; set; }

        public dynamic Pager { get; set; }

        public int TotalAccountWalletRecord { get; set; }
    }
}