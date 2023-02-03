﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class PSSPresettlementDeductionConfigurationVM
    {
        public int SettlemntRuleId { get; set; }

        public int DefinitionLevelId { get; set; }

        public int PaymentProviderId { get; set; }

        public string PaymentProviderName { get; set; }

        public int ServiceId { get; set; }

        public int RevenueHeadId { get; set; }

        public int MDAId { get; set; }

        public int Channel { get; set; }

        public string Name { get; set; }

        public string ImplementClass { get; set; }

        public int DeductionShareTypeId { get; set; }

        public decimal PercentageShare { get; set; }

        public decimal FlatShare { get; set; }
    }
}
