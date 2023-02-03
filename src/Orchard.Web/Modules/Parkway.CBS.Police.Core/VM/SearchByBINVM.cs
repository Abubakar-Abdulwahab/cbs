using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class SearchByBINVM
    {
        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string BIN { get; set; }
    }
}