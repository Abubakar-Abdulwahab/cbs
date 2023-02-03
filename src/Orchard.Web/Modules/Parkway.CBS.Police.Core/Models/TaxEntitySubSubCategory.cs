using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class TaxEntitySubSubCategory : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual TaxEntitySubCategory TaxEntitySubCategory { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual bool IsDefault { get; set; }
    }
}