using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class TaxEntitySubCategory : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual TaxEntityCategory TaxEntityCategory { get; set; }

        public virtual bool IsActive { get; set; }
    }
}