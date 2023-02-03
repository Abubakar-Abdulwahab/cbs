using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class NetPayModel
    {
        public string Identifier { get; set; }

        public string CallBackURL { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string Token { get; set; }
    }
}