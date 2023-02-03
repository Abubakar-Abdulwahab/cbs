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
    public interface IPSSExtractDocumentManager<PSSExtractDocument> : IDependency, IBaseManager<PSSExtractDocument>
    {
        /// <summary>
        /// Gets extract document details
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        ExtractDocumentVM GetExtractDocumentDetails(string fileRefNumber);
    }
}
