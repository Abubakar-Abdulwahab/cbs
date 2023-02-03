using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels.Charts
{
    public class ExpectationLineChart
    {
        public string[][] Labels { get; set; }
        public string BackGroundColors { get; set; }
        public string BorderColors { get; set; }
        public decimal[] ExpectedAmountData { get; set; }
    }
}