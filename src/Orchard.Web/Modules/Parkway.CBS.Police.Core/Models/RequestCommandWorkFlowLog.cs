using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class RequestCommandWorkFlowLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual Command Command { get; set; }

        public virtual PSServiceRequestFlowDefinitionLevel DefinitionLevel { get; set; }

        public virtual bool IsActive { get; set; }

        /// <summary>
        /// <see cref="Enums.RequestPhase"/>
        /// </summary>
        public virtual int RequestPhaseId { get; set; }

        public virtual string RequestPhaseName { get; set; }
    }
}