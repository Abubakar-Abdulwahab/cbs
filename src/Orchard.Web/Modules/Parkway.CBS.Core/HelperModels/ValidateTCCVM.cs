using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ValidateTCCVM
    {
        public string ApplicationNumber { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public string Message { get; set; }

        public HeaderObj HeaderObj { get; set; }
    }
}