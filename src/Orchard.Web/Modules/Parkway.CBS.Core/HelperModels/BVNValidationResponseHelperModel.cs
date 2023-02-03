using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class BVNValidationResponseHelperModel
    {
        public string requestId { get; set; }
        public string responseCode { get; set; }
        public string responseDescription { get; set; }
        public BVNValidationResponse responseObject { get; set; }
    }
}