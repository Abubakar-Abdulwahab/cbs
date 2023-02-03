using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts
{
    public interface IPSSBranchOfficersFileUploadValidation
    {
        /// <summary>
        /// Validates PSSBranchOfficers line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        PSSBranchOfficersItemVM ValidateExtractedPSSBranchOfficersLineItems(List<string> lineValues);
    }
}
