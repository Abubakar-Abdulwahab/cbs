using System;
using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceServiceRequest : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual PSService Service { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSRequestStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// this indicates what stage this request was made for
        /// </summary>
        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

    }
}