using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSHangfireSettlementReference : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string HangfireJobId { get; set; }

        /// <summary>
        /// <see cref="Enums.HangfireReferenceType"/>
        /// </summary>
        public virtual int ReferenceType { get; set; }

        public virtual Int64 ReferenceId { get; set; }
    }
}