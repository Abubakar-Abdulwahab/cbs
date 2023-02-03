using Orchard.Users.Models;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.Models
{
    public class TaxClearanceCertificateRequest : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string ApplicantName { get; set; }

        public virtual string ResidentialAddress { get; set; }

        public virtual string OfficeAddress { get; set; }

        public virtual string Occupation { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string TIN { get; set; }

        public virtual bool IsRentedApartment { get; set; }

        public virtual string LandlordName { get; set; }

        public virtual string LandlordAddress { get; set; }

        public virtual string RequestReason { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        /// <summary>
        /// <see cref="TCCExemptionType"/>
        /// </summary>
        public virtual int ExemptionCategory { get; set; }

        /// <summary>
        /// For exempted wife
        /// </summary>
        public virtual string HusbandName { get; set; }

        /// <summary>
        /// For exempted wife
        /// </summary>
        public virtual string HusbandAddress { get; set; }

        /// <summary>
        /// For exempted student
        /// </summary>
        public virtual string InstitutionName { get; set; }

        /// <summary>
        /// For exempted student
        /// </summary>
        public virtual string IdentificationNumber { get; set; }

        /// <summary>
        /// <see cref="TCCRequestStatus"/>
        /// </summary>
        public virtual int Status { get; set; }

        public virtual string ApplicationNumber { get; set; }

        public virtual Invoice DevelopmentLevyInvoice { get; set; }

        public virtual IEnumerable<TaxClearanceCertificateRequestFiles> FileUploads { get; set; }

        public virtual string TCCNumber { get; set; }

        /// <summary>
        /// Year the TCC is being requested for
        /// </summary>
        public virtual int ApplicationYear { get; set; }

        /// <summary>
        /// <see cref="TCCApprovalLevel"/>
        /// </summary>
        public virtual int ApprovalStatusLevelId { get; set; }

    }
}