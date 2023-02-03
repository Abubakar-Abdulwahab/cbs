using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Models
{
    public class PSSExternalDataStateStaging : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual CallLogForExternalSystem CallLogForExternalSystem { get; set; }

        public virtual bool HasErorr { get; set; }

        public virtual string ErrorMessage { get; set; }

    }
}