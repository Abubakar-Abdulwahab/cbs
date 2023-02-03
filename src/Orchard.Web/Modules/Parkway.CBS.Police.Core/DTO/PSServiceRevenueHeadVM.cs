using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.DTO
{
    public class PSServiceRevenueHeadVM
    {
        public string ServiceName { get; set; }

        public decimal AmountToPay { get; set; }

        public string FeeDescription { get; set; }

        public int RevenueHeadId { get; set; }

        public int MDAId { get; set; }

        public bool IsGroupHead { get; set; }

        public List<FormControlViewModel> Forms { get; set; }

        public decimal Surcharge { get; set; }

        public int ServiceId { get; set; }
    }
}