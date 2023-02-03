using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSExpenditureHead : CBSBaseModel
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual UserPartRecord LastUpdatedBy { get; set; }

    }
}