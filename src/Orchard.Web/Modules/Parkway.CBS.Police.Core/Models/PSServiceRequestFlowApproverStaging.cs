using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSServiceRequestFlowApproverStaging : CBSModel
    {
        /// <summary>
        /// this value will hold the user that has been asssigned to perform the
        /// action value 
        /// </summary>
        public virtual UserPartRecord AssignedApprover { get; set; }

        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

        public virtual PSSAdminUsers PSSAdminUser { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string Reference { get; set; }


    }
}