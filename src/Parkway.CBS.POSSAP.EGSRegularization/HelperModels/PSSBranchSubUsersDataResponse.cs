using System.Collections.Generic;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModel
{
    public class PSSBranchSubUsersDataResponse
    {
        /// <summary>
        /// check to see if there are any errors here, that is if the header values have been read to be in correct order
        /// </summary>
        public HeaderValidateObject HeaderValidateObject { get; set; }

        /// <summary>
        /// if HeaderValidateObject.Error is false, get the list of PSSBranchSubUsersItemVM
        /// </summary>
        public List<PSSBranchSubUsersItemVM> PSSBranchSubUsersLineRecords { get; set; }
    }
}
