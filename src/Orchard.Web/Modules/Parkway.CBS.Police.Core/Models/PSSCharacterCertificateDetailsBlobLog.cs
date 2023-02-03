namespace Parkway.CBS.Police.Core.Models
{
    public class PSSCharacterCertificateDetailsBlobLog : PSSCharacterCertificateDetailsBlobBase
    {
        public virtual PSSCharacterCertificateDetailsBlob PSSCharacterCertificateDetailsBlob { get; set; }

        public virtual PSSCharacterCertificateDetailsLog PSSCharacterCertificateDetailsLog { get; set; }
    }
}