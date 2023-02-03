using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class GenericPoliceRequest : RequestDumpVM
    {

        public IEnumerable<FormControlViewModel> Forms { get; set; }

        public bool ViewedTermsAndConditionsModal { get; set; }

        public PSServiceCaveatVM Caveat { get; set; }
    }
}