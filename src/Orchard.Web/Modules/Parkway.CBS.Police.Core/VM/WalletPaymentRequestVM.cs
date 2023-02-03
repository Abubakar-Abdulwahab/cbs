namespace Parkway.CBS.Police.Core.VM
{
    public class WalletPaymentRequestVM
    {
        public int SelectedBankId { get; set; }

        public string Bank { get; set; }

        public int SelectedWalletId { get; set; }

        public string SelectedWallet { get; set; }

        public int SelectedExpenditureHeadId { get; set; }

        public string SelectedExpenditureHead { get; set; }

        public string BeneficiaryName { get; set; }

        public string AccountNumber { get; set; }

        public string AccountName { get; set; }

        public decimal Amount { get; set; }

    }
}