using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSBiometricAppUserDetail : CBSBaseModel
    {
        public virtual int Id { get; set; }

        public virtual string MacAddress { get; set; }

        public virtual string Version { get; set; }

        public virtual Command Command { get; set; }

    }
}