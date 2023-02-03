using Parkway.CBS.Core.HelperModels;
using System;

namespace Parkway.CBS.Police.Core.DTO
{
    public class DeploymentAllowancePaymentRequestItemDTO
    {
        public long Id { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public decimal Amount { get; set; }

        public BankVM Bank { get; set; }

        public string PaymentReference { get; set; }

        public int CommandTypeId { get; set; }

        public string CommandTypeName { get; set; }

        public int DayTypeId { get; set; }

        public string DayTypeName { get; set; }

        public long DeploymentAllowancePaymentRequestId { get; set; }

        public DateTime StartDate { get; set; }

        public string StartDateString => StartDate.ToString("dd/MM/yyyy");

        public DateTime EndDate { get; set; }

        public string EndDateString => EndDate.ToString("dd/MM/yyyy");
    }
}