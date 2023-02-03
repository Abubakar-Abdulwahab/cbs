using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parkway.CBS.POSSAP.EGSRegularization.Implementations.Contracts
{
    public interface IPSSBranchSubUsersFileUploadValidation
    {
        /// <summary>
        /// Validates PSSBranchSubUsers line items
        /// </summary>
        /// <param name="lineValues"></param>
        /// <returns></returns>
        PSSBranchSubUsersItemVM ValidateExtractedPSSBranchSubUsersLineItems(List<string> lineValues);
    }
}
