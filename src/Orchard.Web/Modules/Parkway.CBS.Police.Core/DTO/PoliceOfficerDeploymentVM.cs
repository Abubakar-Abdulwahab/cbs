using Orchard.Users.Models;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PoliceOfficerDeploymentVM 
    {
        public string FileRefNumber { get; set; }

        public string ApprovalNumber { get; set; }

        public Int64 Id { get; set; }

        /// <summary>
        /// this prop will hold the address as per the request.
        /// <para>This prop will serve as the immutable value for this deployment</para>
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// this prop will hold the customer name as per the request
        /// <para>This prop will serve as the immutable value for this deployment</para>
        /// </summary>
        public string CustomerName { get; set; }

        public Int64 RequestId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        /// <summary>
        /// police officer Id
        /// </summary>
        public Int64 PolicerOfficerId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PoliceOfficerName { get; set; }

        public string PoliceOfficerRank { get; set; }

        public string PoliceOfficerIPPIS { get; set; }

        public string PoliceOfficerIdentificationNumber { get; set; }

        /// <summary>
        /// This prop should hold the command the police officer is
        /// attached to as of the time this deployment was made
        /// this prop should be treated as immutable
        /// </summary>
        public string CommandName { get; set; }


        public string StateName { get; set; }

        public string LGAName { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        public bool IsActive { get; set; }

        public int Status { get; set; }


        public object InvoiceNumber { get; set; }

        public DeploymentStatus GetStatus()
        {
            return (DeploymentStatus)this.Status;
        }

    }
}