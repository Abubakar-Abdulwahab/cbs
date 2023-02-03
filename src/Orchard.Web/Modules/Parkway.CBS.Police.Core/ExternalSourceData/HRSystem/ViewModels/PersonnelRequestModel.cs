using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem
{
    public class PersonnelRequestModel
    {
        public string Page { get; set; }

        public string PageSize { get; set; }

        public string ServiceNumber { get; set; }

        public string StateCode { get; set; }

        public string IPPSNumber { get; set; }

        public string LGACode { get; set; }

        public string RankCode { get; set; }
    }
}