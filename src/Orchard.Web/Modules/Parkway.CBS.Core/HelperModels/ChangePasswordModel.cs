using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Your old password field is required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "Your new password field value must be between 5 and 255 characters")]
        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The new confirm password field value must be between 5 and 255 characters")]
        [Compare("NewPassword", ErrorMessage = "New Password and new Confirm Password do not match")]
        [Required(ErrorMessage = "New Confirm Password is required")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }


        public HeaderObj HeaderObj { get; set; }

        public FlashObj FlashObj{get;set;}
    }
}