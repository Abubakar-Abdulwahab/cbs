using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSServiceSettlementConfigurationVM
    {
        public int SettlemntConfigurationId { get; set; }

        public int SettlemntRuleId { get; set; }

        public int DefinitionLevelId { get; set; }

        public int PaymentProviderId { get; set; }

        public string PaymentProviderName { get; set; }

        public int ServiceId { get; set; }

        public int RevenueHeadId { get; set; }

        public int MDAId { get; set; }

        public int Channel { get; set; }
    }
}
