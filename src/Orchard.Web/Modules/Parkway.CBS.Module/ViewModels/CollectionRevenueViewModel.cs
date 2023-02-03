using Parkway.CBS.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class CollectionRevenueViewModel
    {
        public int TaxPayerType { get; set; }

        public IEnumerable<SelectListItem> RevenueHeads { get; set; }
    }
}