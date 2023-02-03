using Orchard;
using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPoliceOfficerManager<PoliceOfficer> : IDependency, IBaseManager<PoliceOfficer>
    {
        /// <summary>
        /// Get police officers of the rank with specified rankId that belong to the command with the specified commandId
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="rankId"></param>
        /// <returns></returns>
        List<PoliceOfficerVM> GetPoliceOfficersByCommandAndRankId(int commandId, long rankId);


        /// <summary>
        /// Get police officer detials
        /// </summary>
        /// <param name="officerId"></param>
        /// <returns>PoliceOfficerVM</returns>
        PoliceOfficerVM GetPoliceOfficerDetails(int officerId);

        /// <summary>
        /// Get officer Id using the ID Number
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns>int</returns>
        int GetPoliceOfficerId(string idNumber);

        /// <summary>
        /// Get count for number of active police officer
        /// </summary>
        /// <returns>IEnumerable<ReportStatsVM></returns>
        IEnumerable<ReportStatsVM> GetTotalOfficers();

    }
}
