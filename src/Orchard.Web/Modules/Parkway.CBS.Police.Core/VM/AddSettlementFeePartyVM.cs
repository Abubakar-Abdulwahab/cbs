using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class AddSettlementFeePartyVM
    {
        public string Name { get; set; }

        public string AccountNumber { get; set; }

        public string SelectedBankCode { get; set; }

        public bool AllowAdditionalCommandSplit { get; set; }

        public List<BankViewModel> Banks { get; set; }

        public List<FeePartyAdapterConfigurationVM> FeePartyAdapterConfigurations { get; set; }

    }
}