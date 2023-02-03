using Parkway.CBS.Core.HelperModels;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Police.Core.VM
{
    public class RetrieveEmailVM
    {
        public HeaderObj HeaderObj { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        public string PhoneNumber { get; set; }

        public FlashObj FlashObj { get; set; }
    }
}