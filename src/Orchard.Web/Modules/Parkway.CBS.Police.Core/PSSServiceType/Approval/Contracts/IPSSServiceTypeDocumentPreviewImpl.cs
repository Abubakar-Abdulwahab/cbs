using Orchard;
using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.Police.Core.PSSServiceType.Approval.Contracts
{
    public interface IPSSServiceTypeDocumentPreviewImpl : IDependency
    {
        PSSServiceTypeDefinition GetServiceTypeDefinition { get; }

        /// <summary>
        /// Generates a draft service document for preview before approval
        /// </summary>
        /// <param name="fileRefNumber"></param>
        /// <returns></returns>
        CreateCertificateDocumentVM CreateDraftServiceDocumentByteFile(string fileRefNumber);
    }
}
