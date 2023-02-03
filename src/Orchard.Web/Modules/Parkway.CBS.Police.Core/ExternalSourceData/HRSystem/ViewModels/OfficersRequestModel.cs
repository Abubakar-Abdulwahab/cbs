namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem
{
    public class OfficersRequestModel
    {
        public string StateCode { get; set; }

        public string LGACode { get; set; }

        public string CommandCode { get; set; }

        public string RankCode { get; set; }

        public string GenderCode { get; set; }

        public string ServiceNumber { get; set; }

        public string Name { get; set; }

        public string IPPISNumber { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}