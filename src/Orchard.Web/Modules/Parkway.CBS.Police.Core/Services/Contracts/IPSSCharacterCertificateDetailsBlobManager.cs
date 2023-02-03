using Orchard;
using Parkway.CBS.Police.Core.VM;
using Parkway.CBS.Core.Services.Contracts;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSCharacterCertificateDetailsBlobManager<PSSCharacterCertificateDetailsBlob> : IDependency, IBaseManager<PSSCharacterCertificateDetailsBlob>
    {
        /// <summary>
        /// Returns blob details for character certificate
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        PSSCharacterCertificateDetailsBlobVM GetCharacterCertificateBlobDetails(long requestId);


        /// <summary>
        /// Updates passport photo
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="characterCertificateDetailsId"></param>
        /// <returns></returns>
        bool UpdatePassportPhoto(string filePathName, long characterCertificateDetailsId);


        /// <summary>
        /// Updates passport bio data page
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="characterCertificateDetailsId"></param>
        /// <returns></returns>
        bool UpdatePassportBioDataPage(string filePathName, long characterCertificateDetailsId);
    }
}
