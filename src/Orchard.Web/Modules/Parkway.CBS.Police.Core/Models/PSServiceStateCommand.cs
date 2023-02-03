using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceStateCommand : CBSModel
    {
        public virtual PSServiceState ServiceState { get; set; }

        public virtual Command Command { get; set; }

        public virtual bool IsActive { get; set; }
    }
}