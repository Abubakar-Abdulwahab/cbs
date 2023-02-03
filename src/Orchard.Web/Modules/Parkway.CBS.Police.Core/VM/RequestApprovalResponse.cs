namespace Parkway.CBS.Police.Core.VM
{
    public class RequestApprovalResponse
    {
        public string ServiceType { get; set; }

        public string CustomerName { get; set; }

        public string FileNumber { get; set; }

        public string ApprovalNumber { get; set; }

        public string NotificationMessage { get; set; }

        public bool ResponseFromPartial { get; set; }
    }
}