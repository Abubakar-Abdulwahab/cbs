using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportVM
    {
        public List<PSSSettlementVM> Settlements { get; set; }

        public dynamic Pager { get; set; }

        public int TotalActiveSettlements { get; set; }
    }
}