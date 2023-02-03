using System;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class CharacterCertificateBiometricRequestVM
    {
        public string FileNumber { get; set; }

        public string Comment { get; set; }

        public string RightThumb { set; get; }

        public string RightIndex { set; get; }

        public string RightMiddle { set; get; }

        public string RightRing { set; get; }

        public string RightPinky { set; get; }

        public string LeftThumb { set; get; }

        public string LeftIndex { set; get; }

        public string LeftMiddle { set; get; }

        public string LeftRing { set; get; }

        public string LeftPinky { set; get; }

        public string Token { set; get; }

        public string PassportImage { set; get; }

        public DateTime RegisteredDate { get; set; }

    }
}