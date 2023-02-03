using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class DeploymentAllowanceSettlementItemsVM
    {
        public string AccountName { get; set; }

        public decimal Amount { get; set; }

        public string Narration { get; set; }

        public string AccountNumber { get; set; }

        public string BankCode { get; set; }
    }
}