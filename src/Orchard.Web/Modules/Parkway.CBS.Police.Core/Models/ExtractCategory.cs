using Parkway.CBS.Core.Models;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Models
{
    public class ExtractCategory : CBSModel
    {
        public virtual string Name { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual  ICollection<ExtractSubCategory> SubCategories { get; set; }

        public virtual bool FreeForm { get; set; }
    }
}