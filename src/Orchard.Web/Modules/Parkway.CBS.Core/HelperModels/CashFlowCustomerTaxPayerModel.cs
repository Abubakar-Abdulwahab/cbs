using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CashFlowCustomerTaxPayerModel
    {
        public Int64 TaxEntityId { get; set; }
        public string IdentificationNumber { get; set; }
        public Int64 PrimaryContactId { get; set; }
        public Int64 CashFlowId { get; set; }
    }
}