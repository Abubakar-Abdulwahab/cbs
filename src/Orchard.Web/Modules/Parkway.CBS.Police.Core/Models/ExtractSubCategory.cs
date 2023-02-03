using Parkway.CBS.Core.Models;

namespace Parkway.CBS.Police.Core.Models
{
    public class ExtractSubCategory : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual ExtractCategory ExtractCategory { get; set; }

        public virtual bool FreeForm { get; set; }
    }
}