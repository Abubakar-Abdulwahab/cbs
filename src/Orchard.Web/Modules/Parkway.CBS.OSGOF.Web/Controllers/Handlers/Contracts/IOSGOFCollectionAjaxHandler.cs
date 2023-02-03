using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Module.Web.Controllers.Handlers.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.OSGOF.Web.Controllers.Handlers.Contracts
{
    public interface IOSGOFCollectionAjaxHandler : IModuleCollectionAjaxHandler
    {
        /// <summary>
        /// Get the list of cell sites attached to the specified operator Id
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="LGAId"></param>
        /// <returns></returns>
        APIResponse GetOperatorCellSites(string taxEntityId, string LGAId);

        /// <summary>
        /// Get a cell site details based on Id
        /// </summary>
        /// <param name="cellSiteId"></param>
        /// <returns></returns>
        APIResponse GetCellSite(string cellSiteId);
    }
}
