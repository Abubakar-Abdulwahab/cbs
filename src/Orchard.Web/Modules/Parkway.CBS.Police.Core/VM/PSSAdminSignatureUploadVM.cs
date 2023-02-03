using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSAdminSignatureUploadVM
    {
        public int Id { get; set; }

        public string SignatureBlob { get; set; }

        public string SignatureContentType { get; set; }

        public string SignatureFilePath { get; set; }

        public string SignatureFileName { get; set; }

        public int UserId { get; set; }

        public DateTime? createdAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; }
    }
}