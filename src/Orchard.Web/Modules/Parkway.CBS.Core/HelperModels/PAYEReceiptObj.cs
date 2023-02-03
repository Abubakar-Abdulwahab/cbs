using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEReceiptObj
    {
        public HeaderObj HeaderObj { get; set; }

        public string ReceiptNumber { get; set; }

        public string DateFilter { get; set; }

        public IEnumerable<PAYEReceiptVM> ReceiptItems { get; set; }

        public Int64 DataSize { get; set; }

        public string Token { get; set; }
    }
}