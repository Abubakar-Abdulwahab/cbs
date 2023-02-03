using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Core.HelperModels
{
    public class ReferenceDataViewModel
    {
        public string StateName { get; set; }
        /// <summary>
        /// List of available ref data
        /// </summary>
        public List<string> RefData { get; set; }
    }
}