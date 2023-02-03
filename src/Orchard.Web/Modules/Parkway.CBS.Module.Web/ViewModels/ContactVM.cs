using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class AboutVM
    {
        public string TenantName { get; set; }

        public HeaderObj HeaderObj { get; set; }
    }


    public class ContactVM
    {
        public HeaderObj HeaderObj { get; set; }
    }
}