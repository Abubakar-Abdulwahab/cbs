using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Core.Models.Enums;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Parkway.CBS.Module.ViewModels
{
    public class GenerateInvoiceVM
    {
        public string TaxPayerType { get; set; }

        public string RevenueHeadIdentifier { get; set; }

        public List<TaxEntityCategory> TaxCategories { get; set; }

        public IEnumerable<RevenueHead> RevenueHeads { get; set; }

        public bool HasErrors { get; set; }

        public string ErrorMessage { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public bool AllowCategorySelect { get; set; }
    }    
}