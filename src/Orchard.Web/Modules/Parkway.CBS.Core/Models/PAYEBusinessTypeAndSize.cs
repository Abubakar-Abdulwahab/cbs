using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class PAYEBusinessTypeAndSize : CBSModel
    {
        public virtual PAYEBusinessSize PAYEBusinessSize { get; set; }

        public virtual PAYEBusinessType PAYEBusinessType { get; set; }

        public virtual decimal Amount { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }
    }
}