namespace Parkway.CBS.Core.Models
{
    public class TaxCategoryTaxCategoryPermissions : CBSModel
    {
        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        public virtual TaxCategoryPermissions TaxCategoryPermissions { get; set; }
    }
}