using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.VM
{
    public class SelectServiceVM
    {
        public List<TaxEntityCategoryVM> TaxCategories { get; set; }

        public List<PSSRequestTypeVM> Services { get; set; }

        public HeaderObj HeaderObj { get; set; }

        public bool HasMessage { get; set; }

        public bool AllowCategorySelect { get; set; }

        public string TaxPayerType { get; set; }

        public string ServiceIdentifier { get; set; }

        public string SubCategoryIdentifier { get; set; }

        public string SubSubCategoryIdentifier { get; set; }

        public FlashObj FlashObj { get; set; }
    }
}