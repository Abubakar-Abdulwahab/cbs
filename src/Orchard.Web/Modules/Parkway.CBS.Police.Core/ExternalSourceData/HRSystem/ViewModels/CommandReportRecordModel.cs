using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.ExternalSourceData.HRSystem.ViewModels
{
    public class CommandReportRecordModel
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string StateCode { get; set; }

        public string LgaCode { get; set; }

        public string ZoneCode { get; set; }

        public List<CommandReportRecordModel> Sub { get; set; }
    }
}