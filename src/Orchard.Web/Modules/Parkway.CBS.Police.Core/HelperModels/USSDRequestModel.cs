using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class USSDRequestModel
    {
        public string Text { get; set; }

        public string PhoneNumber { get; set; }

        public string SessionId { get; set; }

        public string ServiceCode { get; set; }
    }
}