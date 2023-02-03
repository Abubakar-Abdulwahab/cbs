using Parkway.CBS.Core.HelperModels;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModel
{
    public class PSSBranchSubUsersItemVM
    {
        public long Id { get; set; }

        public LGAVM BranchLGA { get; set; }

        public StateModelVM BranchState { get; set; }

        public string BranchStateCode { get; set; }

        public string BranchLGACode { get; set; }

        public string BranchStateValue { get; set; }

        public string BranchLGAValue { get; set; }

        public string BranchAddress { get; set; }

        public string BranchName { get; set; }

        public string SubUserName { get; set; }

        public string SubUserEmail { get; set; }

        public string SubUserPhoneNumber { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}
