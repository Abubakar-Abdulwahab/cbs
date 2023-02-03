using Orchard;
using Orchard.Users.Models;
using Parkway.CBS.Core.Models;
using Parkway.CBS.Entities.VMs;
using Parkway.CBS.OSGOF.Admin.ViewModels;

namespace Parkway.CBS.OSGOF.Admin.CoreServices.Contracts
{
    public interface ICoreCellSites : IDependency
    {
        CellSitesFileValidationObject StoreCellSitesIntoStaging(TaxEntity taxProfile, UserPartRecord adminUser, CBSUser loggedInUser, string path);


        /// <summary>
        /// Get tax category
        /// </summary>
        /// <param name="categoryType"></param>
        /// <returns>TaxEntityCategory</returns>
        /// <exception cref="NoCategoryFoundException"></exception>
        TaxEntityCategory GetTaxEntityCategory(int catId);

        /// <summary>
        /// Get cell sites for staging
        /// </summary>
        /// <param name="scheduleBatchRef"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns>CellSitesStagingReportVM</returns>
        CellSitesStagingReportVM GetCellSitesStaging(string scheduleBatchRef, int take, int skip);


        /// <summary>
        /// Move the valid cell sites from staging to the main table
        /// </summary>
        /// <param name="scheduleRef"></param>
        /// <param name="userPartRecord"></param>
        CellSitesStagingReportVM MoveCellSitesFromStagingToMainTable(string scheduleRef, UserPartRecord userPartRecord);
    }
}
