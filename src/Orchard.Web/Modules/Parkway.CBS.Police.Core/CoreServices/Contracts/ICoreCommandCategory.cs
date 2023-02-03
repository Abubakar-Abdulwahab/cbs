using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICoreCommandCategory : IDependency
    {
        /// <summary>
        /// Get list of command categories
        /// </summary>
        /// <returns>IEnumerable<CommandCategoryVM></returns>
        IEnumerable<CommandCategoryVM> GetCategories();
    }
}
