using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class MDARevenueHeadsVM
    {
        public int RevenueHeadID { get; set; }

        public int MDAId { get; set; }

        public List<int> RevenueHeadIds { get; set; }
    }
}