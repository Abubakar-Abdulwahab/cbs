using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class JsonRevenueViewModel
    {
        public IEnumerable<SelectListItem> RevenueHeads { get; set; }
    }
}