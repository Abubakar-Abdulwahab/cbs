using System;

namespace Parkway.CBS.Core.Models
{
    public class HangfireJobReference : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string HangfireJobId { get; set; }

        public virtual string JobReferenceNumber { get; set; }
    }
}