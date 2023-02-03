using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class ValidateFileModel
    {
        public List<ReferenceDataLGAs> LGAList { get; set; }

        public string LGAId { get; set; }

        public IEnumerable<AdaptersVM> Adapters { get; set; }

        public string ClassName { get; set; }

        public string Value { get; set; }

        public int StateId { get; set; }
    }
}