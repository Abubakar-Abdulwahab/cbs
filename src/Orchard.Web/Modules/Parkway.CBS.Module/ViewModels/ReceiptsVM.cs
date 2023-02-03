using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class ReceiptsVM
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public string TIN { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public ReceiptStatus ReceiptStatus { get; set; }

        public string ReceiptNumber { get; set; }

        public string DateFilter { get; set; }

        public List<ReceiptItems> ReceiptsItems { get; set; }

        public Int64 DataSize { get; set; }

        public string Token { get; set; }
    }
}