using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class InitiateAccountWalletPaymentVM
    {
        public List<BankViewModel> Banks { get; set; }

        public List<ExpenditureHeadVM> ExpenditureHeads { get; set; }

        public List<AccountWalletConfigurationVM> AccountWalletConfigurations { get; set; }

        public List<WalletPaymentRequestVM> WalletPaymentRequests { get; set; }
    }
}