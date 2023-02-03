using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class ResetPasswordVM
    {
        public HeaderObj HeaderObj { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The new password field value must be between 5 and 255 characters")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [StringLength(255, MinimumLength = 5, ErrorMessage = "The confirm password field value must be between 5 and 255 characters")]
        [Compare("NewPassword", ErrorMessage = "Your new password and confirm password do not match")]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }

        public FlashObj FlashObj { get; set; }
    }
}