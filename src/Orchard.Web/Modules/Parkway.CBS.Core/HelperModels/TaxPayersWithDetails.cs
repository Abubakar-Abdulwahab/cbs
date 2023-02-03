using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxPayerWithDetails
    {
        public Int64 Id { get; set; }

        public string TIN { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Email { get; set; }

        public int CategoryId { get; set; }

        public string PayerId { get; set; }

        public string TaxPayerCode { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public Int64 CustomerId { get; set; }

        /// <summary>
        /// used for OSGOF now, to identify the prefix for each category
        /// </summary>
        public string CategoryStringIdentifier { get; set; }
    }
}