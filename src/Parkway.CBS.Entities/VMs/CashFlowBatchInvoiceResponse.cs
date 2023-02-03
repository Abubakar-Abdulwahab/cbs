using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.VMs
{
    public class CashFlowBatchInvoiceResponse
    {
        public List<CashFlowCreateCustomerAndInvoiceResponse> ResultModel { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string BatchIdentifier { get; set; }

        public string ErrorMessage { get; set; }

        public bool ErrorOccurred { get; set; }

        public bool DoneProcessing { get; set; }
    }
}
