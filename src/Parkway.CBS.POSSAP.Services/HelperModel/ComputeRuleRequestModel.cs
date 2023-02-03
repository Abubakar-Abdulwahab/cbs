using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class ComputeRuleRequestModel
    {
        public string RuleCode { get; set; }

        public decimal Amount { get; set; }

        public string Narration { get; set; }

        public string SettlementDate { get; set; }

        public int NumberOfTransactions { get; set; }

        public string ReferenceNumber { get; set; }
    }
}
