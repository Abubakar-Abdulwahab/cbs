using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem
{
    public class OfficersRequestResponseModel
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

        public int TotalNumberOfOfficers { get; set; }

        public List<OfficersItems> ReportRecords { get; set; }
    }

    public class OfficersItems
    {
        public string Name { get; set; }

        public string IPPISNumber { get; set; }

        public string ServiceNumber { get; set; }

        public string PhoneNumber { get; set; }

        public string Gender { get; set; }

        public string GenderCode { get; set; }

        public string RankName { get; set; }

        public string RankCode { get; set; }

        public string StateName { get; set; }

        public string StateCode { get; set; }

        public string CommandName { get; set; }

        public string CommandCode { get; set; }

        public string LGAName { get; set; }

        public string LGACode { get; set; }

        public string DateOfBirth { get; set; }

        public string StateOfOrigin { get; set; }

        public string AccountNumber { get; set; }

        public string BankCode { get; set; }

    }

}