using System;

namespace Parkway.CBS.Core.HelperModels
{
    public class LogInObj
    {
        public HeaderObj HeaderObj { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string ReturnURL { get; set; }

        public FlashObj FlashObj { get; set; }
    }
}