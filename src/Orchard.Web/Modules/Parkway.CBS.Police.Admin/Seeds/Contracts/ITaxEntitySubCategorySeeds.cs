using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Seeds.Contracts
{
    public interface ITaxEntitySubCategorySeeds : IDependency
    {
        /// <summary>
        /// Add tax entity sub category
        /// </summary>
        void AddTaxEntitySubCategory();

        /// <summary>
        /// Add tax entity sub sub category
        /// </summary>
        void AddTaxEntitySubSubCategory();
    }
}
