using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class UserLoginModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }
    }
}