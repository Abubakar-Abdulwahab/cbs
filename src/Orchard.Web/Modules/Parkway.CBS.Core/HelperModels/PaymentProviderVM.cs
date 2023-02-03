using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PaymentProviderVM
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public int Identifier { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}