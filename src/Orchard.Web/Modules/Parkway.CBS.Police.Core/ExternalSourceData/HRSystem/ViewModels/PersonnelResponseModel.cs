using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels
{
    public class PersonnelResponseModel
    {
        public List<PersonnelReportRecord> ReportRecords { get; set; }

        public PersonnelRequestModel SearchFilter { get; set; }
    }
}