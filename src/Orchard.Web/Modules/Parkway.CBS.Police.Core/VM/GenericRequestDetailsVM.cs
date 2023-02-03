using System;
using System.Collections.Generic;
using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.Police.Core.VM
{
    public class GenericRequestDetailsVM : PSSRequestDetailsVM
    {
        public ICollection<FormControlRevenueHeadValueVM> FormControlValues { get; set; }

        public string CommandStateName { get; set; }

        public string CommandLgaName { get; set; }
    }
}