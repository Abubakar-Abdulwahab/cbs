using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class DeploymentAllowancePaymentSearchParams
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string SourceAccountName { get; set; }

        public int Status { get; set; }

        public string PaymentRef { get; set; }

        public bool DontPageData { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public int UserPartRecordId { get; set; }

        public bool ApplyRestriction { get; set; }

        public int CommandTypeId { get; set; }
    }
}