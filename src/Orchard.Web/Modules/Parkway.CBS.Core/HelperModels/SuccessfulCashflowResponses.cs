using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class SuccessfulAndUnSuccessfulCashflowResponses
    {
        public ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> SuccessfulCashFlowResponses { get; set; }

        public ConcurrentStack<CashflowCreateCustomerAndGenerateInvoiceResponseModel> UnSuccessfulCashFlowResponses { get; set; }
    }
}