namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels
{
    public class RootPersonnelResponse
    {
        public bool Error { get; set; }

        public string ErrorCode { get; set; }

        public dynamic ResponseObject { get; set; }
    }

}