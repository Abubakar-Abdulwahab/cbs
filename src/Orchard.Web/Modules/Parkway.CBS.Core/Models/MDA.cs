using System.Collections.Generic;

namespace Parkway.CBS.Core.Models
{
    public class MDA : MDARevenueHead
    {
        public virtual ICollection<RevenueHead> RevenueHeads { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }

        public virtual IEnumerable<BillingSchedule> BillingSchedules { get; set; }

        public virtual MDASettings MDASettings { get; set; }

        public virtual BankDetails BankDetails { get; set; }

        public virtual string SMEKey { get; set; }

        /// <summary>
        /// if the MDA uses TSA
        /// </summary>
        public virtual bool UsesTSA { get; set; }

        public virtual ExpertSystemSettings ExpertSystemSettings { get; set; }

        public virtual bool HasPaymentProviderValidationConstraint { get; set; }
    }
}