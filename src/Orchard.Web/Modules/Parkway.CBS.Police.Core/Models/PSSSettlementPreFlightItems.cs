using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementPreFlightItems : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSSettlementPreFlightBatch Batch { get; set; }

        public virtual PSSSettlement PSSSettlement { get; set; }

        public virtual DateTime SettlementScheduleDate { get; set; }

        public virtual DateTime StartRange{ get; set; }

        public virtual DateTime EndRange { get; set; }

        public virtual bool AddToSettlementBatch { get; set; }
    }
}