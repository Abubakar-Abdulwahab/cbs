namespace Parkway.CBS.Police.Core.DTO
{
    public class AccountWalletPaymentRequestItemDTO
    {
        public string BeneficiaryName { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string ExpenditureHeadName { get; set; }

        public decimal Amount { get; set; }

        public string BankCode { get; set; }

        public string PaymentReference { get; set; }

        public long Id { get;  set; }
    }
}