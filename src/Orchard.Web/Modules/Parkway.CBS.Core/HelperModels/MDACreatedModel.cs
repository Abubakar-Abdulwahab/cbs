using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class MDACreatedModel
    {
        public string CashFlowCompanyKey { get; set; }
        public int CBSId { get; set; }
        public string MDASlug { get; set; } 
    }


    public class MDAEditedModel
    {
        public string CashFlowCompanyKey { get; set; }
        public int CBSId { get; set; }
        public string MDASlug { get; set; }
    }
}