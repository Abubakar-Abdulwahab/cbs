using Newtonsoft.Json;
using Parkway.CBS.Core.Payee;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class DirectAssessmentModel
    {
        public bool AllowFileUpload { get; set; }

        public string AdapterValue { get; set; }

        [JsonIgnore]
        public List<AssessmentInterface> DirectAssessmentAdapters { get; set; }

        [JsonIgnore]
        public string FilePath { get; set; }

        [JsonIgnore]
        public string RCNumber { get; set; }

        [JsonIgnore]
        public string CompanyName { get; set; }
    }
}