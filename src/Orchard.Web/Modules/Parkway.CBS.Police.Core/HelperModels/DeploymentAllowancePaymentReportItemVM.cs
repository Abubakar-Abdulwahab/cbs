using System;
using Parkway.CBS.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class DeploymentAllowancePaymentReportItemVM
    {
        public long AccountPaymentRequestId { get; set; }

        public string PaymentReference { get; set; }

        public string SourceAccount { get; set; }

        public string SourceAccountNumber { get; set; }

        public string AccountNumber { get; set; }

        public string ServiceNumber { get; set; }

        public string AccountName { get; set; }

        public decimal Amount { get; set; }

        public int Status { get; set; }

        public string StatusString => ((Models.Enums.PaymentRequestStatus)Status).GetDescription();

        public DateTime DateInitiated { get; set; }

        public DateTime StartDate { get; set; }

        public string StartDateString => StartDate.ToString("dd/MM/yyyy");

        public DateTime EndDate { get; set; }

        public string EndDateString => EndDate.ToString("dd/MM/yyyy");

        public string CommandTypeName { get; set; }

        public string DayTypeName { get; set; }

        public string CustomerName { get; set; }
    }
}