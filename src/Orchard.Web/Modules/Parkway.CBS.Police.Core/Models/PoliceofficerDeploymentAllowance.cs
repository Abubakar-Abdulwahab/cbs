using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceofficerDeploymentAllowance : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual Invoice Invoice { get; set; }

        /// <summary>
        /// <see cref="Enums.DeploymentAllowanceStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        public virtual PoliceOfficer PoliceOfficer { get; set; }

        /// <summary>
        /// Allowance payment amount
        /// </summary>
        public virtual decimal Amount { get; set; }

        /// <summary>
        /// Amount officer contributed to the total invoice amount
        /// </summary>
        public virtual decimal ContributedAmount { get; set; }


        /// <summary>
        /// Initiated by admin user
        /// </summary>
        public virtual UserPartRecord InitiatedBy { get; set; }

        public virtual string Comment { get; set; }

        public virtual string Narration { get; set; }

        /// <summary>
        /// Settlment Engine Reference Number
        /// </summary>
        public virtual string SettlementReferenceNumber { get; set; }

        /// <summary>
        /// Final allowance schedule job id
        /// </summary>
        public virtual string ScheduleJob_Id { get; set; }

        /// <summary>
        /// <see cref="Enums.PSSAllowancePaymentStage"/>
        /// </summary>
        public virtual int PaymentStage { get; set; }

        public virtual Command Command { get; set; }

        public virtual PolicerOfficerLog PoliceOfficerLog { get; set; }

        public virtual PSSEscortDetails EscortDetails { get; set; }

    }
}