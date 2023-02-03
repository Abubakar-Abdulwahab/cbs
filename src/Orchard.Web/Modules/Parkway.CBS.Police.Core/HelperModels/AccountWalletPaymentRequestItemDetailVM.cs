namespace Parkway.CBS.Police.Core.HelperModels
{
    public class AccountWalletPaymentRequestItemDetailVM
    {
        public string BeneficiaryName { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public string ExpenditureHeadName { get; set; }

        public decimal Amount { get; set; }

        public string Bank { get; set; }
    }
}