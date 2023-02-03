using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class BillingDemandNotice
    {
        public bool IsChecked { get; set; }
        public EffectiveFromType EffectiveFromType { get; set; }
        public int EffectiveFrom { get; set; }

        public string DemandNoticeHeaderText { get; set; }
    }
}