using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class AccountWalletPaymentReportVM
    {
        public long AccountPaymentRequestId { get; set; }

        public string PaymentId { get; set; }

        public string SourceAccount { get; set; }

        public string SourceAccountNumber { get; set; }

        public string BeneficiaryName { get; set; }

        public string AccountNumber { get; set; }

        public string Bank { get; set; }

        public string AccountName { get; set; }

        public string ExpenditureHead { get; set; }

        public decimal Amount { get; set; }

        public int Status { get; set; }

        public string StatusString {
            get
            {
                return ((Models.Enums.PaymentRequestStatus)Status).ToString();
            }
        }

        public DateTime DateInitiated { get; set; }
    }
}