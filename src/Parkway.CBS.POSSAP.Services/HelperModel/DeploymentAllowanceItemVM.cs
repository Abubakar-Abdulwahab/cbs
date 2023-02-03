namespace Parkway.CBS.POSSAP.Services.HelperModel
{
    public class DeploymentAllowanceItemVM
    {
        public int Status { get; set; }

        public long PoliceOfficerLogId { get; set; }

        public decimal Amount { get; set; }

        public decimal ContributedAmount { get; set; }

        public string Narration { get; set; }

        public long RequestId { get; set; }

        public long InvoiceId { get; set; }

        public int CommandId { get; set; }

        public int PaymentStageId { get; set; }

        public long EscortDetailsId { get; set; }

    }
}
