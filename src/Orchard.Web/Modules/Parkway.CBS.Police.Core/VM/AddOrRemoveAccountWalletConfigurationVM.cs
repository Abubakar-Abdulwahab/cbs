using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class AddOrRemoveAccountWalletConfigurationVM
    {
        public int AccountWalletId { get; set; }

        public List<DTO.PSServiceRequestFlowDefinitionLevelDTO> FlowDefinitionLevels { get; set; }

        public List<WalletUsersVM> WalletUsers { get; set; }

        public List<WalletUsersVM> AddedWalletUsers { get; set; }

        public List<WalletUsersVM> RemovedWalletUsers { get; set; }

        public string WalletName { get; set; }
    }
}