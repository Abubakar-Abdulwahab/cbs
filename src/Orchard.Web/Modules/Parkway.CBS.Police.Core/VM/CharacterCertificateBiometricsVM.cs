using Parkway.CBS.Police.Core.Utilities;
using System;

namespace Parkway.CBS.Police.Core.VM
{
    public class CharacterCertificateBiometricsVM
    {
        public string ApplicantName { get; set; }

        public string FileNumber { get; set; }

        public string Comment { get; set; }

        public string PassportImage { get; set; }


        private string rightThumb;

        public string RightThumb
        {
            get { return rightThumb; }
            set { rightThumb = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string rightIndex;
        public string RightIndex
        {
            get { return rightIndex; }
            set { rightIndex = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string rightMiddle;
        public string RightMiddle
        {
            get { return rightMiddle; }
            set { rightMiddle = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string rightRing;
        public string RightRing
        {
            get { return rightRing; }
            set { rightRing = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string rightPinky;
        public string RightPinky
        {
            get { return rightPinky; }
            set { rightPinky = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string leftThumb;
        public string LeftThumb
        {
            get { return leftThumb; }
            set { leftThumb = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string leftIndex;
        public string LeftIndex
        {
            get { return leftIndex; }
            set { leftIndex = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string leftMiddle;
        public string LeftMiddle
        {
            get { return leftMiddle; }
            set { leftMiddle = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string leftRing;
        public string LeftRing
        {
            get { return leftRing; }
            set { leftRing = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        private string leftPinky;
        public string LeftPinky
        {
            get { return leftPinky; }
            set { leftPinky = HelperImageConverter.ConvertToBase64BiometricsImage(value); }
        }

        public long Id { get; set; }

        public DateTime RegisteredAt { get; set; }

    }

}