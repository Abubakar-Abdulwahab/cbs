using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class FormHelperModel
    {
        public bool IsCompulsory { get; set; }
        public FormControl FormControls { get; set; }
        public TaxEntityCategory TaxEntityCategory { get; set; }

    }
}