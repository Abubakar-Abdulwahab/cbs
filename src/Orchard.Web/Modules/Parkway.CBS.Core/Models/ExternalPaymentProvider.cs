using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class ExternalPaymentProvider : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual string ClassImplementation { get; set; }

        public virtual string ClientID { get; set; }

        public virtual string ClientSecret { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }

        public virtual bool AllowAgentFeeAddition { get; set; }
    }
}