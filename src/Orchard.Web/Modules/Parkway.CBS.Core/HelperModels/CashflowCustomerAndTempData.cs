using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.ReferenceData;

namespace Parkway.CBS.Core.HelperModels
{
    public class CashflowCustomerAndTempData
    {
        public CashFlowCustomer CashflowCustomer { get; set; }

        public RefDataTemp RefDataTemp { get; set; }

        public bool Successful { get; set; }

        public string ReasonForFailure { get; set; }
    }
}