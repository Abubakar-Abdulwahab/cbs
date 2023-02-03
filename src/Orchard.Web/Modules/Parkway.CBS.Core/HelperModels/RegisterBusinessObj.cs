using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class RegisterBusinessObj
    {
        public HeaderObj HeaderObj { get; set; }

        public RegisterBusinessModel RegisterBusinessModel { get; set; }

        public List<StateModel> StateLGAs { get; set; }

        public List<LGA> ListLGAs { get; set; }

        public string TaxPayerType { get; set; }
    }
}