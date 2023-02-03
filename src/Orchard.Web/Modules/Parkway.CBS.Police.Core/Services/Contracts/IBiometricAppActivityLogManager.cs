using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IBiometricAppActivityLogManager<BiometricAppActivityLog> : IDependency, IBaseManager<BiometricAppActivityLog>
    {
    }
}
