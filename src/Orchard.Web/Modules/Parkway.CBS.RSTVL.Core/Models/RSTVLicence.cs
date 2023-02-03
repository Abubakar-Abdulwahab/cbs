using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.RSTVL.Core.Models
{
    public class RSTVLicence : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string CustomerReference { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }
        
        public virtual RevenueHead RevenueHead { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual Invoice Invoice { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual int Year { get; set; }

        public virtual ExpertSystemSettings ClaimedBy { get; set; }

        /// <summary>
        /// RSTVLicenseStatus
        /// <see cref="RSTVLicenseStatus"/>
        /// </summary>
        public virtual int Status { get; set; }
    }
}