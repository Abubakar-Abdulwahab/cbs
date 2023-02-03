using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ExpertSystemPermissionsVM
    {
        public bool IsPermitted { get; set; }

        public EnumExpertSystemPermissions Permission { get; set; }
    }
}