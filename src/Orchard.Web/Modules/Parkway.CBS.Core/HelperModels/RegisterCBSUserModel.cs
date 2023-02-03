using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class RegisterUserModel
    {
        [Required(ErrorMessage = "This field is required")]
        public int SelectedState { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public int SelectedStateLGA { get; set; }

        public int CategoryIdentifier { get; set; }

        public string SCategoryIdentifier { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The name field value must be between 5 and 255 characters")]
        [Required(ErrorMessage = "This field is required")]
        public string Name { get; set; }

        [StringLength(20, MinimumLength = 5, ErrorMessage = "The short name field value must be between 5 and 255 characters")]
        public string ShortName { get; set; }

        [StringLength(20, MinimumLength = 5, ErrorMessage = "The username field value must be between 5 and 20 characters")]
        [RegularExpression("^[A-Za-z\\d_-]+$", ErrorMessage = "Only numbers, alphabets and underscores are allowed e.g user_name. No white spaces")]
        public string UserName { get; set; }

        [StringLength(200, MinimumLength = 10, ErrorMessage = "The address field value must be between 10 and 200 characters")]
        [Required(ErrorMessage = "Address field is required")]
        public string Address { get; set; }

        [StringLength(14, MinimumLength = 6, ErrorMessage = "Add a valid phone number")]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [StringLength(100, MinimumLength = 5, ErrorMessage = "Add a valid TIN field")]
        public string TIN { get; set; }

        [StringLength(100, MinimumLength = 5, ErrorMessage = "Add a valid RC Number field")]
        public string RCNumber { get; set; }

        [EmailAddress]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The email field value must be between 2 and 100 characters")]
        public string Email { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The contact person name field value must be between 5 and 255 characters")]
        public string ContactPersonName { get; set; }

        [EmailAddress]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The contact person email field value must be between 2 and 100 characters")]
        public string ContactPersonEmail { get; set; }

        [StringLength(14, MinimumLength = 6, ErrorMessage = "Add a valid phone number")]
        public string ContactPersonPhoneNumber { get; set; }

        [StringLength(14, MinimumLength = 11, ErrorMessage = "Add a valid BVN")]
        public string BVN { get; set; }

        public int IdType { get; set; }

        public string IdNumber { get; set; }

        public int BusinessTypeId { get; set; }

        public int Gender { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The password field value must be between 5 and 255 characters")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string RequestReference { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }


    public class RegisterCBSUserModel : RegisterUserModel
    {
        

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The confirm password field value must be between 5 and 255 characters")]
        [Compare("Password", ErrorMessage ="Password and confirm password do not match")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}