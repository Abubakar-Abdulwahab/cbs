using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PCCDiasporaUserInputVM : CharacterCertificateRequestVM
    {
        public int SelectedCountryOfResidence { get; set; }

        public string NIN { get; set; }

        /// <summary>
        /// selected identity type
        /// </summary>
        public int SelectedIdentity { get; set; }

        /// <summary>
        /// the identity value for the selected identity
        /// </summary>
        public string SelectedIdentityNumber { get; set; }

        public List<IdentificationTypeVM> IdentityTypeList { get; set; }

        public int SelectedIdentityType { get; set; }

        public string IdentityValue { get; set; }
    }
}