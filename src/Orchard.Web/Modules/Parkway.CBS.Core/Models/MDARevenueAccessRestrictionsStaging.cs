using Orchard.Users.Models;
using System;

namespace Parkway.CBS.Core.Models
{
    public class MDARevenueAccessRestrictionsStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual bool IsRemoval { get; set; }

        public virtual MDARevenueHeadEntryStaging MDARevenueHeadEntryStaging { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}