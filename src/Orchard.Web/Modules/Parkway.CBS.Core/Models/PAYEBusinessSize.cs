using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBusinessSize : CBSModel
    {
        public virtual string Size { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }
    }
}