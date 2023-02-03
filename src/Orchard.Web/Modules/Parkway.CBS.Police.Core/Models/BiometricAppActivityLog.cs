using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class BiometricAppActivityLog : CBSModel
    {
        public virtual string MacAddress { get; set; }

        public virtual string IPAddress { get; set; }

        public virtual string Version { get; set; }
    }
}