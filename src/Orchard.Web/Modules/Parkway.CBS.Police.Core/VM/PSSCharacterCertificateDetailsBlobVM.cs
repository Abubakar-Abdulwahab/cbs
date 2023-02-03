namespace Parkway.CBS.Police.Core.VM
{
    public class PSSCharacterCertificateDetailsBlobVM
    {
        public string PassportPhotographBlob { get; set; }

        public string InternationalPassportDataPageBlob { get; set; }

        public string SignatureBlob { get; set; }

        public string PassportPhotographOriginalFileName { get; set; }

        public string InternationalPassportDataPageOriginalFileName { get; set; }

        public string SignatureOriginalFileName { get; set; }

        public string PassportPhotographFilePath { get; set; }

        public string InternationalPassportDataPageFilePath { get; set; }

        public string SignatureFilePath { get; set; }

        public string PassportPhotographContentType { get; set; }

        public string InternationalPassportDataPageContentType { get; set; }

        public string SignatureContentType { get; set; }
    }
}