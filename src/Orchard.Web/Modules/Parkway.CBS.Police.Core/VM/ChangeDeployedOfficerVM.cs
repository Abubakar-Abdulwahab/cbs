using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class ChangeDeployedOfficerVM
    {
        public PoliceOfficerDeploymentLogVM DeploymentInfo { get; set; }

        public int selectedOfficer { get; set; }

        public int deploymentLogId { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<PoliceRankingVM> PoliceRanks { get; set; }

        public bool CanNotBeChanged { get; set; }
    }
}