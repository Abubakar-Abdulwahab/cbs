using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementFeePartyVM
    {
        public string SettlementFeePartyName { get; set; }

        public int FeePartyId { get; set; }

        public bool HasAdditionalSplits { get; set; }

        public int AdapterId { get; set; }

        public string AdapterName { get; set; }

        public decimal DeductionValue { get; set; }

        public bool MaxPercentage { get; set; }
    }
}