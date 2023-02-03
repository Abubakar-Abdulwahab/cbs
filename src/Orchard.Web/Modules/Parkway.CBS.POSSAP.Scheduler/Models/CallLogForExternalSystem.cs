using System;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.POSSAP.Scheduler.Models
{
    public class CallLogForExternalSystem : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string URL { get; set; }

        public virtual string CallDescription { get; set; }

        public virtual ExternalDataSourceConfigurationSetting ExternalDataSourceConfigurationSetting { get; set; }

        public virtual int CallStatus { get; set; }

        public virtual string JSONCallParameters { get; set; }

        public virtual bool CallIsSuccessful { get; set; }

        public virtual string ErrorMessage { get; set; }
    }
}