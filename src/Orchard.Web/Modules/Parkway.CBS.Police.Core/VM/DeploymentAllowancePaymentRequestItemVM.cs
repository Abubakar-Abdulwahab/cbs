namespace Parkway.CBS.Police.Core.VM
{
    public class DeploymentAllowancePaymentRequestItemVM
    {
        public string BankCode { get; set; }

        public string BankName { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public decimal Amount { get; set; }

        public string AmountString { get; set; }

        public int CommandTypeId { get; set; }

        public string CommandTypeName { get; set; }

        public int DayTypeId { get; set; }

        public string DayTypeName { get; set; }

        public string Duration { get; set; }
    }
}