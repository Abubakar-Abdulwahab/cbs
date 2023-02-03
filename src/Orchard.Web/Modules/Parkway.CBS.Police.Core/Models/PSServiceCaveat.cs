using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceCaveat : CBSModel
    {
        public virtual string CaveatHeader { get; set; }

        public virtual string CaveatContent { get; set; }

        public virtual PSService Service { get; set; }

        public virtual bool IsActive { get; set; }
    }
}