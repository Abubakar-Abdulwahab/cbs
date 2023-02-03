using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class JsonRevenueViewModel
    {
        public IEnumerable<SelectListItem> RevenueHeads { get; set; }
    }
}