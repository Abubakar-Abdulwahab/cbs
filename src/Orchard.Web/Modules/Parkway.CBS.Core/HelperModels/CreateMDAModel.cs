using Parkway.CBS.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateMDAModel
    {
        public MDA MDA { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The Request identifier value between 2 to 100 characters", MinimumLength = 2)]
        /// <summary>
        /// this is the unique request identifier
        /// </summary>
        public string RequestIdentifier { get; set; }

        [Required(ErrorMessage = "Authorizing user email is required")]
        [EmailAddress(ErrorMessage = "Add a valid Email address")]
        public string UserEmail { get; set; }
    }
}