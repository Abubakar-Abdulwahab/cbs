using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Models
{
    public class PSSStateModelExternalDataState : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual StateModel State { get; set; }

        public virtual string ExternalDataStateCode { get; set; }

        public virtual CallLogForExternalSystem CallLogForExternalSystem { get; set; }
    }
}