using Newtonsoft.Json;
using System.Collections.Generic;

namespace Parkway.CBS.RemoteClient
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
