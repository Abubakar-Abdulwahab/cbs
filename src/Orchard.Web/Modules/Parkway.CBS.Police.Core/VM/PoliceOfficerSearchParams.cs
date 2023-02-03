namespace Parkway.CBS.Police.Core.VM
{
    public class PoliceOfficerSearchParams
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public int AdminUserId { get; set; }

        public bool DontPageData { get; set; }

        public string SelectedCommand { get; set; }

        public int CommandId { get; set; }

        public int State { get; set; }

        public int LGA { get; set; }

        public string IPPISNo { get; set; }

        public string IdNumber { get; set; }

        public int Rank { get; set; }

        public string OfficerName { get; set; }
        public string StateCode { get; internal set; }
        public string LGACode { get; internal set; }
        public string SelectedStateCode { get; internal set; }
        public string SelectedLGACode { get; internal set; }
        public string SelectedRank { get; internal set; }
        public string CommandCode { get; internal set; }
        public string RankCode { get; internal set; }
        public string GenderCode { get; internal set; }
        public string ServiceNumber { get; internal set; }
    }
}