namespace Parkway.CBS.Police.Core.VM
{
    public class TaxEntitySubCategoryVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int CategoryId { get; set; }

        public int SubSubTaxEntityCategoryId { get; set; }
    }
}