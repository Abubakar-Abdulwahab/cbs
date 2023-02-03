using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class SignInObj
    {
        public HeaderObj HeaderObj { get; set; }

        public String ErrorMessage { get; set; }

        public bool Error { get; set; }

        //public string Email { get; set; }

        public string CBSUserName { get; set; }

        public string Message { get; set; }

        public string Password { get; set; }

        public string ReturnURL { get; set; }
    }
}