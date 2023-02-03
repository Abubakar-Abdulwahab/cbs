using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSDispatchNote : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string ApplicantName { get; set; }

        public virtual string ApprovalNumber { get; set; }

        public virtual string FileRefNumber { get; set; }

        public virtual string OriginStateName { get; set; }

        public virtual string OriginLGAName { get; set; }

        public virtual string OriginAddress { get; set; }

        public virtual string ServiceDeliveryStateName { get; set; }

        public virtual string ServiceDeliveryLGAName { get; set; }

        public virtual string ServiceDeliveryAddress { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        /// <summary>
        /// JSON string of IEnumerable<CommandVM> Collection containing all the commands handling the request
        /// </summary>
        public virtual string ServicingCommands { get; set; }

        /// <summary>
        /// JSON string of IEnumerable<PoliceOfficerLogVM> Collection containing the assigned officers
        /// </summary>
        public virtual string PoliceOfficers { get; set; }

        public virtual string DispatchNoteTemplate { get; set; }
    }
}