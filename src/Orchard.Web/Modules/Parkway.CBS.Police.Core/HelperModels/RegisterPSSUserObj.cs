using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class RegisterPSSUserObj : RegisterCBSUserObj
    {
        public IEnumerable<dynamic> PSSIdentificationTypes { get; set; }

        public int FormErrorNumber { get; set; }

        public string AlternativeContactPersonName { get; set; }

        public string AlternativeContactPersonEmail { get; set; }

        public string AlternativeContactPersonPhoneNumber { get; set; }

        public bool HasAlternativeContactInfo { get; set; }

        public string CBSUserName { get; set; }

        public TaxEntityProfileLocationVM LocationInfo { get; set; }
    }
}