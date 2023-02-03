namespace Parkway.CBS.Core.HelperModels
{
    public class VerifyAccountModel
    {
        public HeaderObj HeaderObj { get; set; }

        public string Token { get; set; }

        public bool Redirecting { get; set; }

        public string URL { get; set; }

        public FlashObj FlashObj { get; set; }
    }
}