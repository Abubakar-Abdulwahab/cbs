using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.ReferenceData.Admin.ViewModels
{
    public class ReferenceDataBatchSearchParams
    {
        public string BatchRef { get; set; }

        public DateTime? FromRange { get; set; }

        public DateTime? EndRange { get; set; }
    }
}