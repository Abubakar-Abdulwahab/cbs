using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class AssignRevenueHeadPermissionConstraintsVM
    {
        public ExpertSystemVM ExpertSystem { get; set; }

        public List<RevenueHeadPermissionVM> Permissions { get; set; }

        public string SelectedPermissionId { get; set; }

        public int SelectedPermissionIdParsed { get; set; }

        public List<MDAVM> MDAs { get; set; }

        public List<int> SelectedMdas { get; set; }

        public List<int> SelectedRevenueHeads { get; set; }

        public string SelectedRhAndMdas { get; set; }
    }
}