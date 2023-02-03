using Orchard;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Core.PSSIdentification.Contracts
{
    public interface IIdentityBiometric : IDependency
    {

        /// <summary>
        /// Implementation class name
        /// </summary>
        /// <returns>string</returns>
        string ImplementingClassName { get; }

    }
}
