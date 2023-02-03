using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CashflowCreateCustomerAndGenerateInvoiceResponseModel
    {
        public CashFlowCreateCustomerAndInvoice RequestModel { get; set; }

        public RefDataAndCashflowDetails RefDataItem { get; set; }

    }
}