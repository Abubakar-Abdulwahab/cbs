namespace Parkway.CBS.Police.Core.VM
{
    public class CanApproveEscortVM
    {
        public string Message { get; set; }

        public string Token { get; set; }

        public bool CanApprove { get; set; }
    }

    public class EscortApprovalMessage
    {
        public string Message { get; set; }

        public bool CanApproveRequest { get; set; }
    }

}