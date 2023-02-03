using Orchard;
using Orchard.Data;
using Parkway.CBS.Police.Core.CoreServices.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models;
using Parkway.CBS.Police.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parkway.CBS.Police.Core.CoreServices
{
    public class CoreCommandCategory: ICoreCommandCategory
    {

        private readonly ICommandCategoryManager<CommandCategory> _commandCategoryManager;
        private readonly ITransactionManager _transactionManager;

        public CoreCommandCategory(IOrchardServices orchardService, ICommandCategoryManager<CommandCategory> commandCategoryManager)
        {
            _commandCategoryManager = commandCategoryManager;
            _transactionManager = orchardService.TransactionManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CommandCategoryVM> GetCategories()
        {
            return _commandCategoryManager.GetCategories();
        }
    }
}