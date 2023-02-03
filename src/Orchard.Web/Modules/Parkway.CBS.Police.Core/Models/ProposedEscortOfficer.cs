using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class ProposedEscortOfficer : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSEscortDetails EscortDetails { get; set; }

        public virtual PoliceOfficer Officer { get; set; }

        public virtual PolicerOfficerLog OfficerLog { get; set; }

        public virtual decimal EscortRankRate { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}