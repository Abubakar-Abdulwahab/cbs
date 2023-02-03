using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSSettlementScheduleUpdate : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PSSSettlementPreFlightBatch PSSSettlementPreFlightBatch { get; set; }

        public virtual PSSSettlement PSSSettlement { get; set; }

        public virtual SettlementRule SettlementRule { get; set; }

        public virtual DateTime CurrentSchedule { get; set; }

        public virtual DateTime NextSchedule { get; set; }

        public virtual DateTime NextStartDate { get; set; }

        public virtual DateTime NextEndDate { get; set; }
    }
}