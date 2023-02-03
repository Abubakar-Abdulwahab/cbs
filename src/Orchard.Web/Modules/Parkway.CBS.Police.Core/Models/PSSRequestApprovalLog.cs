using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRequestApprovalLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSRequestStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        /// <summary>
        /// this indicates what stage this request was made for
        /// </summary>
        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

        public virtual UserPartRecord AddedByAdminUser { get; set; }

        public virtual string Comment { get; set; }

    }
}