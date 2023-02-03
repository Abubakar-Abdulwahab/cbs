namespace Parkway.CBS.Police.Core.HelperModels
{
    public class DeploymentAllowancePaymentNotificationModel
    {
        public bool Error { get; set; }

        public string ErrorCode { get; set; }

        public DeploymentAllowancePaymentDetailsModel ResponseObject { get; set; }
    }
}