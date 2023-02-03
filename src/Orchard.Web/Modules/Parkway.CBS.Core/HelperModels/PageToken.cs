using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class PageToken
    {
        public DateTime DateTimeCreated { get; set; }

        public string sTaxPayerId { get; set; }

        public string sRevenueHeadId { get; set; }

        public string SessionKey { get; set; }
    }
}