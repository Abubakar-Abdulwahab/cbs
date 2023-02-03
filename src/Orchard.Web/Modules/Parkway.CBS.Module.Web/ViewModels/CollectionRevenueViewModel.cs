using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class CollectionRevenueViewModel
    {
        public int TaxPayerType { get; set; }

        public IEnumerable<SelectListItem> RevenueHeads { get; set; }
    }
}