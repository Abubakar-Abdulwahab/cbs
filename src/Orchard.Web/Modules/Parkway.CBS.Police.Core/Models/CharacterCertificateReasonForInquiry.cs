using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class CharacterCertificateReasonForInquiry : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool FreeForm { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}