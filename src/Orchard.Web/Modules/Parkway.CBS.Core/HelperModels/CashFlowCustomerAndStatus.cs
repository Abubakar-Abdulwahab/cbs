using Parkway.Cashflow.Ng.Models;

namespace Parkway.CBS.Core.HelperModels
{
    public class CashFlowCustomerAndStatus
    {
        public CashFlowCustomer CashFlowCustomer { get; set; }

        public string TaxIdentificationNumber { get; set; }

        public string BatchNumber { get; set; }

        public bool status { get; set; }
    }
}