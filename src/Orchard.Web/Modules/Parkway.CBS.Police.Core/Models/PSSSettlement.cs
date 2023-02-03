using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// This model would hold the settlement schedule details and the service
    /// Here each settlement rule can only be used for one settlement only
    /// </summary>
    public class PSSSettlement : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual SettlementRule SettlementRule { get; set; }

        public virtual PSService Service { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual bool HasCommandSplits { get; set; }
        
    }

}