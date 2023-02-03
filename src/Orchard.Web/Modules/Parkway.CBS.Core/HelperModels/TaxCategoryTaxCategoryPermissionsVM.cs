using System.Collections.Generic;

namespace Parkway.CBS.Core.HelperModels
{
    public class TaxCategoryTaxCategoryPermissionsVM
    {
        public int TaxCategoryId { get; set; }

        public IEnumerable<TaxCategoryPermissionsVM> TaxCategoryPermissions { get; set; }
    }
}