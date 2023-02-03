using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBusinessType : CBSModel
    {

        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }
    }
}