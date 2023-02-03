using Parkway.Cashflow.Ng.Models;
using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class SetStateViewModel
    {
        public string Identifier { get; set; }
        
        /// <summary>
        /// List of available states
        /// </summary>
        public List<TenantCBSSettings> States { get; set; }
    }
}