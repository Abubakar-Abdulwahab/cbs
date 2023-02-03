using System;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PoliceOfficerDeploymentLog : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string OfficerName { get; set; }

        /// <summary>
        /// this prop will hold the address as per the request.
        /// <para>This prop will serve as the immutable value for this deployment</para>
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// this prop will hold the customer name as per the request
        /// <para>This prop will serve as the immutable value for this deployment</para>
        /// </summary>
        public virtual string CustomerName { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        /// <summary>
        /// This prop should hold the police officer who
        /// this deployment is for
        /// This prop should be treated as immutable
        /// </summary>
        public virtual PoliceOfficer PoliceOfficer { get; set; }

        /// <summary>
        /// This prop should hold the command the police officer is
        /// attached to as of the time this deployment was made
        /// this prop should be treated as immutable
        /// </summary>
        public virtual Command Command { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual Invoice Invoice { get; set; }

        /// <summary>
        /// This shows the rank of the officer as of the time deployment was made
        /// this value is immutable
        /// </summary>
        public virtual PoliceRanking OfficerRank { get; set; }


        public virtual bool IsActive { get; set; }

        public virtual int Status { get; set; }

        public virtual decimal DeploymentRate { get; set; }

        /// <summary>
        /// The Officer replacing the initial {PoliceOfficer} of this deployment
        /// </summary>
        public virtual PoliceOfficer RelievingOfficer { get; set; }

        public virtual string DeploymentEndReason { get; set; }

        public virtual UserPartRecord DeploymentEndBy { get; set; }

        public virtual PolicerOfficerLog PoliceOfficerLog { get; set; }

        public virtual PolicerOfficerLog RelievingOfficerLog { get; set; }

    }
}