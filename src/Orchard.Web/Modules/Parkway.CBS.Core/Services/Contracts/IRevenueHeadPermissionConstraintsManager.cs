using Orchard;
using Parkway.CBS.Core.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface IRevenueHeadPermissionConstraintsManager<RevenueHeadPermissionConstraints> : IDependency, IBaseManager<RevenueHeadPermissionConstraints>
    {

        /// <summary>
        /// This method fetches existing revenue head constraints for the expert system with the specified Id.
        /// </summary>
        /// <param name="expertSystemId"></param>
        /// <param name="permissionId">Permission Id</param>
        /// <returns></returns>
        IEnumerable<RevenueHeadPermissionsConstraintsVM> GetExistingConstraints(int expertSystemId, int permissionId);

        /// <summary>
        /// Delete records for expert system with specified Id.
        /// </summary>
        /// <param name="expertSystemId"></param>
        void DeleteExpertSystemRecords(int expertSystemId);




        #region Remove after migration
        void ClearTable();

        void UpdateMDAId(); 
        #endregion


    }
}
