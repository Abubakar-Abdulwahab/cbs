using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSPay.Core.ViewModels
{
    public class EIRSAPILoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string grant_type { get; set; }
    }

    public class EIRSAPILoginResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }
}
