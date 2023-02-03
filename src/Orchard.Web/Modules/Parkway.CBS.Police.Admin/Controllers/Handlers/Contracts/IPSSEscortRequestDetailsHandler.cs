using Orchard;
using Parkway.CBS.Police.Core.HelperModels;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSEscortRequestDetailsHandler : IDependency
    {
        /// <summary>
        /// Create dispatch note byte file
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateDispatchNoteByteFile(string fileRefNumber);
    }
}
