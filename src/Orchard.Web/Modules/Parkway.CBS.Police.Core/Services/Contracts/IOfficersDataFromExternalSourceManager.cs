using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IOfficersDataFromExternalSourceManager<OfficersDataFromExternalSource> : IDependency, IBaseManager<OfficersDataFromExternalSource>
    {

        /// <summary>
        /// Get the count of items between the startSN
        /// and endSN
        /// </summary>
        /// <param name="startSN"></param>
        /// <param name="endSN"></param>
        /// <returns>int</returns>
        int GetCountWithinRange(int startSN, int endSN);

    }
}
