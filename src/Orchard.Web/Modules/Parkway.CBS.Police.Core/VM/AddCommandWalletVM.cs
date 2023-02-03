using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class AddCommandWalletVM
    {
        public string WalletNumber { get; set; }

        public List<CommandVM> Commands { get; set; }

        public List<BankViewModel> Banks { get; set; }

        public int SelectedCommandId { get; set; }

        public int SelectedBankId { get; set; }

        public SettlementAccountType SelectedAccountType { get; set; }

    }
}