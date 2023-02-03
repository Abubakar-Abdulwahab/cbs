using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using System;

namespace Parkway.CBS.Police.Core.Models
{
    public class CharacterCertificateBiometrics : CBSBaseModel
    {
        public virtual long Id { get; set; }

        public virtual UserPartRecord UserPartRecord { get; set; }

        public virtual PSSRequest Request { get; set; }

        public virtual PSSCharacterCertificateDetails CharacterCertificateDetails { get; set; }

        public virtual string PassportImage { set; get; }

        public virtual string RightThumb { set; get; }

        public virtual string RightIndex { set; get; }

        public virtual string RightMiddle { set; get; }

        public virtual string RightRing { set; get; }

        public virtual string RightPinky { set; get; }

        public virtual string LeftThumb { set; get; }

        public virtual string LeftIndex { set; get; }

        public virtual string LeftMiddle { set; get; }

        public virtual string LeftRing { set; get; }

        public virtual string LeftPinky { set; get; }

        public DateTime RegisteredAt { get; set; }
    }
}