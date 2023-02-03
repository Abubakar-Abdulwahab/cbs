using System;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class ProcessPayeModel
    {
        public decimal Amount { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "Profile ID field is required")]
        public Int64 ProfileId { get; set; }

        [Required(ErrorMessage = "The request reference field is required.")]
        [StringLength(100, ErrorMessage = "Request reference value is too long. This field can only contain up to 100 characters.")]
        public string RequestReference { get; set; }
    }
}