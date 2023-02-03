using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;


namespace Parkway.CBS.Core.HelperModels
{
    public class MDASettingsHelper
    {
        public bool UseTSA { get; set; }
        public MDA MDA { get; set; }
        public List<CashFlowBank> Banks { get; set; }
    }
}