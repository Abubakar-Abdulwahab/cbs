using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.POSSAP.Scheduler.Models
{
    public class PSSExternalDataLGAStaging : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual string StateCode { get; set; }

        public virtual CallLogForExternalSystem CallLogForExternalSystem { get; set; }

        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }

    }
}