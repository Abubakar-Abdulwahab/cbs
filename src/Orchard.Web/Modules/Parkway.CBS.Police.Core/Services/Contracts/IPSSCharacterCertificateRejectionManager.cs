using Orchard;
using Parkway.CBS.Core.Services.Contracts;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.Services.Contracts
{
    public interface IPSSCharacterCertificateRejectionManager<PSSCharacterCertificateRejection> : IDependency, IBaseManager<PSSCharacterCertificateRejection>
    {
        /// <summary>
        /// Gets character certificate details with specified file ref number
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CharacterCertificateDocumentVM GetCertificateDetails(string fileRefNumber);
    }
}
