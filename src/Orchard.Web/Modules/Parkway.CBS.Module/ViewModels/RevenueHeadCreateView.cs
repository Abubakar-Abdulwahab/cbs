using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class RevenueHeadCreateView
    {
        public string MDAName { get; set; }
        public string MDASlug { get; set; }
        public RevenueHead RevenueHead { get; set; }

        public dynamic AdminBreadCrumb { get; set; }

        public string ParentSlug { get; set; }
        public int ParentId { get; set; }
        public int MDAId { get; set; }
    }
}