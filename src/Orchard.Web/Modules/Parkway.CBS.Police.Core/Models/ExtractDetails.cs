using System;
using System.Collections.Generic;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class ExtractDetails : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual string RequestReason { get; set; }

        public virtual int SelectedCategory { get; set; }

        public virtual int SelectedSubCategory { get; set; }

        public virtual bool IsIncidentReported { get; set; }

        public virtual string IncidentReportedDate { get; set; }

        public virtual string AffidavitNumber { get; set; }

        public virtual DateTime? AffidavitDateOfIssuance { get; set; }

        public virtual IEnumerable<ExtractRequestFiles> ExtractFiles { get; set; }

        public virtual string DiarySerialNumber { get; set; }

        public virtual DateTime? IncidentDateAndTime { get; set; }

        public virtual string CrossReferencing { get; set; }

        public virtual string Content { get; set; }

        public virtual string DPOServiceNumber { get; set; }

        public virtual string DPOName { get; set; }

        public virtual UserPartRecord DPOAddedBy { get; set; }

        public virtual string DPORankCode { get; set; }

    }
}