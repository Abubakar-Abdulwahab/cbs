using System;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PoliceOfficerDeploymentAllowanceVM
    {
        public long DeploymentAllowanceTrackerId { get; set; }

        public bool IsSettlementCompleted { get; set; }

        public int NumberOfSettlementDone { get; set; }

        public DateTime NextSettlementDate { get; set; }

        public DateTime NextSettlementCycleStartDate { get; set; }

        public DateTime NextSettlementCycleEndDate { get; set; }

        public List<DeploymentAllowanceItemVM> AllowanceItems { get; set; }
    }
}
