namespace Parkway.CBS.Core.HelperModels
{
    public class AccountVerificationEmailNotificationModel
    {
        public string Subject { get; set; }

        public string Sender { get; set; }

        public string TemplateFileName { get; set; }
    }
}