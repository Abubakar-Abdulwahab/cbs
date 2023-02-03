using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class ReceiptsVM
    {
        public HeaderObj HeaderObj { get; set; }

        public string ReceiptNumber { get; set; }

        public string DateFilter { get; set; }

        public IEnumerable<PAYEReceiptItems> ReceiptItems { get; set; }

        public Int64 DataSize { get; set; }

        public string Token { get; set; }
    }
}