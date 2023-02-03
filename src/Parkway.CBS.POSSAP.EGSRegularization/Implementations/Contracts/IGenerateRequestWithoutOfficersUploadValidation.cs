using Parkway.CBS.POSSAP.EGSRegularization.HelperModels;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts
{
    public interface IGenerateRequestWithoutOfficersUploadValidation
    {
        /// <summary>
        /// Validates PSSBranchSubUsers line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        GenerateRequestWithoutOfficersUploadItemVM ValidateExtractedGenerateRequestWithoutOfficersLineItems(List<string> lineValues);
    }
}
