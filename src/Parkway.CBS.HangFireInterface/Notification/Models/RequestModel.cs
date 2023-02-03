using Newtonsoft.Json;
using System.Collections.Generic;

namespace Parkway.CBS.HangFireInterface.Notification.Models
{
    public class RequestModel
    {
        [JsonIgnore]
        public Dictionary<string, dynamic> Headers { get; set; }
        public dynamic Model { get; set; }
    }
}
