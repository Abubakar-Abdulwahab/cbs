using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEBatchItemSearchParams
    {
        public string BatchRef { get; set; }

        public long TaxEntityId { get; set; }

        public int Page { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

        public bool PageData { get; set; }
    }
}