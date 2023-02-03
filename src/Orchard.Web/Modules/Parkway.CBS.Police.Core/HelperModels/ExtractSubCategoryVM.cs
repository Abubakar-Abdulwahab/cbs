namespace Parkway.CBS.Police.Core.HelperModels
{
    public class ExtractSubCategoryVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ExtractCategoryVM Category { get; set; }

        public bool FreeForm { get; set; }
    }
}