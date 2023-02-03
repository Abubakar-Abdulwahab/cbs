using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class EditRevenueHeadModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please enter valid integer Number. Minimum value is 1")]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name field is required")]
        [StringLength(250, ErrorMessage = "The Name value can be 250 characters long or 2 characters short", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Code field is required")]
        [StringLength(250, ErrorMessage = "The Code value can be 250 characters long or 2 characters short", MinimumLength = 2)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Authorized user email is required")]
        public string UserEmail { get; set; }
        
    }
}