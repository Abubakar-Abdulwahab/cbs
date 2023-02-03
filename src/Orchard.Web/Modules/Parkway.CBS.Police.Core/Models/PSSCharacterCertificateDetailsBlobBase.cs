using Parkway.CBS.Core.Models;


namespace Parkway.CBS.Police.Core.Models
{
    public abstract class PSSCharacterCertificateDetailsBlobBase : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual PSSCharacterCertificateDetails PSSCharacterCertificateDetails { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual string PassportPhotographBlob { get; set; }

        public virtual string InternationalPassportDataPageBlob { get; set; }

        public virtual string SignatureBlob { get; set; }

        public virtual string PassportPhotographOriginalFileName { get; set; }

        public virtual string InternationalPassportDataPageOriginalFileName { get; set; }

        public virtual string SignatureOriginalFileName { get; set; }

        public virtual string PassportPhotographFilePath { get; set; }

        public virtual string InternationalPassportDataPageFilePath { get; set; }

        public virtual string SignatureFilePath { get; set; }

        public virtual string PassportPhotographContentType { get; set; }

        public virtual string InternationalPassportDataPageContentType { get; set; }

        public virtual string SignatureContentType { get; set; }
    }
}