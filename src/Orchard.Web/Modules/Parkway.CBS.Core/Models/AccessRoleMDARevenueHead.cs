using System;
using Orchard.Users.Models;


namespace Parkway.CBS.Core.Models
{
    public class AccessRoleMDARevenueHead : CBSBaseModel
    {
        public virtual Int64 Id { get; set; }

        public virtual AccessRole AccessRole { get; set; }

        public virtual MDA MDA { get; set; }

        public virtual RevenueHead RevenueHead { get; set; }

        public virtual UserPartRecord AddedBy { get; set; }
    }
}