using System;
using System.Collections.Generic;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSEscortDetails : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual int NumberOfOfficers { get; set; }

        public virtual int PaymentPeriodType { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual string Address { get; set; }

        public virtual int DurationNumber { get; set; }

        public virtual int DurationType { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime EndDate { get; set; }

        public virtual TaxEntitySubCategory TaxEntitySubCategory { get; set; }

        public virtual TaxEntitySubSubCategory TaxEntitySubSubCategory { get; set; }

        /// <summary>
        /// indicates that officers have been assigned
        /// <para>Doesn't neccessarily mean that the list of officers have been confirmed final</para>
        /// </summary>
        public virtual bool OfficersHaveBeenAssigned { get; set; }

        public virtual ICollection<ProposedEscortOfficer> ProposedOfficers { get; set; }

        public virtual PSSEscortSettings Settings { get; set; }

        public virtual PSSEscortServiceCategory ServiceCategory { get; set; }

        public virtual PSSEscortServiceCategory CategoryType { get; set; }

        public virtual StateModel OriginState { get; set; }

        public virtual LGA OriginLGA { get; set; }

        public virtual string OriginAddress { get; set; }

        public virtual CommandType CommandType { get; set; }

    }
}