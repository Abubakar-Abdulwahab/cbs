using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public class ValidatedDocumentVM
    {
        public HeaderObj HeaderObj { get; set; }

        public int ServiceType { get; set; }

        public long RequestId { get; set; }

        public string PartialViewName { get; set; }

        public dynamic DocumentInfo { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }
    }
}