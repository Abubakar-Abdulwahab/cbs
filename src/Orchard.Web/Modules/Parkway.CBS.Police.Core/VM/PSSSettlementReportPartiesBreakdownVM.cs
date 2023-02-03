using Parkway.CBS.Core.HelperModels;
using Parkway.CBS.Police.Core.DTO;
using Parkway.CBS.Police.Core.HelperModels;
using System;
using System.Collections.Generic;

namespace Parkway.CBS.Police.Core.VM
{
    public class PSSSettlementReportPartiesBreakdownVM
    {
        public IEnumerable<PSSSettlementBatchItemsVM> ReportRecords { get; set; }

        public dynamic Pager { get; set; }

        public List<StateModelVM> StateAndLGAs { get; set; }

        public IEnumerable<CommandVM> Commands { get; set; }

        public IEnumerable<PSServiceVM> Services { get; set; }

        public string SettlementName { get; set; }

        public string FeePartyName { get; set; }

        public decimal AmountSettled { get; set; }

        public DateTime SettlementStartDate { get; set; }

        public DateTime SettlementEndDate { get; set; }

        public int TotalRecordCount { get; set; }

        public DateTime? SettlementDate { get; set; }
    }
}