using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class PasswordGeneratorOptions
    {
        public PasswordGeneratorOptions()
        {
            this.RequiredLength = 6;
            this.RequiredUniqueChars = 1;
            this.RequireNonAlphanumeric = true;
            this.RequireLowercase = true;
            this.RequireUppercase = true;
            this.RequireDigit = true;
        }

        public int RequiredLength { get; set; }

        public int RequiredUniqueChars { get; set; }

        public bool RequireNonAlphanumeric { get; set; }

        public bool RequireLowercase { get; set; }
     
        public bool RequireUppercase { get; set; }

        public bool RequireDigit { get; set; }
    }
}