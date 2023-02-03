using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class EndOfficerDeploymentVM
    {
        public PoliceOfficerDeploymentLogVM DeploymentInfo { get; set; }

        public int SelectedOfficer { get; set; }

        public int DeploymentLogId { get; set; }

        public string EndReason { get; set; }

        public bool CanNotEndDeployment { get; set; }
    }
}