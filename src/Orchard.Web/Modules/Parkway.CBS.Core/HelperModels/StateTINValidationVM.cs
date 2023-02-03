using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class StateTINValidationVM
    {
        [Required(ErrorMessage = "State TIN is required")]
        public string StateTIN { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public TaxPayerWithDetails StateTINViewModel { get; set; }

    }
}