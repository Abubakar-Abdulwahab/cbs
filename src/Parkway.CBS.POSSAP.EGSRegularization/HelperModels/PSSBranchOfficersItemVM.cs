using Parkway.CBS.Police.Core.HelperModels;
using Parkway.CBS.Police.Core.VM;

namespace Parkway.CBS.POSSAP.EGSRegularization.HelperModel
{
    public class PSSBranchOfficersItemVM
    {
        public long Id { get; set; }

        public string BranchCode { get; set; }

        public string BranchCodeValue { get; set; }

        public string APNumber { get; set; }

        public string OfficerName { get; set; }

        public CommandVM OfficerCommand { get; set; }

        public string OfficerCommandValue { get; set; }

        public string OfficerCommandCode { get; set; }

        public string RankCode { get; set; }

        public string RankName { get; set; }

        public PoliceRankingVM Rank { get; set; }

        public string BankCode { get; set; }

        public string BankName { get; set; }

        public string IPPISNumber { get; set; }

        public string Gender { get; set; }

        public string AccountNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool HasError { get; set; }

        public string ErrorMessage { get; set; }
    }
}
