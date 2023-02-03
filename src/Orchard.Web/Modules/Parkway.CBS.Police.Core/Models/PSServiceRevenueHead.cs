using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    /// <summary>
    /// Service Revenue head is a table that lists the services and the respective revenue head for that service
    /// More or less a joiner between the service and a revenue head
    /// </summary>
    public class PSServiceRevenueHead : CBSModel
    {
        public virtual PSService Service { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual string Description { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

    }

}