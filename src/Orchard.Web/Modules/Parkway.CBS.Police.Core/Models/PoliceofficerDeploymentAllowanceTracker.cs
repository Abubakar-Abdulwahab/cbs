using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceofficerDeploymentAllowanceTracker : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual bool IsSettlementCompleted { get; set; }

        /// <summary>
        /// Tracks number of settlement done
        /// </summary>
        public virtual int NumberOfSettlementDone { get; set; }

        /// <summary>
        /// The beginning of the current settlement to be settled
        /// </summary>
        public virtual DateTime SettlementCycleStartDate { get; set; }

        /// <summary>
        /// The end of the current settlement to be settled
        /// </summary>
        public virtual DateTime SettlementCycleEndDate { get; set; }

        /// <summary>
        /// The next date to settle the officers for this deployment
        /// </summary>
        public virtual DateTime NextSettlementDate { get; set; }

        public virtual PSSEscortDetails EscortDetails { get; set; }
    }
}