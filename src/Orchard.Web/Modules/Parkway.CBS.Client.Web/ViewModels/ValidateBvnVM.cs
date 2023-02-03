using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Client.Web.ViewModels
{
    public class ValidateBvnVM
    {
        public HeaderObj HeaderObj { get; set; }

        public RegisterCBSUserModel RegisterCBSUserModel { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public string TaxPayerType { get; set; }

        public String ErrorMessage { get; set; }

        public bool Error { get; set; }

        public string Message { get; set; }
    }
}