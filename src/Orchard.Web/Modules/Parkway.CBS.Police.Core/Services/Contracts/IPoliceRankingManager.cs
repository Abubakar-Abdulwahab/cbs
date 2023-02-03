using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPoliceRankingManager<PoliceRanking> : IDependency, IBaseManager<PoliceRanking>
    {
        /// <summary>
        /// Get list of police rank
        /// </summary>
        /// <returns></returns>
        List<PoliceRankingVM> GetPoliceRanks();

        /// <summary>
        /// Get police rank details using rank id
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns></returns>
        PoliceRankingVM GetPoliceRank(long rankId);

        /// <summary>
        /// Get police rank details using rank code
        /// </summary>
        /// <param name="rankId"></param>
        /// <returns>PoliceRankingVM</returns>
        PoliceRankingVM GetPoliceRank(string rankCode);

    }


}
