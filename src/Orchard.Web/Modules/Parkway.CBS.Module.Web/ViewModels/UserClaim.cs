using System;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class UserLoginSuccessfulResponse
    {
        public string UserName { get; set; }
        public string TIN { get; set; }
        public string RCNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Recipient { get; set; }
        public string LoggedInUser { get; set; }
    }


    public class UserClaim
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public DateTime TTL { get; set; }
    }
}