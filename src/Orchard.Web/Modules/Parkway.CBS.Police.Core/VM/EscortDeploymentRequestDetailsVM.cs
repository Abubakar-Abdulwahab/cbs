using Parkway.CBS.Core.Utilities;
using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class EscortDeploymentRequestDetailsVM
    {
        public EscortRequestVM EscortInfo { get; set; }

        public string ViewName { get; set; }

        public string Comment { get; set; }

        public string PoliceOfficerName { get; set; }

        public string AccountNumber { get; set; }

        public string BankCode { get; set; }

        public string BankName { get { return Util.GetBankName(BankCode); } }

        public string ServiceNumber { get; set; }

        public string IPPISNumber { get; set; }

        public string Narration { get; set; }

        public decimal Amount { get; set; }

        public decimal AmountContributed { get; set; }

        public string RankName { get; set; }

        public string InvoiceNumber { get; set; }

        public bool ShowApprovalForm { get; set; }

        public Int64 RequestId { get; set; }

        public int ApprovalStatus { get; set; }

        public int ApproverId { get; set; }

        public Int64 DeploymentAllowanceRequestId { get; set; }
    }
}