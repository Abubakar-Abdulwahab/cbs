using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class SettlementFeePartiesVM
    {
        public List<SettlementFeePartyVM> FeeParties { get; set; }

        public dynamic Pager { get; set; }

        public int TotalNumberOfFeePartyConfiguration { get; set; }
    }
}