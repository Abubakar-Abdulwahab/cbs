using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class ForgotPasswordVM
    {
        public HeaderObj HeaderObj { get; set; }

        [EmailAddress]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "The email field value must be between 2 and 100 characters")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public FlashObj FlashObj { get; set; }

    }
}