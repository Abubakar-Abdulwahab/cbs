using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSRequestApprovalDocumentPreviewLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual UserPartRecord Approver { get; set; }

        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

        public virtual string RequestDocumentDraftBlob { get; set; }

        public virtual bool Confirmed { get; set; }
    }
}