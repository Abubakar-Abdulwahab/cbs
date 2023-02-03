using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSSettlementFeePartyBatchAggregateSettlementRequestModel
    {
        public string RuleCode { get; set; }

        public string ReferenceNumber { get; set; }

        public string Narration { get; set; }

        public string SettlementDate { get; set; }

        public string PaymentType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public IEnumerable<PSSSettlementFeePartyBatchAggregateSettlementItemRequestModel> Items { get; set; }
    }
}
