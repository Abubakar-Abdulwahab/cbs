using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class FlashObj
    {
        public string Message { get; set; }

        public string MessageTitle { get; set; }

        public FlashType FlashType { get; set; }

        public bool RedirectToLogin { get; set; }
    }
}