using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class CreateCompanyResponse : BaseRequestResponse
    {
        public string Name { get; set; }
        public string CompanyKey { get; set; }
    }    
}