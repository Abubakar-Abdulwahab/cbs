namespace Parkway.CBS.Core.Models
{
    public class TaxPayerEnumerationItemsStaging : CBSBaseModel
    {
        /// <summary>
        /// Taxpayer Address
        /// </summary>
        public virtual string Address { get; set; }

        /// <summary>
        /// Taxpayer Email
        /// </summary>
        public virtual string Email { get; set; }

        public virtual long Id { get; set; }

        /// <summary>
        /// Taxpayer Local Government Area
        /// </summary>
        public virtual string LGA { get; set; }

        /// <summary>
        /// Taxpayer Phone number
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// Taxpayer Name
        /// </summary>
        public virtual string TaxPayerName { get; set; }

        /// <summary>
        /// Taxpayer TIN
        /// </summary>
        public virtual string TIN { get; set; }

        public virtual bool HasErrors { get; set; }

        public virtual string ErrorMessages { get; set; }

        /// <summary>
        /// Enumeration batch the item belongs to
        /// </summary>
        public virtual TaxPayerEnumeration TaxPayerEnumeration { get; set; }
    }
}