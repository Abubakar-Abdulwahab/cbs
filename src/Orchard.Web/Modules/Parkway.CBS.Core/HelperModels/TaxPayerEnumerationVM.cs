using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxPayerEnumerationVM
    {
        public HeaderObj HeaderObj { get; set; }

        public TaxEntityViewModel TaxEntity { get; set; }

        public List<Models.StateModel> StateLGAs { get; set; }

        public int PageSize { get; set; }

        public string BatchToken { get; set; }

        public string ErrorMessage { get; set; }
    }
}