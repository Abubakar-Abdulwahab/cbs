using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Models
{
    public class PSSLGAModelExternalDataLGA : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual StateModel State { get; set; }

        public virtual LGA LGA { get; set; }

        public virtual string ExternalDataLGACode { get; set; }

        public virtual string ExternalDataLGAStateCode { get; set; }

        public virtual CallLogForExternalSystem CallLogForExternalSystem { get; set; }
    }
}