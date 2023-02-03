using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class SearchByReceiptNumberVM
    {
        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string ReceiptNumber { get; set; }

        public ReceiptViewModel ReceiptViewModel { get; set; }

    }
}