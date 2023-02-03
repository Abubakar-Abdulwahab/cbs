using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PAYEAPIRequestBatchDetailVM
    {
        public Int64 TaxEntityId { get; set; }

        public string PayerId { get; set; }

        public string BatchIdentifier { get; set; }

        public int BatchLimit { get; set; }

        public int PAYEAPIRequestId { get; set; }

        public long PAYEBatchRecordStagingId { get; set; }

        public string AdapterValue { get; set; }
    }
}