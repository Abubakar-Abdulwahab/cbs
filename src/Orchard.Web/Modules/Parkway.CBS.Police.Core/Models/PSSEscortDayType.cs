using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSEscortDayType : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }
    }
}