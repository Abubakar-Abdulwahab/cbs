using Orchard;
using System.Collections.Generic;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IEscortViewRubricManager<EscortViewRubric> : IDependency, IBaseManager<EscortViewRubric>
    {

        /// <summary>
        /// Get the rubric for current level to admin level
        /// </summary>
        /// <param name="currentLevelId"></param>
        /// <param name="adminLevelId"></param>
        /// <returns>List{EscortViewRubricDTO}</returns>
        List<EscortViewRubricDTO> GetPermissionRubric(int currentLevelId, int adminLevelId);

        /// <summary>
        /// Get the rubric for current level to admin level
        /// </summary>
        /// <param name="currentLevelId"></param>
        /// <param name="adminLevelId"></param>
        /// <returns>List{EscortViewRubricDTO}</returns>
        List<EscortViewRubricDTO> GetPermissionRubric(int adminLevelId);

    }
}

