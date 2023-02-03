using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class SubRevenueHeadsListViewModel
    {
        public RevenueHead ParentRevenueHead { get; set; }
        public List<RevenueHead> RevenueHeads { get; set; }
        public dynamic Pager { get; set; }
        public RHIndexOptions Options { get; set; }

        public dynamic AdminBreadCrumb { get; set; }
    }
}