using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Module.Web.ViewModels
{
    public class GenerateInvoiceVM
    {
        public string TaxPayerType { get; set; }

        public string RevenueHeadIdentifier { get; set; }

        public List<TaxEntityCategoryVM> TaxCategories { get; set; }

        public IEnumerable<RevenueHead> RevenueHeads { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public bool AllowCategorySelect { get; set; }
    }
}