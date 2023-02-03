using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class HierarchyViewModel
    {
        public string Name { get; set; }

        public IEnumerable<RevenueHead> RevenueHeads { get; set; }
    }
}