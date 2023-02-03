using Orchard.Users.Models;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.Models
{
    public class TenantCBSSettings : CBSBaseModel
    {
        public virtual int Id { get; set; }

        [StringLength(30, ErrorMessage ="Identifier value should be between 5 to 30 characters",MinimumLength =5)]
        public virtual string Identifier { get; set; }

        public virtual string StateName { get; set; }

        public virtual int StateId { get; set; }

        public virtual int CountryId { get; set; }

        public virtual string CountryName { get; set; }

        public virtual LGA DefaultLGA { get; set; }

        /// <summary>
        /// Added by admin user
        /// </summary>
        public virtual UserPartRecord AddedBy { get; set; }

        /// <summary>
        /// last updated by admin user
        /// </summary>
        public virtual UserPartRecord LastUpdatedBy { get; set; }
    }
}