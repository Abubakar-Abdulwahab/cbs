using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class AdditionalDetailsHelperModel
    {
        public string DetailsConcat { get; set; }

        public IEnumerable<string> DetailsAndControlIdConcat { get; set; }
    }
}