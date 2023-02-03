using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class AccountWalletConfigurationPSServiceRequestFlowApprover : CBSModel
    {
        public virtual AccountWalletConfiguration AccountWalletConfiguration { get; set; }

        public virtual PSServiceRequestFlowApprover PSServiceRequestFlowApprover { get; set; }

        public virtual bool IsDeleted { get; set; }

    }
}