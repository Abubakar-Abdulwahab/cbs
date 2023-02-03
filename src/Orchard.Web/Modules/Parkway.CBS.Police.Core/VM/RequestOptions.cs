using Parkway.CBS.Police.Core.Models.Enums;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestOptions
    {
        public PSSRequestStatus RequestStatus { get; set; }

        public string InvoiceNumber { get; set; }

        public string FileNumber { get; set; }

        public string ApprovalNumber { get; set; }

        public int CommandId { get; set; }

        public int RevenueHeadId { get; set; }

        public string PaymentRef { get; set; }

        public string ReceiptNumber { get; set; }

        public string CustomerName { get; set; }

    }
}