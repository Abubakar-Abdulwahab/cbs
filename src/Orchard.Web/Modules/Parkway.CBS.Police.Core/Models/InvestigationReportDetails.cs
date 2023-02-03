using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class InvestigationReportDetails : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual string RequestReason { get; set; }

        public virtual int SelectedCategory { get; set; }

        public virtual int SelectedSubCategory { get; set; }
    }
}