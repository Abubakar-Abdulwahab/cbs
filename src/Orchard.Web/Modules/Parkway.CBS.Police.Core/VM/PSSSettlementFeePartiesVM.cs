using Parkway.CBS.Police.Core.DTO;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementFeePartiesVM
    {
        public List<SettlementFeePartyVM> FeeParties { get; set; }

        public List<PSSFeePartyAdapterConfigurationDTO> FeePartyAdapters { get; set; }

        public List<PSSSettlementFeePartyVM> SelectedSettlementFeeParties { get; set; }

        public List<PSSSettlementFeePartyVM> AddedSettlementFeeParties { get; set; }

        public List<PSSSettlementFeePartyVM> RemovedSettlementFeeParties { get; set; }

        public dynamic Pager { get; set; }

        public int SettlementId { get; set; }

        public string SettlementName { get; set; }

        public int TotalNumberOfActiveSettlementFeeParties { get; set; }
    }
}