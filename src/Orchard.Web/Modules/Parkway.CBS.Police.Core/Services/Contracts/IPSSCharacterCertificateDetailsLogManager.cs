using Orchard;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSCharacterCertificateDetailsLogManager<PSSCharacterCertificateDetailsLog> : IDependency, IBaseManager<PSSCharacterCertificateDetailsLog>
    {
        string LogNewEntryQueryStringValue(long characterCertificateDetailsId);
    }
}
