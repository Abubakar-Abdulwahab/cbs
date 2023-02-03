using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxEntityViewModel
    {
        [StringLength(100, MinimumLength =3, ErrorMessage = "Full name field must be between 3 to 100 characters long.")]
        [Required(ErrorMessage = "Full name field is required", AllowEmptyStrings = false)]
        public string Recipient { get; set; }

        [EmailAddress]
        //[Required(ErrorMessage ="Email field is required", AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Email field must be between 3 to 50 characters long.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address field is required", AllowEmptyStrings = false)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Address field must be between 5 to 100 characters long.")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "Phone number field is required", AllowEmptyStrings = false)]
        [StringLength(13, MinimumLength = 11, ErrorMessage = "Add a valid mobile phone number.")]
        public string PhoneNumber { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "TIN field must be between 3 to 20 characters long.")]
        public string TaxPayerIdentificationNumber { get; set; }

        [StringLength(200, MinimumLength = 3, ErrorMessage = "External Bill Number must be between 3 to 200 characters long.")]
        public string ExternalBillNumber { get; set; }

        public string PayerId { get; set; }

        public long Id { get; set; }

        public long CashflowCustomerId { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public int ExpertSystemId { get; set; }

        public int SelectedState { get; set; }

        public int SelectedStateLGA { get; set; }

        [StringLength(20, MinimumLength = 5, ErrorMessage = "RC Number must be between 5 to 20 characters long.")]
        public string RCNumber { get; set; }

        public string SelectedStateName { get; set; }

        public string SelectedLGAName { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The contact person name field value must be between 5 and 255 characters")]
        public string ContactPersonName { get; set; }

        [EmailAddress]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The contact person email field value must be between 2 and 100 characters")]
        public string ContactPersonEmail { get; set; }

        [StringLength(14, MinimumLength = 6, ErrorMessage = "Add a valid phone number")]
        public string ContactPersonPhoneNumber { get; set; }

        public int IdType { get; set; }

        public string IdNumber { get; set; }

        /// <summary>
        /// Holds the default LGA Id for the expert system
        /// </summary>
        public int DefaultLGAId { get; set; }

        public int Gender { get; set; }
    }
}