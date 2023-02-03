using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class InitiateDeploymentAllowancePaymentVM
    {
        public List<AccountWalletConfigurationVM> AccountWalletConfigurations { get; set; }

        public int SelectedSourceAccountId { get; set; }

        public string SelectedSourceAccountName { get; set; }

        public string InvoiceNumber { get; set; }

        public List<BankViewModel> Banks { get; set; }

        public IEnumerable<CommandTypeVM> CommandTypes { get; set; }

        public IEnumerable<PSSEscortDayTypeDTO> EscortDayTypes { get; set; }

        public List<DeploymentAllowancePaymentRequestItemVM> DeploymentAllowancePaymentRequests { get; set; }
    }
}