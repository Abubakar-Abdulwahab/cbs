using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReceiptObj
    {
        public List<ReceiptItems> ReceiptItems { get; set; }

        public int PageSize { get; set; }
    }
}