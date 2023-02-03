using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSService : CBSModel
    {
        public virtual string Name { get; set;}

        public virtual bool IsActive { get; set; }

        public virtual ICollection<PSSRequest> Requests { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSServiceTypeDefinition"/>
        /// </summary>
        public virtual int ServiceType { get; set; }

        /// <summary>
        /// this holds service specific note worhty information.
        /// </summary>
        public virtual string ServiceNotes { get; set; }

        public virtual string ServicePrefix { get; set; }

        /// <summary>
        /// this describes the flow to approval for requests made on this service
        /// </summary>
        public virtual PSServiceRequestFlowDefinition FlowDefinition { get; set; }

        /// <summary>
        /// this flag would help us ascertain if the service has a different workflow 
        /// based on some certain differential values as specificed by the service implementation
        /// </summary>
        public virtual bool HasDifferentialWorkFlow { get; set; }

        /// <summary>
        /// Side note partial name
        /// </summary>
        public virtual string SideNotePartialName { get; set; }
    }
}