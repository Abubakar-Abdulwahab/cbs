using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IIdentificationTypeManager<IdentificationType> : IDependency, IBaseManager<IdentificationType>
    {

        /// <summary>
        /// Get identification type VM for identification type with specified Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>IdentificationTypeVM</returns>
        IdentificationTypeVM GetIdentificationTypeVM(int id);


        /// <summary>
        /// Check if the identity type has biometric support
        /// </summary>
        /// <param name="id"></param>
        /// <returns>bool</returns>
        bool HasBiometricSupport(int id);

    }
}
