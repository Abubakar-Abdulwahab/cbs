using Orchard;
using Orchard.Security.Permissions;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.OSGOF.Admin.ViewModels;
using System.Web;

namespace Parkway.CBS.OSGOF.Admin.Controllers.Handlers.Contracts
{
    public interface ICellSitesHandler : IDependency
    {
       
        /// <summary>
        /// Check for permission
        /// </summary>
        /// <param name="permission"></param>
        /// <exception cref="UserNotAuthorizedForThisActionException"></exception>
        void CheckForPermission(Permission permission);

        /// <summary>
        /// Get the paged data of cell sites staging
        /// </summary>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>CellSitesStagingReportVM</returns>
        CellSitesStagingReportVM GetScheduleStagingData(string scheduleBatchRef, int take, int skip);


        CellSitesStagingReportVM CompleteCellSiteProcessing(string scheduleRef);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="operatorId"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        CellSitesVM GetCellSites(long operatorId, int page, int pageSize);


        /// <summary>
        /// Process cell sites file
        /// </summary>
        /// <param name="payerId"></param>
        /// <param name="file"></param>
        /// <returns>CellSitesFileValidationObject</returns>
        CellSitesFileValidationObject CreateCellSites(string payerId, HttpPostedFileBase file, UserPartRecord adminUser, CBSUser loggedInUser);


    }
}
