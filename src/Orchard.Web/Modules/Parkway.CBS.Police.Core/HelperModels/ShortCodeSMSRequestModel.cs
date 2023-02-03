using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class ShortCodeSMSRequestModel
    {
        public string Content { get; set; }

        public string PhoneNumber { get; set; }

        public string FileNumber { get; set; }

        public string ServiceCode { get; set; }
    }
}