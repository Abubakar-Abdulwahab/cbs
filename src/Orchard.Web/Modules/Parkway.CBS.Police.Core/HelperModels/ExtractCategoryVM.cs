using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.HelperModels
{
    public class ExtractCategoryVM
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool FreeForm { get; set; }

        public IEnumerable<ExtractSubCategoryVM> SubCategories { get; set; }
    }   
}