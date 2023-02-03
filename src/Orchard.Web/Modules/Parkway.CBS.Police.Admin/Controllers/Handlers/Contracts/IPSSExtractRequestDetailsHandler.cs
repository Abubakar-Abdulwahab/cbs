using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Admin.Controllers.Handlers.Contracts
{
    public interface IPSSExtractRequestDetailsHandler : IDependency
    {
        /// <summary>
        /// Generate extract document
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateExtractDocumentByteFile(string fileRefNumber);
    }
}
