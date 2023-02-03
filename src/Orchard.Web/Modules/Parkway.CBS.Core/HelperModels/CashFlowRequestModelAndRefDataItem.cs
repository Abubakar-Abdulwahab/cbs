using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CashFlowRequestModelAndRefDataItem
    {

        public RefDataAndCashflowDetails RefDataAndCashflowDetails { get; set; }

        public CashFlowCreateCustomerAndInvoice CashFlowRequestModel { get; set; }
    }
}