namespace Parkway.CBS.Police.Core.VM
{
    public class PoliceRankingVM
    {
        public long Id { get; set; }

        public string RankName { get; set; }

        public decimal ServiceAmountRate { get; set; }

        public string OfficerName { get; set; }

        public string IPPISNumber { get; set; }

        public int RankLevel { get; set; }

        public string ExternalDataCode { get; set; }
    }
}