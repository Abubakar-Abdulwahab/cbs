using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxProfilesSearchParams
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name field must be between 2 to 100 characters long.")]
        public string Name { get; set; }

        [StringLength(11, MinimumLength = 11, ErrorMessage = "Add a valid mobile phone number.")]
        public string PhoneNumber { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "TIN field must be between 3 to 20 characters long.")]
        public string TIN { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Payer Id field must be between 3 to 20 characters long.")]
        public string PayerId { get; set; }

        public int CategoryId { get; set; }

        public int Page { get; set; }

        [Range(0, 100, ErrorMessage = "Page size limit is 100 records.")]
        public int PageSize { get; set; }
    }
}