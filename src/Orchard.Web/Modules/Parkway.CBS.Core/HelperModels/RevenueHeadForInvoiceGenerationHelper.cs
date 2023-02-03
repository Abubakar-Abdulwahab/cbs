using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class RevenueHeadForInvoiceGenerationHelper
    {
        public RevenueHeadVM RevenueHeadVM { get; set; }


        public BillingModelVM BillingModelVM { get; set; }


        public MDAVM MDAVM { get; set; }


        public List<RevenueHeadGroupVM> RevenueHeadGroupVM { get; set; }

        /// <summary>
        /// holds the expected form control details of this revenue head
        /// </summary>
        public IEnumerable<FormControlViewModel> Forms { get; set; }


        public int Index { get; internal set; }
    }
}