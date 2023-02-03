using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class RequestStatusLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual string StatusDescription { get; set; }

        public virtual Invoice Invoice { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSRequestStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        public virtual PSServiceRequestFlowDefinitionLevel FlowDefinitionLevel { get; set; }

        public virtual bool UserActionRequired { get; set; }

        public virtual bool Fulfilled { get; set; }

    }
}