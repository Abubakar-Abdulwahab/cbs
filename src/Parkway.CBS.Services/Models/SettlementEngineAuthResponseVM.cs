using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Services.Models
{
    public class SettlementEngineAuthResponseVM
    {
        public string token { get; set; }

        public string expiration { get; set; }
    }
}