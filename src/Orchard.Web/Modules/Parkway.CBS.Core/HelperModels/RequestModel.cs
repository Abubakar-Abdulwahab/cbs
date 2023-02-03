using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class RequestModel
    {
        [JsonIgnore]
        public string URL { get; set; }
        [JsonIgnore]
        public Dictionary<string, dynamic> Headers { get; set; }
        public dynamic Model { get; set; }
    }
}