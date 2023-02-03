using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parkway.CBS.OSGOF.Admin.Models;
using System.Threading.Tasks;
using Parkway.CBS.OSGOF.Admin.ViewModels;

namespace Parkway.CBS.OSGOF.Admin.Services.Contracts
{
    public interface ICellSiteManager<CellSites> : IDependency, IBaseManager<CellSites>
    {
        /// <summary>
        /// Get the Cell Sites list
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IEnumerable<Models.CellSites> GetCellSites(long operatorId, int skip, int take);


        /// <summary>
        /// Get the aggregate total number of Cell Sites
        /// </summary>
        /// <returns></returns>
        IEnumerable<CellSitesVM> GetAggregateForCellSites(long operatorID);

        /// <summary>
        /// Get cell sites list for a particular operator based on a particular LGA 
        /// </summary>
        /// <param name="taxEntityId"></param>
        /// <param name="LGAId"></param>
        /// <returns></returns>
        List<CellSitesDropdownBindingVM> GetOperatorCellSites(int taxEntityId, int LGAId);


        /// <summary>
        /// Get cell site details using the Id
        /// </summary>
        /// <param name="cellSiteId"></param>
        /// <returns></returns>
        CellSitesDetailsVM GetCellSite(int cellSiteId);
    }
}
