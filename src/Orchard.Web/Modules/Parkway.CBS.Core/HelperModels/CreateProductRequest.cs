using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateProductRequest : BaseRequestResponse
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public Decimal Price { get; set; }
        public string ProductCode { get; set; }
    }
}