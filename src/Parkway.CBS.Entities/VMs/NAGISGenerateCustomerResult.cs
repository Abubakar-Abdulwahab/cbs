using Parkway.Cashflow.Ng.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Entities.VMs
{
    public class NAGISGenerateCustomerResult
    {
        public CashFlowCustomer CashFlowCustomer { get; set; }

        public Int64 TaxEntityId { get; set; }
    }
}
