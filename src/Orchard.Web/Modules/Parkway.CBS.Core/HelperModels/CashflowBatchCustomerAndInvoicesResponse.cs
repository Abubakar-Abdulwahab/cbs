using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CashflowBatchCustomerAndInvoicesResponse
    {
        public string BatchIdentifier { get; set; }

        public string Status { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime CompletionDate { get; set; }

        public string Mac { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }

        public string FileName { get; set; }
    }
}