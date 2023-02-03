using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Core.HelperModels
{
    public class DueDateModel
    {
        public int DueDateInterval { get; set; }
        public DueDateAfter DueDateAfter { get; set; }

        /// <summary>
        /// only for recurring billing
        /// </summary>
        public bool DueOnNextBillingDate { get; set; }
    }
}