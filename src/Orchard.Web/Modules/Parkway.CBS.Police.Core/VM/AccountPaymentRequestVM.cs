namespace Parkway.CBS.Police.Core.VM
{
    public class AccountPaymentRequestVM
    {
        public long Id { get; set; }

        public int FlowDefinitionLevelId { get; set; }

        /// <summary>
        /// <see cref="Enums.PaymentRequestStatus"/>
        /// </summary>
        public int PaymentRequestStatus { get; set; }

        public int FlowDefinitionId { get; set; }

        public int FlowDefinitionLevelPosition { get; set; }

        public decimal TotalAmount { get; set; }

        public string SourceAccount { get; set; }

    }

    public class AccountPaymentSettlementRequestVM
    {
        public long Id { get; set; }

        public string BankCode { get; set; }

        public string SourceAccountNumber { get; set; }

        public int FlowDefinitionId { get; set; }

        public int FlowDefinitionLevelPosition { get; set; }
    }
}