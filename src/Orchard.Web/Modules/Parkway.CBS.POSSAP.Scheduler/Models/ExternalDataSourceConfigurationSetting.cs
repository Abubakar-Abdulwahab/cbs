using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.POSSAP.Scheduler.Models
{
    public class ExternalDataSourceConfigurationSetting : CBSModel
    {
        public virtual string ActionName { get; set; }

        public virtual string ImplementingClass { get; set; }

        public virtual string CRONValue { get; set; }

        public virtual DateTime NextScheduleDate { get; set; }

        public virtual bool IsActive { get; set; }

    }
}