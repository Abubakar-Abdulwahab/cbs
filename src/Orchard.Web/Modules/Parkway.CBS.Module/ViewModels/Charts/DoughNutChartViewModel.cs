using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels.Charts
{
    public class DoughNutChartViewModel
    {
        public string[] Labels { get; set; }
        public string[] BackGroundColors { get; set; }
        public decimal[] Data { get; set; }
        public string Description { get; set; }
    }
}