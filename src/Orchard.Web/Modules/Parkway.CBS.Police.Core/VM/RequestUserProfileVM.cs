using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public class RequestUserProfileVM
    {
        public HeaderObj HeaderObj { get; set; }

        public bool HasMessage { get; set; }

        public FlashObj FlashObj { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }
    }
}