using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class ProposedEscortOfficerSelectionTracking : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSEscortDetails EscortDetails { get; set; }

        public virtual PolicerOfficerLog OfficerLog { get; set; }

        public virtual decimal EscortRankRate { get; set; }

        public virtual bool IsDeleted { get; set; }

        public virtual string Reference { get; set; }
    }
}