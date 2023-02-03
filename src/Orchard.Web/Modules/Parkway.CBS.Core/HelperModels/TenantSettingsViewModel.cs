using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.Cashflow.Ng.Models;


namespace Parkway.CBS.Core.HelperModels
{
    public class ExpertSettingsViewModel
    {
        public ExpertSystemSettings ExpertSystemsSettings { get; set; }

        public List<CashFlowState> States { get; set; }

        public List<CashFlowBank> Banks { get; set; }

        public List<string> ListOfRefData { get; set; }

        public bool IsEdit { get; set; }
    }
}