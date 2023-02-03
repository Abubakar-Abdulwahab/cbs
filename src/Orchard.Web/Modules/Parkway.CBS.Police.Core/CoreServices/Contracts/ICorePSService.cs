using Orchard;

namespace Parkway.CBS.Police.Core.CoreServices.Contracts
{
    public interface ICorePSService : IDependency
    {

        /// <summary>
        /// Get the initialization request flow level Id for this service
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns>int</returns>
        /// <exception cref="NoRecordFoundExceptiion"></exception>
        int GetInitFlow(int serviceId);

    }
}
