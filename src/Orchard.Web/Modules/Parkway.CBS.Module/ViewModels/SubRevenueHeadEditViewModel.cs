using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class SubRevenueHeadEditViewModel
    {
        public string SRHName { get; set; }
        public string SRHSlug { get; set; }
        public RevenueHead RevenueHead { get; set; }
    }
}