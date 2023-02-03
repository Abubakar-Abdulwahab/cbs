using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSFeePartyAdapterConfiguration : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual string ImplementingClass { get; set; }

        public virtual bool IsActive { get; set; }

    }
}