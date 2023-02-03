using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class RegisterCBSUserObj
    {
        public HeaderObj HeaderObj { get; set; }

        public string ErrorMessage { get; set; }

        public bool Error { get; set; }

        public RegisterCBSUserModel RegisterCBSUserModel { get; set; }

        public List<TaxEntityCategory> TaxCategories { get; set; }

        public string TaxPayerType { get; set; }
    }
}