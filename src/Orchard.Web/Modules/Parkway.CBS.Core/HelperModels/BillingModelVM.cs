using Parkway.CBS.Core.Models.Enums;
using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class BillingModelVM
    {
        public int Id { get; set; }

        public DateTime? NextBillingDate { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// Assessment Due Date model
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// Updated by the job
        /// </summary>
        public bool StillRunning { get; set; }

        /// <summary>
        /// Fixed or Variable
        /// <see cref="Enums.BillingType"/>
        /// </summary>
        public BillingType BillingType { get; set; }

        public string PenaltyJSONModel { get; set; }

        public string DiscountJSONModel { get; set; }
    }
}