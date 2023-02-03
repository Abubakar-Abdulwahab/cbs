using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxEntityProfileHelper
    {
        //identifies that this profile was a new profile
        public bool NewProfile { get; set; }
        //message for the tax payer
        public string Message { get; set; }
        //tax entity model
        public TaxEntity TaxEntity { get; set; }
        //tax entity category
        public TaxEntityCategory Category { get; set; }
        //check whether tax entity category needs a logged in user
        public bool RequiresLogin { get; set; }
    }
}