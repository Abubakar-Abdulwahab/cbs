using Parkway.CBS.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class UploadCellSiteVM
    {
        public string PayerId { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public string ErrorMessage { get; set; }
    }
}