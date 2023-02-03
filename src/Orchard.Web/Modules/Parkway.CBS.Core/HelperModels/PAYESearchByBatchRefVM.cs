using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYESearchByBatchRefVM
    {
        public HeaderObj HeaderObj { get; set; }

        public string BatchRef { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }
    }
}