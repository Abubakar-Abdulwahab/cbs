using Parkway.CBS.Police.Core.HelperModels;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Admin.VM
{
    public class CommandReportVM
    {
        public string AccountNumber { get; set; }

        public string CommandName { get; set; }

        public IEnumerable<CommandWalletReportVM> CommandWallets { get; set; }

        public dynamic Pager { get; set; }

        public int TotalActiveCommandRecord { get; set; }

        public SettlementAccountType SelectedAccountType { get; set; }
    }
}