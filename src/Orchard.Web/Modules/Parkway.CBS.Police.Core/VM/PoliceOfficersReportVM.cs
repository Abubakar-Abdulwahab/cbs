using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PoliceOfficersReportVM
    {
        public PoliceOfficerReportSearchFilter SearchFilter { get; set; }

        public ICollection<PoliceOfficerDetailsVM> Officers { get; set; }
    }
}