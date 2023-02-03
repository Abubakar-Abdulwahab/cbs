using Newtonsoft.Json;
using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSExternalDataSourceConfigurationSettingVM
    {
        public int ExternalDataSourceConfigId { get; set; }

        public string ActionName { get; set; }

        public string ImplementingClass { get; set; }

        public string CRONValue { get; set; }

        public DateTime NextScheduleDate { get; set; }

    }
}
