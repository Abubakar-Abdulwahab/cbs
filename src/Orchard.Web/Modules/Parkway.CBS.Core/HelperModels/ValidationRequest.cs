using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ValidationRequest
    {
        public string InvoiceNumber { get; set; }

        public string mac { get; set; }

        [JsonIgnore]
        public string Signature { get; set; }

        [JsonIgnore]
        public string RequestDump { get; set; }

        [JsonIgnore]
        public bool FlatScheme { get; set; }
    }
}