using System;
using Orchard.Users.Models;

namespace Parkway.CBS.Core.Models
{
    public class CBSUser : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual string Name { get; set; }

        public virtual TaxEntity TaxEntity { get; set; }

        public virtual UserPartRecord UserPartRecord { get; set; }

        public virtual bool Verified { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string Email { get; set; }

        public virtual string Address { get; set; }

        public virtual bool IsAdministrator { get; set; }

        public virtual bool IsActive { get; set; }
    }

}