using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface ICommandCategoryManager<CommandCategory> : IDependency, IBaseManager<CommandCategory>
    {
        /// <summary>
        /// Get list of command categories
        /// </summary>
        /// <returns>IEnumerable<CommandCategoryVM></returns>
        IEnumerable<CommandCategoryVM> GetCategories();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        IEnumerable<CommandCategory> GetCategory(int categoryId);

        /// <summary>
        /// Gets list of command categories with no parents i.e the parent command categories
        /// </summary>
        /// <returns></returns>
        IEnumerable<CommandCategoryVM> GetParentCategories();

    }
}
