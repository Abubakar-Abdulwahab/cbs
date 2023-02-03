using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateStateTINModel
    {
        [StringLength(255, MinimumLength = 5, ErrorMessage = "The name field value must be between 5 and 255 characters")]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [StringLength(14, MinimumLength = 6, ErrorMessage = "Add a valid phone number")]
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        [StringLength(200, MinimumLength = 10, ErrorMessage = "The address field value must be between 10 and 200 characters")]
        [Required(ErrorMessage = "Address field is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Payer Category is required")]
        public int PayerCategory { get; set; }

        [Required(ErrorMessage = "State Code is required")]
        public string StateCode { get; set; }

        [Required(ErrorMessage = "LGA Code is required")]
        public string LGACode { get; set; }
    }
}