using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public class ValidateDocumentVM
    {
        public HeaderObj HeaderObj { get; set; }

        public string ApprovalNumber { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }
    }
}