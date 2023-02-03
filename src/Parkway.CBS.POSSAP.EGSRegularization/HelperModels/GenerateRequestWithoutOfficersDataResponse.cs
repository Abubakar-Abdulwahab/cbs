using Parkway.CBS.POSSAP.EGSRegularization.HelperModel;
using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModels
{
    public class GenerateRequestWithoutOfficersDataResponse
    {
        /// <summary>
        /// check to see if there are any errors here, that is if the header values have been read to be in correct order
        /// </summary>
        public HeaderValidateObject HeaderValidateObject { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of GenerateRequestWithoutOfficersUploadItemVM
        /// </summary>
        public List<GenerateRequestWithoutOfficersUploadItemVM> GenerateRequestWithoutOfficersLineRecords { get; set; }
    }
}
