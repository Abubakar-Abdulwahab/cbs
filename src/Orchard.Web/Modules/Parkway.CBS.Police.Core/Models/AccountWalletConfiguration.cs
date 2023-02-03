using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class AccountWalletConfiguration : CBSModel
    {
        public virtual PSSFeeParty PSSFeeParty { get; set; }

        public virtual CommandWalletDetails CommandWalletDetail { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

        public virtual PSServiceRequestFlowDefinition FlowDefinition { get; set; }
       
        public virtual bool IsDeleted { get; set; }

    }
}