using Orchard.Users.Models;
using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class PSSAdminSignatureUpload : CBSModel
    {
        public UserPartRecord AddedBy { get; set; }

        public string SignatureBlob { get; set; }

        public string SignatureFileName { get; set; }

        public string SignatureFilePath { get; set; }

        public string SignatureContentType { get; set; }

        public bool IsActive { get; set; }
    }
}