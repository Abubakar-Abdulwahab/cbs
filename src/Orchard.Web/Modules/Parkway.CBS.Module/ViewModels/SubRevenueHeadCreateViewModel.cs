using Parkway.CBS.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Module.ViewModels
{
    public class SubRevenueHeadCreateViewModel
    {
        public string SRHName { get; set; }
        public string SRHSlug { get; set; }
        public int SRHId { get; set; }
             
        public bool HasBilling { get; set; }
        public string CodePrefix { get; set; }
        public ICollection<RevenueHead> RevenueHeadsCollection { get; set; }

        public dynamic AdminBreadCrumb { get; set; }
    }
}