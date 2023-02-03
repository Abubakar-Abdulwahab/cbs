using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ExternalRedirect
    {
        public string URL { get; set; }

        public string Token { get; set; }

        public InvoiceGenerationStage Stage { get; set; }

        public string Message { get; set; }

        public bool Redirecting { get; set; }
    }
}