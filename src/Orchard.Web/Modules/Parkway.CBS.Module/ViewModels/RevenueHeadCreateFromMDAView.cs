using Parkway.CBS.Core.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class RevenueHeadCreateFromMDAView
    {
        public string MDAName { get; set; }
        public string MDASlug { get; set; }
        public string CodePrefix { get; set; }
        public ICollection<RevenueHead> RevenueHeadsCollection { get; set; }
        public dynamic AdminBreadCrumb { get; set; }
        public List<SelectListItem> Mdas { get; set; }
    }
}