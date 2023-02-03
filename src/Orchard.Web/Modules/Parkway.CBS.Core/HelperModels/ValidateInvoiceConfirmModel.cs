using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ValidateInvoiceConfirmModel
    {
        public dynamic ViewModel { get; set; }

        public string ViewToShow { get; set; }

        public string ErrorMessage { get; set; }
    }
}