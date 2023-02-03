using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSEscortSettings : CBSModel
    {
        public virtual UserPartRecord AdminToAssignOfficers { get; set; }

        public virtual PSServiceRequestFlowDefinition WorkFlowDefinition { get; set; }
    }
}