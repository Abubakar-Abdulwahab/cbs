using Orchard;

namespace Parkway.CBS.Core.Services.Contracts
{
    public interface ITenantStateSettings<TenantStateSettings> : IDependency, IBaseManager<TenantStateSettings>
    {

        /// <summary>
        /// Get state
        /// </summary>
        /// <returns>TenantStateSettings</returns>
        TenantStateSettings GetState();
    }
}